namespace ServerlessPizzaAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using ServerlessPizzaAPI.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string TABLE_NAME = "serverless-pizza-orders";
        private const string ORDERS_CACHE_KEY = "orders";

        private static readonly SemaphoreSlim cacheSyncRoot = new SemaphoreSlim(1, 1);

        private IAmazonDynamoDB Client { get; }
        private IMemoryCache Cache { get; }

        public OrdersController(IMemoryCache memoryCache, IAmazonDynamoDB amazonDynamoDBClient)
        {
            Cache = memoryCache;
            Client = amazonDynamoDBClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get([FromQuery]string name)
        {
            var orders = await FetchOrdersAsync();

            if (!string.IsNullOrEmpty(name))
            {
                orders = orders.Where(o => o.Name == name);
            }

            return Ok(orders.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById([FromRoute]string id)
        {
            var order = (await FetchOrdersAsync())
                .FirstOrDefault(o => o.Id == id);

            if (order == default)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task Put([FromBody]Order order)
        {
            Table table = Table.LoadTable(Client, TABLE_NAME);

            var json = JsonConvert.SerializeObject(order, new JsonSerializerSettings
            {
                Converters =
                {
                    new StringEnumConverter()
                },
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            await table.PutItemAsync(Document.FromJson(json));

            Cache.Remove(ORDERS_CACHE_KEY);
        }

        private async Task<IEnumerable<Order>> FetchOrdersAsync()
        {
            var cacheHit = true;
            var sw = new Stopwatch();

            sw.Start();

            if (!Cache.TryGetValue(ORDERS_CACHE_KEY, out List<Order> orders))
            {
                await cacheSyncRoot.WaitAsync();

                try
                {
                    orders = await Cache.GetOrCreateAsync(ORDERS_CACHE_KEY, async entry =>
                    {
                        cacheHit = false;

                        Table table = Table.LoadTable(Client, TABLE_NAME);

                        var config = new ScanOperationConfig()
                        {
                            Select = SelectValues.AllAttributes
                        };

                        var search = table.Scan(config);

                        var orderDocuments = new List<Document>();

                        do
                        {
                            orderDocuments.AddRange(await search.GetNextSetAsync());
                        } while (!search.IsDone);

                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                        return orderDocuments
                            .Select(d => JsonConvert.DeserializeObject<Order>(d.ToJson()))
                            .ToList();
                    });
                }
                finally
                {
                    cacheSyncRoot.Release();
                }
            }

            sw.Stop();

            Response.Headers.Add("X-Execution-Time", sw.ElapsedMilliseconds + "ms");
            Response.Headers.Add("X-Cache", cacheHit ? "HIT" : "MISS");

            return orders;
        }
    }
}