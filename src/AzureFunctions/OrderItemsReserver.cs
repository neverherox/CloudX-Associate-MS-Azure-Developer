using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public static class OrderItemsReserver
    {
        [FunctionName("order-items-reserver")]
        public static void Run(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")]
            string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
