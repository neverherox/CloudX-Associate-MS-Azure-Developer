using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public static class OrderItemsReserver
    {
        [FunctionName("order-items-reserver")]
        public static async Task Run(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")]
            string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var blobServiceClient = new BlobServiceClient(
                Environment.GetEnvironmentVariable("BlobConnectionString")
            );
            var containerClient = blobServiceClient.GetBlobContainerClient("e-shop-orders");
            var blobName = "order-details." + Guid.NewGuid() + ".json";
            await using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(myQueueItem));
            await containerClient.UploadBlobAsync(blobName, ms);
        }
    }
}
