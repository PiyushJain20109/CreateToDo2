using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreateToDo2
{
    public static class DurableFunctionDemo
    {
        [Function(nameof(DurableFunctionDemo))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var order = context.GetInput<Order>();

            await context.CallActivityAsync("ValidateOrder", order);
            await context.CallActivityAsync("ChargeCustomer", order);
            await context.CallActivityAsync("CheckInventory", order);
            await context.CallActivityAsync("ShipOrder", order);
            await context.CallActivityAsync("SendConfirmationEmail", order);
        }

        //[Function(nameof(SayHello))]
        //public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        //{
        //    ILogger logger = executionContext.GetLogger("SayHello");
        //    logger.LogInformation("Saying hello to {name}.", name);
        //    return $"Hello {name}!";
        //}

        [Function(nameof(ValidateOrder))]
        public static void ValidateOrder([ActivityTrigger] Order order, FunctionContext executionContext, ILogger log)
        {
            log.LogInformation($"Validating order {order.OrderId}.");
            // Add validation logic here.
        }
        [Function(nameof(ChargeCustomer))]
        public static void ChargeCustomer([ActivityTrigger] Order order, FunctionContext executionContext, ILogger log)
        {
            log.LogInformation($"Charging customer {order.CustomerId} for order {order.OrderId}.");
            // Add charging logic here.
        }
        [Function(nameof(CheckInventory))]
        public static void CheckInventory([ActivityTrigger] Order order, FunctionContext executionContext, ILogger log)
        {
            log.LogInformation($"Checking inventory for order {order.OrderId}.");
            // Add inventory check logic here.
        }
        [Function(nameof(ShipOrder))]
        public static void ShipOrder([ActivityTrigger] Order order, FunctionContext executionContext, ILogger log)
        {
            log.LogInformation($"Shipping order {order.OrderId}.");
            // Add shipping logic here.
        }
        [Function(nameof(SendConfirmationEmail))]
        public static void SendConfirmationEmail([ActivityTrigger] Order order, FunctionContext executionContext, ILogger log)
        {
            log.LogInformation($"Sending confirmation email for order {order.OrderId}.");
            // Add email sending logic here.
        }


        [Function("DurableFunctionDemo_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("DurableFunctionDemo_HttpStart");

            //// Function input comes from the request content.
            //string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            //    nameof(DurableFunctionDemo));

            //logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            //// Returns an HTTP 202 response with an instance management payload.
            //// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            //return await client.CreateCheckStatusResponseAsync(req, instanceId);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(DurableFunctionDemo), order);
            logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return await client.CreateCheckStatusResponseAsync(req, instanceId); 
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<string> ItemIds { get; set; }
    }
}
