using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace router
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
            LambdaLogger.Log($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                List<AttributeValue> events = record.Dynamodb.NewImage["events"].L;
                string orderId = record.Dynamodb.NewImage["id"].S;

                if (events.Count == 0)
                {
                    SendSQSMessage("serverless-pizza-prep", orderId).Wait();
                }
                else
                {
                    switch (events.Last().M["type"].S)
                    {
                        case "prep":
                            SendSQSMessage("serverless-pizza-cook", orderId).Wait();
                            break;
                        case "cook":
                            SendSQSMessage("serverless-pizza-finish", orderId).Wait();
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