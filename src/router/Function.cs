using Amazon.DynamoDBv2.DocumentModel;
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

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ServerlessPizza.Router
{
    public class Function
    {
        // Credentials, region are pulled from instance information
        private AmazonSQSClient _client = new AmazonSQSClient();

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            Console.WriteLine($"Incoming Event: {JsonConvert.SerializeObject(dynamoEvent)}");

            // Process records and send SQS messages asynchronously
            IEnumerable<Task<SendMessageResponse>> tasks = dynamoEvent.Records.Select(r => ProcessRecord(r));

            // Wait for everything to finish, then exit
            Task.WhenAll(tasks).Wait();
        }

        private Task<SendMessageResponse> ProcessRecord(DynamoDBEvent.DynamodbStreamRecord record)
        {
            string json = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
            List<AttributeValue> events;

            // Be prepared for anomalous records
            try
            {
                events = record.Dynamodb.NewImage["events"].L;
            }
            catch (Exception)
            {
                Console.WriteLine($"Invalid record schema: {json}");
                return default;
            }

            // If there aren't any events yet, send the order to Prep
            if (events.Count == 0)
            {
                return SendSQSMessage("serverless-pizza-prep", json);
            }

            // Get the type number of an event
            // A function delegate is used here, but a regular method works just the same
            Func<AttributeValue, int> getTypeNumber = e =>
            {
                try
                {
                    string type = e.M["type"].S;
                    switch (type)
                    {
                        case "Prep":
                            return 1;
                        case "Cook":
                            return 2;
                        case "Finish":
                            return 3;
                        case "Delivery":
                            return 4;
                        default:
                            return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            };

            // List items in a DynamoDB record are not sorted; find the event with the largest type number
            var lastEvent = events.OrderBy(e => getTypeNumber(e)).Last();

            // If the event is still in progress, don't send a message
            if (!lastEvent.M.ContainsKey("end"))
            {
                return default;
            }

            // Using early return enhances readability and keeps this switch statement at a reasonable level of indentation
            switch (getTypeNumber(lastEvent))
            {
                case 1: // Prep
                    return SendSQSMessage("serverless-pizza-cook", json);
                case 2: // Cook
                    return SendSQSMessage("serverless-pizza-finish", json);
                case 3: // Finish
                    return SendSQSMessage("serverless-pizza-deliver", json);
                case 4: // Delivery
                    Console.WriteLine("Order delivered, nothing more to do.");
                    return default;
                default:
                    Console.WriteLine($"Unknown event: {JsonConvert.SerializeObject(lastEvent)}");
                    return default;
            }
        }

        private Task<SendMessageResponse> SendSQSMessage(string functionName, string payload)
        {
            Console.WriteLine($"Sending message to '{functionName}' queue");

            // Environment variable set in Lambda console
            string prefix = Environment.GetEnvironmentVariable("SqsUrlPrefix");
            string url = prefix + functionName;

            SendMessageRequest request = new SendMessageRequest(url, payload);
            return _client.SendMessageAsync(request);
        }
    }
}