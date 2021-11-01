using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace L04
{
    class Program
    {

        private static CloudTableClient tableClient;
        private static CloudTable studentsTable;

        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }

        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=laborator4datc"
                + ";AccountKey=KwBBQC+H6hWRr+C2desFRy4gLNOHj3LPwFLUwzZwbqh6StQoFVfRM+8G1vLU4oiedszDb1lOUTdu8aZw2OvQOQ=="
                + ";EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            TableClient = account.CreateCloudTableClient();
            StudentsTable = TableClient.GetTableReference("studenti");

            await StudentsTable.CreateIfNotExistsAsync();
            await AddNewStudent();
            await GetAllStudents();

            var get_univ = ReadValue("University: ");
            var get_cnp = ReadValue("CNP= ");
            await GetStudent(get_univ, get_cnp);

            var up_univ = ReadValue("University: ");
            var up_cnp = ReadValue("CNP= ");
            await UpdateStudent(up_univ, up_cnp);

            var del_univ = ReadValue("University: ");
            var del_cnp = ReadValue("CNP= ");
            await DeleteStudent(del_univ, del_cnp);
        }

        private static async Task AddNewStudent()
        {
            var student = new StudentEntity("UPT", "2940521016653");
            student.FirstName = "Anthony";
            student.LastName = "Lark";
            student.Email = "anthony23@gmail.com";
            student.Year = 3;
            student.PhoneNumber = "0799883113";
            student.Faculty = "AC";

            var insertOperation = TableOperation.Insert(student);

            await StudentsTable.ExecuteAsync(insertOperation);
        }

        private static async Task GetAllStudents()
        {
            Console.WriteLine("Universitate\tCNP\tNume\tEmail\tNumar telefon\tAn");
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await StudentsTable.ExecuteQuerySegmentedAsync(query);
                token = resultSegment.ContinuationToken;

                foreach (StudentEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}\t{1}\t{2} {3}\t{4}\t{5}\t{6}", entity.PartitionKey, entity.RowKey, entity.FirstName,
                                        entity.LastName, entity.Email, entity.Year, entity.PhoneNumber, entity.Faculty);
                }
            }while (token != null);
        }

        private static async Task GetStudent(string partitionKey, string rowKey)
        {
            var retrieveOperation = TabbleOperation.Retrieve(partitionKey, rowKey);
            await StudentsTable.ExecuteAsync(retrieveOperation);

        }

        private static async Task UpdateStudent(string partitionKey, string rowKey)
        {
            var updateOperation = TableOperation.InsertOrReplace(partitionKey, rowKey);
            await StudentsTable.ExecuteAsync(updateOperation);
        }

        private static async Task DeleteStudent(string partitionKey, string rowKey)
        {
            var deleteOperation = TableOperation.Delete(partitionKey, rowKey);
            await StudentsTable.ExecuteAsync(deleteOperation);
        }
    }
}
