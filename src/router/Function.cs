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
            foreach (DynamoDBEvent.DynamodbStreamRecord record in dynamoEvent.Records)
            {
                string json = JsonConvert.SerializeObject(record);

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
                    SendSQSMessage("serverless-pizza-prep", json).Wait();
                    return;
                }

                var lastEvent = events.Last();

                if (lastEvent.M["end"].S == null || lastEvent.M["end"].S.Trim() == "")
                {
                    return;
                }

                else
                {
                    switch (events.Last().M["type"].S)
                    {
                        case "prep":
                            SendSQSMessage("serverless-pizza-cook", json).Wait();
                            break;
                        case "cook":
                            SendSQSMessage("serverless-pizza-finish", json).Wait();
                            break;
                        case "finish":
                            SendSQSMessage("serverless-pizza-deliver", json).Wait();
                            break;
                        case "deliver":
                            break;
                    }
                }
            }
        }
    }
}