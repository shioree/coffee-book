using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace coffeebook
{
    public static class Login
    {
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "coffeebook-db",
                collectionName: "Session",
                ConnectionStringSetting = "cosmosdb-connection-string")] IAsyncCollector<Session> sessionOut,
            ILogger log,
            ExecutionContext context)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody, typeof(UserInfo));

            string message = data.Id + "�̃��O�C����F�؂��܂�";
            log.LogInformation(message);

            // �ݒ�l�ǂݍ���
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // �R���e�i���擾����
            var connectionString = config.GetConnectionString("cosmosdb-connection-string");
            var client = new CosmosClient(connectionString);
            var usersContainer = client.GetContainer("coffeebook-db", "Users");

            var query = usersContainer.GetItemQueryIterator<User>(new QueryDefinition(
                "select * from r where r.userId = @userId")
                .WithParameter("@userId", (string)data.Id));

            List<User> results = new List<User>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            if (results.Count() != 0)
            {
                // �p�X���[�h�̊m�F
                byte[] hashedPassword = UserService.GenerateHashedValue(data.Password, results[0].Salt);

                if (!Enumerable.SequenceEqual(results[0].HashedPassword, hashedPassword))
                {
                    return new BadRequestResult(); // �p�X���[�h�F�؎��s
                }

                // �Z�b�V�������̍쐬
                string sessionId = UserService.GenerateSessionId();
                var session = new Session
                {
                    UserId = data.Id,
                    SessionId = sessionId
                };
                await sessionOut.AddAsync(session);

                // Cookie�փZ�b�V���������i�[
                //var cookieOption = new CookieOptions()
                //{
                //    Secure = true
                //};
                //req.HttpContext.Response.Cookies.Append("sessionId", sessionId, cookieOption);
                req.HttpContext.Response.Cookies.Append("sessionId", sessionId);

                return new OkResult();
            }
            else
            {
                return new BadRequestResult(); // ���[�U�[���Ȃ�
            }
        }
    }
}
