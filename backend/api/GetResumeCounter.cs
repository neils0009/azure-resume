using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace Company.Function;

public class GetResumeCounter
{
    private readonly ILogger<GetResumeCounter> _logger;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public GetResumeCounter(ILogger<GetResumeCounter> logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer("AzureResume", "Counter");
    }

    [Function("GetResumeCounter")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var id = "1";
        var partitionKey = new PartitionKey("1");

        var response = await _container.ReadItemAsync<Counter>(id, partitionKey);
        var counter = response.Resource;

        counter.Count += 1;

        await _container.ReplaceItemAsync(counter, counter.Id, partitionKey);

        var jsonToReturn = JsonConvert.SerializeObject(counter);

        var res = req.CreateResponse(HttpStatusCode.OK);
        res.Headers.Add("Content-Type", "application/json");
        await res.WriteStringAsync(jsonToReturn);

        return res;
    }
}
