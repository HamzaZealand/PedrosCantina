using Microsoft.Data.SqlClient;
using PedrosCantina.Models;

namespace PedrosCantina.Repositories
{
    public class EmployeeRepository
    {
        public List<Employee> GetAll()
        {
            var employees = new List<Employee>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT EmployeeId, Name, Phone, Email, IsManager
                  FROM Employee
                  ORDER BY Name",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(ReadEmployee(reader));
            }

            return employees;
        }

        public Employee? GetById(int id)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT EmployeeId, Name, Phone, Email, IsManager
                  FROM Employee
                  WHERE EmployeeId = @EmployeeId",
                connection);

            command.Parameters.AddWithValue("@EmployeeId", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadEmployee(reader);
            }

            return null;
        }

        public List<Employee> GetByName(string name)
        {
            var employees = new List<Employee>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT EmployeeId, Name, Phone, Email, IsManager
                  FROM Employee
                  WHERE Name LIKE @Name
                  ORDER BY Name",
                connection);

            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(ReadEmployee(reader));
            }

            return employees;
        }

        public int Create(Employee employee)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"INSERT INTO Employee (Name, Phone, Email, IsManager)
                  VALUES (@Name, @Phone, @Email, @IsManager);
                  SELECT CAST(SCOPE_IDENTITY() AS int);",
                connection);

            AddEmployeeParameters(command, employee);

            return (int)command.ExecuteScalar()!;
        }

        public bool Update(Employee employee)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"UPDATE Employee
                  SET Name = @Name,
                      Phone = @Phone,
                      Email = @Email,
                      IsManager = @IsManager
                  WHERE EmployeeId = @EmployeeId",
                connection);

            command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
            AddEmployeeParameters(command, employee);

            return command.ExecuteNonQuery() == 1;
        }

        public bool Delete(int id)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"DELETE FROM Employee
                  WHERE EmployeeId = @EmployeeId",
                connection);

            command.Parameters.AddWithValue("@EmployeeId", id);

            return command.ExecuteNonQuery() == 1;
        }

        private static void AddEmployeeParameters(SqlCommand command, Employee employee)
        {
            command.Parameters.AddWithValue("@Name", employee.Name);
            command.Parameters.AddWithValue("@Phone", employee.Phone);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@IsManager", employee.IsManager);
        }

        private static Employee ReadEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Phone = reader.GetString(2),
                Email = reader.GetString(3),
                IsManager = reader.GetBoolean(4)
            };
        }
    }
}
