namespace ServerlessPizzaAPI.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private const string TABLE_NAME = "serverless-pizza-settings";
        private const string SETTINGS_CACHE_KEY = "settings";

        private static readonly SemaphoreSlim cacheSyncRoot = new SemaphoreSlim(1, 1);

        private IAmazonDynamoDB Client { get; }
        private IMemoryCache Cache { get; }

        public SettingsController(IMemoryCache memoryCache, IAmazonDynamoDB amazonDynamoDBClient)
        {
            Cache = memoryCache;
            Client = amazonDynamoDBClient;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get()
        {
            if (!Cache.TryGetValue(SETTINGS_CACHE_KEY, out var settings))
            {
                await cacheSyncRoot.WaitAsync();

                try
                {
                    settings = await Cache.GetOrCreateAsync(SETTINGS_CACHE_KEY, async entry =>
                    {
                        Table table = Table.LoadTable(Client, TABLE_NAME);

                        var crustsTask = table.GetItemAsync("crusts");
                        var toppingsTask = table.GetItemAsync("toppings");

                        await Task.WhenAll(crustsTask, toppingsTask);

                        dynamic crusts = JsonConvert.DeserializeObject((await crustsTask).ToJson());
                        dynamic toppings = JsonConvert.DeserializeObject((await toppingsTask).ToJson());

                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);

                        return new { Crusts = crusts.value, Toppings = toppings.value };
                    });
                }
                finally
                {
                    cacheSyncRoot.Release();
                }
            }

            return settings;
        }
    }
}
