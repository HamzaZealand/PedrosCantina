using Microsoft.Data.SqlClient;
using PedrosCantina.Models;
using PedrosCantina.Repositories;

namespace PedrosCantina
{
    internal class Program
    {
        private static readonly EmployeeRepository employeeRepository = new();
        private static readonly ShiftRepository shiftRepository = new();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool running = true;
            while (running)
            {
                PrintMenu();
                string choice = ReadText("Vælg et menupunkt: ");

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowAllEmployees();
                            break;
                        case "2":
                            FindEmployee();
                            break;
                        case "3":
                            CreateEmployee();
                            break;
                        case "4":
                            UpdateEmployee();
                            break;
                        case "5":
                            DeleteEmployee();
                            break;
                        case "6":
                            ShowMonthlyPlan();
                            break;
                        case "7":
                            ShowMonthlyWorkload();
                            break;
                        case "8":
                            ShowContactInformation();
                            break;
                        case "9":
                            ShowYearlyWorkload();
                            break;
                        case "10":
                            CreateShift();
                            break;
                        case "11":
                            AssignEmployeeToShift();
                            break;
                        case "12":
                            RemoveEmployeeFromShift();
                            break;
                        case "0":
                            running = false;
                            Console.WriteLine("Farvel.");
                            break;
                        default:
                            Console.WriteLine("Ugyldigt valg. Prøv igen.");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Der opstod en databasefejl.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    Console.WriteLine("Tjek at SQL Server kører, at databasen PedrosCantina er oprettet,");
                    Console.WriteLine("og at connection string i Database.cs passer til din server.");
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Tryk på Enter for at fortsætte...");
                    Console.ReadLine();
                }
            }
        }

        static void PrintMenu()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }
            Console.WriteLine("=========================");
            Console.WriteLine("     PEDROS CANTINA");
            Console.WriteLine("=========================");
            Console.WriteLine();
            Console.WriteLine("1. Vis alle medarbejdere");
            Console.WriteLine("2. Find medarbejder");
            Console.WriteLine("3. Opret medarbejder");
            Console.WriteLine("4. Opdater medarbejder");
            Console.WriteLine("5. Slet medarbejder");
            Console.WriteLine();
            Console.WriteLine("6. Vis månedsplan");
            Console.WriteLine("7. Vis månedlig belastning");
            Console.WriteLine("8. Find kontaktinformation");
            Console.WriteLine("9. Vis årlig belastning");
            Console.WriteLine();
            Console.WriteLine("10. Opret vagt");
            Console.WriteLine("11. Tildel medarbejder til vagt");
            Console.WriteLine("12. Fjern medarbejder fra vagt");
            Console.WriteLine();
            Console.WriteLine("0. Afslut");
            Console.WriteLine();
        }

        static void ShowAllEmployees()
        {
            var employees = employeeRepository.GetAll();
            PrintEmployees(employees);
        }

        static void FindEmployee()
        {
            int id = ReadInt("Indtast medarbejder-id: ");
            var employee = employeeRepository.GetById(id);

            if (employee == null)
            {
                Console.WriteLine("Medarbejderen blev ikke fundet.");
                return;
            }

            PrintEmployees(new List<Employee> { employee });
        }

        static void CreateEmployee()
        {
            var employee = ReadEmployeeData();
            int id = employeeRepository.Create(employee);
            Console.WriteLine($"Medarbejderen blev oprettet med id {id}.");
        }

        static void UpdateEmployee()
        {
            int id = ReadInt("Indtast id på medarbejderen, der skal opdateres: ");
            var existing = employeeRepository.GetById(id);

            if (existing == null)
            {
                Console.WriteLine("Medarbejderen blev ikke fundet.");
                return;
            }

            Console.WriteLine($"Nuværende: {existing.Name}, {existing.Phone}, {existing.Email}, leder: {YesNo(existing.IsManager)}");
            var updated = ReadEmployeeData();
            updated.EmployeeId = id;

            if (employeeRepository.Update(updated))
            {
                Console.WriteLine("Medarbejderen blev opdateret.");
            }
            else
            {
                Console.WriteLine("Opdateringen lykkedes ikke.");
            }
        }

        static void DeleteEmployee()
        {
            int id = ReadInt("Indtast id på medarbejderen, der skal slettes: ");
            var existing = employeeRepository.GetById(id);

            if (existing == null)
            {
                Console.WriteLine("Medarbejderen blev ikke fundet.");
                return;
            }

            Console.WriteLine($"Slet {existing.Name}? Skriv ja for at bekræfte.");
            string answer = ReadText("Bekræft: ");
            if (!answer.Equals("ja", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Sletning afbrudt.");
                return;
            }

            try
            {
                if (employeeRepository.Delete(id))
                {
                    Console.WriteLine("Medarbejderen blev slettet.");
                }
                else
                {
                    Console.WriteLine("Sletningen lykkedes ikke.");
                }
            }
            catch (SqlException)
            {
                Console.WriteLine("Medarbejderen kan ikke slettes, mens personen stadig er tildelt vagter.");
                Console.WriteLine("Fjern først medarbejderen fra alle vagter.");
            }
        }

        static void ShowMonthlyPlan()
        {
            int year = ReadInt("Indtast år (fx 2026): ");
            int month = ReadInt("Indtast måned (1-12): ");

            if (month < 1 || month > 12)
            {
                Console.WriteLine("Måneden skal være mellem 1 og 12.");
                return;
            }

            var rows = shiftRepository.GetMonthlyPlan(year, month);
            if (rows.Count == 0)
            {
                Console.WriteLine("Der blev ikke fundet nogen vagter i den måned.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Månedsplan {month:00}/{year}");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"{"Dato",-12} {"Start",-8} {"Slut",-8} {"Medarbejder",-22} {"Leder",-6}");
            Console.WriteLine(new string('-', 70));

            foreach (var row in rows)
            {
                Console.WriteLine(
                    $"{row.Date:dd-MM-yyyy}  {row.StartTime:hh\\:mm}    {row.EndTime:hh\\:mm}    {row.EmployeeName,-22} {YesNo(row.IsManager)}");
            }
        }

        static void ShowMonthlyWorkload()
        {
            int year = ReadInt("Indtast år (fx 2026): ");
            int month = ReadInt("Indtast måned (1-12): ");

            if (month < 1 || month > 12)
            {
                Console.WriteLine("Måneden skal være mellem 1 og 12.");
                return;
            }

            var rows = shiftRepository.GetMonthlyWorkload(year, month);
            if (rows.Count == 0)
            {
                Console.WriteLine("Der blev ikke fundet nogen vagter i den måned.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Månedlig belastning {month:00}/{year}");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"{"Medarbejder",-22} {"Vagter",-8} {"Timer",-8}");
            Console.WriteLine(new string('-', 50));

            foreach (var row in rows)
            {
                Console.WriteLine($"{row.EmployeeName,-22} {row.ShiftCount,-8} {row.Hours,5:0.0}");
            }
        }

        static void ShowContactInformation()
        {
            string search = ReadText("Søg på navn eller indtast medarbejder-id: ");

            List<Employee> employees;
            if (int.TryParse(search, out int id))
            {
                var employee = employeeRepository.GetById(id);
                employees = employee == null ? new List<Employee>() : new List<Employee> { employee };
            }
            else
            {
                employees = employeeRepository.GetByName(search);
            }

            if (employees.Count == 0)
            {
                Console.WriteLine("Ingen medarbejder blev fundet.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Kontaktinformation");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"{"Navn",-22} {"Telefon",-14} {"Email"}");
            Console.WriteLine(new string('-', 70));

            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.Name,-22} {employee.Phone,-14} {employee.Email}");
            }
        }

        static void ShowYearlyWorkload()
        {
            int year = ReadInt("Indtast år (fx 2026): ");
            var rows = shiftRepository.GetYearlyWorkload(year);

            if (rows.Count == 0)
            {
                Console.WriteLine("Der blev ikke fundet nogen vagter i det år.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Årlig belastning {year}");
            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"{"Medarbejder",-22} {"Måned",-8} {"Vagter",-8} {"Timer",-8}");
            Console.WriteLine(new string('-', 55));

            foreach (var row in rows)
            {
                Console.WriteLine($"{row.EmployeeName,-22} {row.Month,-8} {row.ShiftCount,-8} {row.Hours,5:0.0}");
            }
        }

        static void CreateShift()
        {
            DateTime date = ReadDate("Indtast dato (dd-MM-yyyy): ");

            Console.WriteLine("1. Formiddag  09:00-14:00");
            Console.WriteLine("2. Eftermiddag 14:00-19:00");
            int type = ReadInt("Vælg vagt: ");

            TimeSpan start;
            TimeSpan end;
            if (type == 1)
            {
                start = new TimeSpan(9, 0, 0);
                end = new TimeSpan(14, 0, 0);
            }
            else if (type == 2)
            {
                start = new TimeSpan(14, 0, 0);
                end = new TimeSpan(19, 0, 0);
            }
            else
            {
                Console.WriteLine("Ugyldigt valg. Vagten blev ikke oprettet.");
                return;
            }

            var existing = shiftRepository.GetByDate(date);
            if (existing.Any(s => s.StartTime == start && s.EndTime == end))
            {
                Console.WriteLine("Der findes allerede en vagt på det tidspunkt.");
                return;
            }

            int shiftId = shiftRepository.CreateShift(date, start, end);
            Console.WriteLine($"Vagten blev oprettet med id {shiftId}.");
            Console.WriteLine("En ny vagt har 0 medarbejdere, indtil du tildeler nogen.");
        }

        static void AssignEmployeeToShift()
        {
            if (!TryChooseShift(out int shiftId))
            {
                return;
            }

            ShowAllEmployees();
            int employeeId = ReadInt("Indtast medarbejder-id: ");

            if (employeeRepository.GetById(employeeId) == null)
            {
                Console.WriteLine("Medarbejderen blev ikke fundet.");
                return;
            }

            string message = shiftRepository.AssignEmployee(shiftId, employeeId);
            Console.WriteLine(message);
        }

        static void RemoveEmployeeFromShift()
        {
            if (!TryChooseShift(out int shiftId))
            {
                return;
            }

            var employees = shiftRepository.GetEmployeesOnShift(shiftId);
            if (employees.Count == 0)
            {
                Console.WriteLine("Der er ingen medarbejdere på vagten.");
                return;
            }

            PrintEmployees(employees);
            int employeeId = ReadInt("Indtast medarbejder-id, der skal fjernes: ");

            if (shiftRepository.RemoveEmployee(shiftId, employeeId))
            {
                Console.WriteLine("Medarbejderen blev fjernet fra vagten.");
                Console.WriteLine(shiftRepository.GetStaffingMessage(shiftId));
            }
            else
            {
                Console.WriteLine("Medarbejderen var ikke tildelt vagten.");
            }
        }

        static bool TryChooseShift(out int shiftId)
        {
            DateTime date = ReadDate("Indtast dato for vagten (dd-MM-yyyy): ");
            var shifts = shiftRepository.GetByDate(date);

            if (shifts.Count == 0)
            {
                Console.WriteLine("Der er ingen vagter på den dato.");
                shiftId = 0;
                return false;
            }

            Console.WriteLine();
            Console.WriteLine($"Vagter {date:dd-MM-yyyy}");
            foreach (var shift in shifts)
            {
                Console.WriteLine($"Id {shift.ShiftId}: {shift.StartTime:hh\\:mm}-{shift.EndTime:hh\\:mm}");
            }

            shiftId = ReadInt("Indtast vagt-id: ");
            bool shiftExists = false;
            foreach (var shift in shifts)
            {
                if (shift.ShiftId == shiftId)
                {
                    shiftExists = true;
                    break;
                }
            }

            if (!shiftExists)
            {
                Console.WriteLine("Vagten blev ikke fundet på den dato.");
                return false;
            }

            return true;
        }

        static Employee ReadEmployeeData()
        {
            return new Employee
            {
                Name = ReadText("Navn: "),
                Phone = ReadText("Telefon: "),
                Email = ReadText("Email: "),
                IsManager = ReadYesNo("Er medarbejderen leder? (ja/nej): ")
            };
        }

        static void PrintEmployees(List<Employee> employees)
        {
            if (employees.Count == 0)
            {
                Console.WriteLine("Der blev ikke fundet nogen medarbejdere.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"Id",-6} {"Navn",-22} {"Telefon",-14} {"Email",-28} {"Leder"}");
            Console.WriteLine(new string('-', 80));

            foreach (var employee in employees)
            {
                Console.WriteLine(
                    $"{employee.EmployeeId,-6} {employee.Name,-22} {employee.Phone,-14} {employee.Email,-28} {YesNo(employee.IsManager)}");
            }
        }

        static string YesNo(bool value)
        {
            return value ? "Ja" : "Nej";
        }

        static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Feltet må ikke være tomt.");
            }
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value))
                {
                    return value;
                }

                Console.WriteLine("Indtast et gyldigt heltal.");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (DateTime.TryParseExact(
                    input,
                    "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date))
                {
                    return date;
                }

                Console.WriteLine("Brug formatet dd-MM-yyyy, fx 15-10-2026.");
            }
        }

        static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                string answer = ReadText(prompt).ToLowerInvariant();
                if (answer is "ja" or "j" or "yes" or "y")
                {
                    return true;
                }

                if (answer is "nej" or "n" or "no")
                {
                    return false;
                }

                Console.WriteLine("Svar ja eller nej.");
            }
        }
    }
}
