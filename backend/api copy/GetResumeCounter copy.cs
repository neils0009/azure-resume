using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace Company.Function;

public class GetResumeCounter
{
    private readonly ILogger<GetResumeCounter> _logger;

    public GetResumeCounter(ILogger<GetResumeCounter> logger)
    {
        _logger = logger;
    }

    [Function("GetResumeCounter")]
    public HttpResponseMessage Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
        //[CosmosDB(databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString", Id = "1", PartitionKey = "1")] Counter counter,
        //[CosmosDB(databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString", Id = "1", PartitionKey = "1")] out Counter updatedCounter
        [CosmosDBInput(databaseName: "AzureResume", containerName: "Counter", Connection ="AzureResumeConnectionString", Id = "1", PartitionKey = "1")] Counter counter,
        //[CosmosDBInput(databaseName: "AzureResume", containerName: "Counter", Connection ="AzureResumeConnectionString", Id = "1", PartitionKey = "1")] out Counter updatedCounter,
        [CosmosDBTrigger(databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString" )] out Counter updatedCounter
        )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        updatedCounter = counter;
        updatedCounter.Count += 1;

        var jsonToReturn = JsonConvert.SerializeObject(counter);
        
        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
        };
    }
}