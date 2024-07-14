using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreateToDo2
{
    public class CreateToDo
    {
        [Function("CreateToDo")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post", Route = "todo")] HttpRequest req)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoItem>(requestBody);

            // Add your logic to save the ToDo item to a database

            return new OkObjectResult(input);
        }
    }
    public class ToDoItem
    {
        public string Id { get; set; }
        public string Task { get; set; }
        public bool IsCompleted { get; set; }
    }
}
