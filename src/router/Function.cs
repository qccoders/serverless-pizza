using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace router
{
    public class Function
    {
        // Credentials are pulled from environment variables auto-populated by Lambda
        private AmazonSQSClient _client = new AmazonSQSClient();

        private void SendSQSMessage(string functionName, string payload)
        {
            var url = $"https://sqs.us-east-2.amazonaws.com/551524640723/{functionName}";
            var request =  new SendMessageRequest(url, payload);
            _client.SendMessageAsync(request).Wait();
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            LambdaLogger.Log($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                List<AttributeValue> events = record.Dynamodb.NewImage["events"].L;
                string orderId = record.Dynamodb.NewImage["id"].S;

                if (events.Count == 0)
                {
                    SendSQSMessage("ServerlessPizzaPrep", orderId);
                }
                else
                {
                    switch (events.Last().M["type"].S)
                    {
                        case "prep":
                            SendSQSMessage("ServerlessPizzaCook", orderId);
                            break;
                        case "cook":
                            SendSQSMessage("ServerlessPizzaFinish", orderId);
                            break;
                        case "finish":
                            LambdaLogger.Log($"Order {orderId} complete.");
                            break;
                    }
                }
            }

            LambdaLogger.Log("Stream processing complete.");
        }
    }
}