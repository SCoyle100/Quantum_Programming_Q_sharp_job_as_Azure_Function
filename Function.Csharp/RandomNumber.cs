using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Quantum.Simulation.Simulators;
using Service.Qsharp;

namespace Company.Function
{
    public static class RandomNumber
    {
        [FunctionName("RandomNumber")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string maxValueStr = req.Query["maxValue"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            maxValueStr = maxValueStr ?? data?.maxValue;

            int maxValue;

            if (!int.TryParse(maxValueStr, out maxValue))
            {
                maxValue = 100;
            }

            using var sim = new QuantumSimulator();

            int randomNumber = (int)await SampleRandomNumberInRange.Run(sim, maxValue);

            string responseMessage = $"{randomNumber}";

            return new OkObjectResult(responseMessage);
        }
    }
}

