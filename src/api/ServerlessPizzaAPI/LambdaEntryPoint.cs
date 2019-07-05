using Microsoft.AspNetCore.Hosting;
using Amazon.Lambda.AspNetCoreServer;

namespace ServerlessPizzaAPI
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>()
                .UseApiGateway();
        }
    }
}