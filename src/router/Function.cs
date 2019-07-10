using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using ServerlessPizzaAPI.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ServerlessPizza.Router
{
    public class Function
    {
        // Credentials, region are pulled from instance information
        private AmazonSQSClient _client = new AmazonSQSClient();

        private Task<SendMessageResponse> SendSQSMessage(string functionName, string payload)
        {
            // Environment variables set in Lambda console
            string prefix = Environment.GetEnvironmentVariable("SqsUrlPrefix");
            string url = prefix + functionName;

            SendMessageRequest request =  new SendMessageRequest(url, payload);
            return _client.SendMessageAsync(request);
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            Console.WriteLine($"Incoming Event: {JsonConvert.SerializeObject(dynamoEvent)}");

            foreach (DynamoDBEvent.DynamodbStreamRecord record in dynamoEvent.Records)
            {
                string json = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
                Console.WriteLine($"Record: {json}");

                var order = JsonConvert.DeserializeObject<Order>(json);

                if (order.Events == null || !order.Events.Any())
                {
                    Console.WriteLine($"Sending message to Prep queue");
                    SendSQSMessage("serverless-pizza-prep", json).Wait();
                }
                else
                {
                    var orderedEvents = order.Events.OrderBy(e => (int)e.Type);

                    Console.WriteLine($"Events: {string.Join(", ", orderedEvents.Select(e => e.Type))}");

                    var lastEvent = orderedEvents.Last();

                    Console.WriteLine($"Last event: {JsonConvert.SerializeObject(lastEvent)}");

                    if (lastEvent.End == null)
                    {
                        Console.WriteLine($"Last event end is null, nothing to route.");
                    }

                    switch (lastEvent.Type)
                    {
                        case EventType.Prep:
                            Console.WriteLine($"Sending message to Cook queue");
                            SendSQSMessage("serverless-pizza-cook", json).Wait();
                            break;
                        case EventType.Cook:
                            Console.WriteLine($"Sending message to Finish queue");
                            SendSQSMessage("serverless-pizza-finish", json).Wait();
                            break;
                        case EventType.Finish:
                            Console.WriteLine($"Sending message to Deliver queue");
                            SendSQSMessage("serverless-pizza-deliver", json).Wait();
                            break;
                        case EventType.Delivery:
                            Console.WriteLine($"Order delivered, nothing more to do.");
                            break;
                    }
                }
            }
        }
    }
}