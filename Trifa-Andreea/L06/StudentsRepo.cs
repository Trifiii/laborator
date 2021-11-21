using System.Security.AccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;
using Newtonsoft.Json;
using Azure.Storage.Queues;


namespace L06
{
    public class StudentsRepository : IStudentRepository
    {
        private CloudTableClient _tableClient;

        private CloudTable _studentsTable;

        private string _connectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=laborator4datc"
                + ";AccountKey=KwBBQC+H6hWRr+C2desFRy4gLNOHj3LPwFLUwzZwbqh6StQoFVfRM+8G1vLU4oiedszDb1lOUTdu8aZw2OvQOQ=="
                + ";EndpointSuffix=core.windows.net";

        public StudentsRepository(IConfiguration configuration)
        {
            _connectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=laborator4datc"
                + ";AccountKey=KwBBQC+H6hWRr+C2desFRy4gLNOHj3LPwFLUwzZwbqh6StQoFVfRM+8G1vLU4oiedszDb1lOUTdu8aZw2OvQOQ=="
                + ";EndpointSuffix=core.windows.net";

            Task.Run(async () => {await InitializeTable();})
                .GetAwaiter()
                .GetResult();
        }

        public async Task CreateStudent(StudentEntity student)
        {
            var jsonStudent = JsonConvert.SerializeObject(student);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonStudent);
            var base64String = System.Convert.ToBase64String(plainTextBytes);
           
            QueueClient queueClient = new QueueClient(
                _connectionString,
                "students-queue"
            );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String);
        }

        public async Task<List<StudentEntity>> GetAllStudents()
        {
            var students = new List<StudentEntity>();

            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                students.AddRange(resultSegment.Results);
            }while (token != null);

            return students;

        }
        
        public async Task<StudentEntity> GetStudent(string partKey, string rowKey)
        {
            var students = new StudentEntity();

            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partKey)).Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                students = resultSegment.Results[0];
            }while (token != null);

            return students;

        }

        public async Task Modify(string partKey, string rowKey, StudentEntity student)
        {
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partKey)).Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                var del = TableOperation.InsertOrReplace(student);
                await _studentsTable.ExecuteAsync(del);
            }while (token != null);
        }
        
        public async Task Delete(string partKey, string rowKey)
        {
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partKey)).Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                
                var del = TableOperation.Delete(resultSegment.Results[0]);
                await _studentsTable.ExecuteAsync(del);
            }while (token != null);
        }

        private async Task InitializeTable()
        {
            _connectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=laborator4datc"
                + ";AccountKey=KwBBQC+H6hWRr+C2desFRy4gLNOHj3LPwFLUwzZwbqh6StQoFVfRM+8G1vLU4oiedszDb1lOUTdu8aZw2OvQOQ=="
                + ";EndpointSuffix=core.windows.net";
            
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();

            _studentsTable = _tableClient.GetTableReference("studenti");

            await _studentsTable.CreateIfNotExistsAsync();
        }
    }

}