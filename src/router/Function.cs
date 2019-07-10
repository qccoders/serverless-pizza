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
                string json = JsonConvert.SerializeObject(record);
                Console.WriteLine($"Record: {json}");

                List<AttributeValue> events;

                try
                {
                    events = record.Dynamodb.NewImage["events"].L;
                }
                catch (Exception)
                {
                    return;
                }


                if (events.Count == 0)
                {
                    Console.WriteLine($"Sending message to Prep queue");
                    SendSQSMessage("serverless-pizza-prep", json).Wait();
                    return;
                }

                Func<AttributeValue, int> getTypeNumber = e =>
                {
                    try
                    {
                        string type = e.M["type"].N;
                        return int.Parse(type);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                };

                var lastEvent = events.OrderBy(e => getTypeNumber(e)).Last();

                if (lastEvent.M["end"].S == null || lastEvent.M["end"].S.Trim() == "")
                {
                    return;
                }

                switch (getTypeNumber(lastEvent))
                {
                    case 0:
                        Console.WriteLine($"Unknown event: {JsonConvert.SerializeObject(lastEvent)}");
                        break;
                    case 1: // Prep
                        Console.WriteLine("Sending message to Cook queue");
                        SendSQSMessage("serverless-pizza-cook", json).Wait();
                        break;
                    case 2: // Cook
                        Console.WriteLine("Sending message to Finish queue");
                        SendSQSMessage("serverless-pizza-finish", json).Wait();
                        break;
                    case 3: // Finish
                        Console.WriteLine("Sending message to Deliver queue");
                        SendSQSMessage("serverless-pizza-deliver", json).Wait();
                        break;
                    case 4: // Deliver
                        Console.WriteLine("Order delivered, nothing more to do.");
                        break;
                }
            }
        }
    }
}