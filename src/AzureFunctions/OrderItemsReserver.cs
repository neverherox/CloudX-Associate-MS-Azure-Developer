using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public static class OrderItemsReserver
    {
        [FunctionName("order-items-reserver")]
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

            var orderDetails = new
            {
                id = Guid.NewGuid().ToString(),
                Items = data.Items,
                ShippingAddress = data.ShippingAddress,
                Price = data.Price,
                OrderID = Guid.NewGuid().ToString()
            };

            var cosmosClient = new CosmosClient(
                "",
                ""
                );
            var database = cosmosClient.GetDatabase("e-shop-cosmos-database");
            var container = database.GetContainer("e-shop-cosmos-container");
            await container.CreateItemAsync(orderDetails, new PartitionKey(orderDetails.OrderID));
            return new OkObjectResult(responseMessage);
        }
    }
}
