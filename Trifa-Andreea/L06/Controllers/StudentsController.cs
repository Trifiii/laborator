using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace L06.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentEntity>> Get()
        {
            return await _studentRepository.GetAllStudents();
        }
        
        [HttpGet("{PartitionKey}/{RowKey}")]
        public async Task<StudentEntity> Get(string PartitionKey, string RowKey)
        {
            return await _studentRepository.GetStudent(PartitionKey, RowKey);
        }

        [HttpPost]
        public async Task Post([FromBody] StudentEntity student)
        {
            try
            {
                await _studentRepository.CreateStudent(student);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        
        [HttpPut("{PartitionKey}/{RowKey}")]
        public  async Task Update(string PartitionKey, string RowKey, [FromBody] StudentEntity student)
        {
            try
            {
                await _studentRepository.Modify(PartitionKey, RowKey, student);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        
        [HttpDelete("{PartitionKey}/{RowKey}")]
        public async Task Delete(string PartitionKey, string RowKey)
        {
            try
            {
                await _studentRepository.Delete(PartitionKey, RowKey);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}