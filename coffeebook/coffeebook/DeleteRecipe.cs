using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;

namespace coffeebook
{
    public static class DeleteRecipe
    {
        [FunctionName("DeleteRecipe")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                // �ݒ�l�ǂݍ���
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // �R���e�i���擾����
                var connectionString = config.GetConnectionString(Consts.COSMOSDB_CONNECTION_STRING);
                var client = new CosmosClient(connectionString);
                var container = client.GetContainer(Consts.COFFEEBOOK_DB, Consts.RECIPES_CONTAINER);

                // �Z�b�V����ID���烆�[�UID���擾����
                req.HttpContext.Request.Cookies.TryGetValue("sessionId", out string sessionId);
                string partitionKey = await UserService.GetUserIdFromSessionContainer(connectionString, sessionId);

                // �폜�Ώۂ�id���擾����
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic deleteModel = JsonConvert.DeserializeObject(requestBody, typeof(DeleteModel));

                log.LogInformation(partitionKey + "�̃��V�sID:" + (string)deleteModel.Id + "���폜���܂�");

                ItemResponse<Recipe> deleteSessionResponse = await container.DeleteItemAsync<Recipe>((string)deleteModel.Id, new PartitionKey(partitionKey));

                return new OkResult();
            }
            catch(Exception ex)
            {
                log.LogInformation(ex.ToString());
                return new BadRequestResult();
            }
        }
    }
}