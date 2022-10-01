using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public static class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var blobServiceClient = new BlobServiceClient(
                "DefaultEndpointsProtocol=https;AccountName=eshopstor;AccountKey=piZ7DZl+kQO3pzttDTF5jtLHDJ8Nerh+Sdw69A4Jh6gDshzcn/wdtalVfct2m1ubthlNSUhy6Nhm+AStxyc7hQ==;EndpointSuffix=core.windows.net"
                );
            var containerClient = blobServiceClient.GetBlobContainerClient("e-shop-container");
            var blobName = "order-details." + Guid.NewGuid() + ".json";
            await using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)))
            {
                await containerClient.UploadBlobAsync(blobName, ms);
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
