using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace router
{
    public class Function
    {
        // No arguments; credentials and region are pulled from environment variables auto-populated by Lambda
        private AmazonLambdaClient _client = new AmazonLambdaClient();

        private void Invoke(string functionName, DynamoDBEvent.DynamodbStreamRecord record)
        {
            // TODO: figure out why Lambda invocation doesn't work
            var request = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = "Event",           // Execute Lambda asynchronously
                Payload = record.ToString(),
            };

            var response = _client.InvokeAsync(request);
            LambdaLogger.Log(response.ToString());
            LambdaLogger.Log($"Invoked {functionName} Lambda");
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            LambdaLogger.Log($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                var events = record.Dynamodb.NewImage["events"].L;

                if (events.Count == 0)
                {
                    Invoke("ServerlessPizzaPrep", record);
                }
                else
                {
                    switch (events.Last().M["type"].S)
                    {
                        case "prep":
                            Invoke("ServerlessPizzaCook", record);
                            break;
                        case "cook":
                            Invoke("ServerlessPizzaFinish", record);
                            break;
                        case "finish":
                            string id = record.Dynamodb.NewImage["id"].S;
                            LambdaLogger.Log($"Order {id} complete.");
                            break;
                    }
                }
            }

            LambdaLogger.Log("Stream processing complete.");
        }
    }
}