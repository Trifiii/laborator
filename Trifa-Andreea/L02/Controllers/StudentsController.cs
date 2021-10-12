using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Repositories;

namespace L02.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        [HttpGet("{id}")]
        public Student GetStudent(int id)
        {
            return StudentsRepo.Students.FirstOrDefault(s => s.Id == id);
        }

         [HttpGet]
        public IEnumerable<Student> Get()
        {
            return StudentsRepo.Students;
        }

        [HttpPost]
        public void Post([FromBody] Student student)
        {
            StudentsRepo.Students.Add(student);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Student student)
        {
            StudentsRepo.Students[StudentsRepo.Students.IndexOf(StudentsRepo.Students.FirstOrDefault(s => s.Id == id))] = student;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            StudentsRepo.Students.RemoveAt(StudentsRepo.Students.IndexOf(StudentsRepo.Students.FirstOrDefault(s => s.Id == id)));
        }
    }
}