using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos;

namespace coffeebook
{
    /// <summary>
    /// ���O�A�E�g
    /// </summary>
    /// <remarks>���O�A�E�g�ɐ���������A�Z�b�V����ID��Cookie����폜����</remarks>
    /// <returns>���O�A�E�g�̐���</returns>
    public static class Logout
    {
        [FunctionName("Logout")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            // �ݒ�l�ǂݍ���
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // �Z�b�V����ID���烆�[�UID���擾����
            req.HttpContext.Request.Cookies.TryGetValue("sessionId", out string sessionId);

            // �R���e�i���擾����
            var connectionString = config.GetConnectionString(Consts.COSMOSDB_CONNECTION_STRING);
            var client = new CosmosClient(connectionString);
            var container = client.GetContainer(Consts.COFFEEBOOK_DB, Consts.SESSION_CONTAINER);

            var query = container.GetItemQueryIterator<Session>(new QueryDefinition(
                "select * from r where r.sessionId = @sessionId")
                .WithParameter("@sessionId", sessionId));

            List<Session> results = new List<Session>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            string documentId = results[0].Id;
            string partitionKey = results[0].UserId;

            log.LogInformation(partitionKey + "�����O�A�E�g���܂�");

            ItemResponse<Session> deleteSessionResponse = await container.DeleteItemAsync<Session>(documentId, new PartitionKey(partitionKey));
            req.HttpContext.Response.Cookies.Delete(UserConst.sessionId);

            return new OkResult();
        }
    }
}
