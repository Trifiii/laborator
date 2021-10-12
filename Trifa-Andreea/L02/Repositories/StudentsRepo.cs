using System;
using System.Collections.Generic;
using Models;

namespace Repositories
{
    public static class StudentsRepo
    {
        public static List<Student> Students = new List<Student>(){
            new Student() { Id = 1, Name = "Georgiana", Faculty = "AC", Year = 2, ParentInitial = "I" },
            new Student() { Id = 2, Name = "Marius", Faculty = "ETC", Year = 1, ParentInitial = "A" },
            new Student() { Id = 3, Name = "Dan", Faculty = "AC", Year = 4, ParentInitial = "M" }
        };
    }
}