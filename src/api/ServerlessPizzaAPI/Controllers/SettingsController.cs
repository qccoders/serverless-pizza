namespace ServerlessPizzaAPI.Controllers
{
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private IAmazonDynamoDB Client { get; }

        public SettingsController(IAmazonDynamoDB amazonDynamoDBClient)
        {
            Client = amazonDynamoDBClient;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get()
        {
            Table table = Table.LoadTable(Client, "serverless-pizza-settings");

            var crustsTask = table.GetItemAsync("crusts");
            var toppingsTask = table.GetItemAsync("toppings");

            await Task.WhenAll(crustsTask, toppingsTask);

            dynamic crusts = JsonConvert.DeserializeObject((await crustsTask).ToJson());
            dynamic toppings = JsonConvert.DeserializeObject((await toppingsTask).ToJson());

            return new { Crusts = crusts.value, Toppings = toppings.value };
        }
    }
}
