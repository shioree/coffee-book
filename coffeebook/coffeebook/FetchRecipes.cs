using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace coffeebook
{
    public static class FetchRecipes
    {
        [FunctionName("FetchRecipes")]
        public static async Task<IEnumerable<Recipe>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("���V�s�擾���������s���܂�");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("cosmosdb-connection-string");
            var client = new CosmosClient(connectionString);
            var container = client.GetContainer("coffeebook-db", "Recipes");

            var query = container.GetItemQueryIterator<Recipe>(new QueryDefinition(
                "select * from r where r.userId = @userId")
                .WithParameter("@userId", "�e�X�g���[�U"));
            List<Recipe> results = new List<Recipe>();
            while (query.HasMoreResults)
            {                
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}