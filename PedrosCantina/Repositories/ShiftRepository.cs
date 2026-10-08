using Microsoft.Data.SqlClient;
using PedrosCantina.Models;

namespace PedrosCantina.Repositories
{
    public class ShiftRepository
    {
        private const int MaxEmployeesOnShift = 3;
        private const int MinEmployeesOnCompleteShift = 2;

        public Shift? GetById(int shiftId)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT ShiftId, ShiftDate, StartTime, EndTime
                  FROM Shift
                  WHERE ShiftId = @ShiftId",
                connection);

            command.Parameters.AddWithValue("@ShiftId", shiftId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadShift(reader);
            }

            return null;
        }

        public List<Shift> GetByDate(DateTime date)
        {
            var shifts = new List<Shift>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT ShiftId, ShiftDate, StartTime, EndTime
                  FROM Shift
                  WHERE ShiftDate = @ShiftDate
                  ORDER BY StartTime",
                connection);

            command.Parameters.AddWithValue("@ShiftDate", date.Date);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                shifts.Add(ReadShift(reader));
            }

            return shifts;
        }

        public List<Employee> GetEmployeesOnShift(int shiftId)
        {
            var employees = new List<Employee>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT e.EmployeeId, e.Name, e.Phone, e.Email, e.IsManager
                  FROM ShiftAssignment sa
                  INNER JOIN Employee e ON sa.EmployeeId = e.EmployeeId
                  WHERE sa.ShiftId = @ShiftId
                  ORDER BY e.Name",
                connection);

            command.Parameters.AddWithValue("@ShiftId", shiftId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    EmployeeId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Phone = reader.GetString(2),
                    Email = reader.GetString(3),
                    IsManager = reader.GetBoolean(4)
                });
            }

            return employees;
        }

        public List<MonthlyPlanRow> GetMonthlyPlan(int year, int month)
        {
            var rows = new List<MonthlyPlanRow>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT s.ShiftDate, s.StartTime, s.EndTime, e.Name, e.IsManager
                  FROM Shift s
                  INNER JOIN ShiftAssignment sa ON s.ShiftId = sa.ShiftId
                  INNER JOIN Employee e ON sa.EmployeeId = e.EmployeeId
                  WHERE YEAR(s.ShiftDate) = @Year
                    AND MONTH(s.ShiftDate) = @Month
                  ORDER BY s.ShiftDate, s.StartTime, e.Name",
                connection);

            command.Parameters.AddWithValue("@Year", year);
            command.Parameters.AddWithValue("@Month", month);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new MonthlyPlanRow
                {
                    Date = reader.GetDateTime(0),
                    StartTime = reader.GetTimeSpan(1),
                    EndTime = reader.GetTimeSpan(2),
                    EmployeeName = reader.GetString(3),
                    IsManager = reader.GetBoolean(4)
                });
            }

            return rows;
        }

        public List<WorkloadRow> GetMonthlyWorkload(int year, int month)
        {
            var rows = new List<WorkloadRow>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT e.Name,
                         COUNT(*) AS ShiftCount,
                         SUM(DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0) AS Hours
                  FROM Employee e
                  INNER JOIN ShiftAssignment sa ON e.EmployeeId = sa.EmployeeId
                  INNER JOIN Shift s ON sa.ShiftId = s.ShiftId
                  WHERE YEAR(s.ShiftDate) = @Year
                    AND MONTH(s.ShiftDate) = @Month
                  GROUP BY e.Name
                  ORDER BY e.Name",
                connection);

            command.Parameters.AddWithValue("@Year", year);
            command.Parameters.AddWithValue("@Month", month);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new WorkloadRow
                {
                    EmployeeName = reader.GetString(0),
                    Month = month,
                    ShiftCount = reader.GetInt32(1),
                    Hours = Convert.ToDecimal(reader.GetValue(2))
                });
            }

            return rows;
        }

        public List<WorkloadRow> GetYearlyWorkload(int year)
        {
            var rows = new List<WorkloadRow>();

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"SELECT e.Name,
                         MONTH(s.ShiftDate) AS MonthNumber,
                         COUNT(*) AS ShiftCount,
                         SUM(DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0) AS Hours
                  FROM Employee e
                  INNER JOIN ShiftAssignment sa ON e.EmployeeId = sa.EmployeeId
                  INNER JOIN Shift s ON sa.ShiftId = s.ShiftId
                  WHERE YEAR(s.ShiftDate) = @Year
                  GROUP BY e.Name, MONTH(s.ShiftDate)
                  ORDER BY e.Name, MONTH(s.ShiftDate)",
                connection);

            command.Parameters.AddWithValue("@Year", year);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new WorkloadRow
                {
                    EmployeeName = reader.GetString(0),
                    Month = reader.GetInt32(1),
                    ShiftCount = reader.GetInt32(2),
                    Hours = Convert.ToDecimal(reader.GetValue(3))
                });
            }

            return rows;
        }

        public int CreateShift(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"INSERT INTO Shift (ShiftDate, StartTime, EndTime)
                  VALUES (@ShiftDate, @StartTime, @EndTime);
                  SELECT CAST(SCOPE_IDENTITY() AS int);",
                connection);

            command.Parameters.AddWithValue("@ShiftDate", date.Date);
            command.Parameters.AddWithValue("@StartTime", startTime);
            command.Parameters.AddWithValue("@EndTime", endTime);

            return (int)command.ExecuteScalar()!;
        }

        public string AssignEmployee(int shiftId, int employeeId)
        {
            var employees = GetEmployeesOnShift(shiftId);

            if (employees.Any(e => e.EmployeeId == employeeId))
            {
                return "Medarbejderen er allerede tildelt denne vagt.";
            }

            if (employees.Count >= MaxEmployeesOnShift)
            {
                return "Vagten kan højst have 3 medarbejdere.";
            }

            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"INSERT INTO ShiftAssignment (ShiftId, EmployeeId)
                  VALUES (@ShiftId, @EmployeeId)",
                connection);

            command.Parameters.AddWithValue("@ShiftId", shiftId);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);
            command.ExecuteNonQuery();

            return GetStaffingMessage(shiftId);
        }

        public bool RemoveEmployee(int shiftId, int employeeId)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                @"DELETE FROM ShiftAssignment
                  WHERE ShiftId = @ShiftId
                    AND EmployeeId = @EmployeeId",
                connection);

            command.Parameters.AddWithValue("@ShiftId", shiftId);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);

            return command.ExecuteNonQuery() == 1;
        }

        public string GetStaffingMessage(int shiftId)
        {
            var employees = GetEmployeesOnShift(shiftId);
            int count = employees.Count;
            bool hasManager = employees.Any(e => e.IsManager);

            if (count == 0)
            {
                return "Vagten er oprettet, men har endnu ingen medarbejdere.";
            }

            if (count < MinEmployeesOnCompleteShift)
            {
                return $"Vagten har {count} medarbejder(e). En færdig vagt skal have 2-3 personer og mindst én leder.";
            }

            if (!hasManager)
            {
                return $"Vagten har {count} medarbejdere, men mangler en leder (Pedro eller Jacoby).";
            }

            return $"Vagten er færdigbemandet med {count} medarbejdere, heraf mindst én leder.";
        }

        private static Shift ReadShift(SqlDataReader reader)
        {
            return new Shift
            {
                ShiftId = reader.GetInt32(0),
                Date = reader.GetDateTime(1),
                StartTime = reader.GetTimeSpan(2),
                EndTime = reader.GetTimeSpan(3)
            };
        }
    }
}
