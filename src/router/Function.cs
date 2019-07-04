using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace router
{
    public class Function
    {
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            LambdaLogger.Log($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                LambdaLogger.Log($"Event ID: {record.EventID}");
                LambdaLogger.Log($"Event Name: {record.EventName}");

                string streamRecordJson = SerializeStreamRecord(record.Dynamodb);
                LambdaLogger.Log($"DynamoDB Record:");
                LambdaLogger.Log(streamRecordJson );
            }

            LambdaLogger.Log("Stream processing complete.");
        }

        private string SerializeStreamRecord(StreamRecord streamRecord)
        {
            using (var writer = new StringWriter())
            {
                _jsonSerializer.Serialize(writer, streamRecord);
                return writer.ToString();
            }
        }
    }
}