using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStudentRepository{
    Task<List<StudentEntity>> GetAllStudents();
    
    Task<StudentEntity> GetStudent(string partKey, string rowKey);

    Task Modify(string partKey, string rowKey, StudentEntity student);

    Task Delete(string partKey, string rowKey);
    Task CreateStudent(StudentEntity student);
}