using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Formats.Asn1;


using System;
using System.Collections.Generic;

class UserAccount
{
    public string Username { get; set; }
    public string Password { get; set; }

    public UserAccount(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

class Enquiry
{
    public string EnquiryId { get; set; } = "";
    public string CandidateName { get; set; } = "";
    public string Qualification { get; set; } = "";
    public string Gender { get; set; } = "";
    public string VisitBy { get; set; } = "";
    public string MobileNumber { get; set; } = "";
    public string Remarks { get; set; } = "";
}

class Student
{
    public string RegistrationId { get; set; } = "";
    public string EnquiryId { get; set; } = "";
    public string CandidateName { get; set; } = "";
    public string FathersName { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; }
}

class Employee
{
    public string EmployeeId { get; set; } = "";
    public string EnquiryId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Experience { get; set; }
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; }
}

class Batch
{
    public string Name { get; set; } = "";
    public int TotalFee { get; set; }
    public int PaidFee { get; set; }
    public string Timing { get; set; } = "";
    public string Trainer { get; set; } = "";

    public int RemainingFee
    {
        get
        {
            return TotalFee - PaidFee;
        }
    }
}

class AttendanceRecord
{
    public string Date { get; set; } = "";
    public bool Present { get; set; }
}

class CollegeManagementSystem
{
    private UserAccount? account;

    private Enquiry? enquiry;
    private Student? student;
    private Employee? employee;
    private Batch? selectedBatch;

    private readonly List<AttendanceRecord> attendanceRecords =
        new List<AttendanceRecord>();

    // --------------------------------------------------
    // CONSOLE UI METHODS
    // --------------------------------------------------

    private void PrintLine()
    {
        Console.WriteLine(new string('=', 72));
    }

    private void PrintHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;

        PrintLine();
        Console.WriteLine($"                         {title}");
        PrintLine();

        Console.ResetColor();
        Console.WriteLine();
    }

    private void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nSUCCESS: {message}");
        Console.ResetColor();
    }

    private void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nERROR: {message}");
        Console.ResetColor();
    }

    private void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nINFO: {message}");
        Console.ResetColor();
    }

    private void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    private string ReadRequired(string message)
    {
        while (true)
        {
            Console.Write(message);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            PrintError("This field cannot be empty.");
        }
    }

    private int ReadInteger(string message, int minimum = 0)
    {
        while (true)
        {
            Console.Write(message);
            string input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out int value) && value >= minimum)
            {
                return value;
            }

            PrintError($"Please enter a valid number greater than or equal to {minimum}.");
        }
    }

    private int ReadChoice(string message, int minimum, int maximum)
    {
        while (true)
        {
            int choice = ReadInteger(message);

            if (choice >= minimum && choice <= maximum)
            {
                return choice;
            }

            PrintError($"Please choose an option between {minimum} and {maximum}.");
        }
    }

    // --------------------------------------------------
    // ACCOUNT CREATION AND LOGIN
    // --------------------------------------------------

    public void Start()
    {
        while (true)
        {
            PrintHeader("COLLEGE MANAGEMENT SYSTEM");

            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            PrintLine();

            int choice = ReadChoice("Enter your choice: ", 1, 3);

            switch (choice)
            {
                case 1:
                    CreateAccount();
                    break;

                case 2:
                    if (Login())
                    {
                        Dashboard();
                    }
                    break;

                case 3:
                    PrintHeader("GOODBYE");
                    Console.WriteLine("Thank you for using the College Management System.");
                    return;
            }
        }
    }

    private void CreateAccount()
    {
        PrintHeader("CREATE ACCOUNT");

        if (account != null)
        {
            PrintInfo("An account already exists in this session.");
            Pause();
            return;
        }

        string username = ReadRequired("Create username: ");

        string password;

        while (true)
        {
            password = ReadRequired("Create password: ");

            string confirmPassword = ReadRequired("Confirm password: ");

            if (password == confirmPassword)
            {
                break;
            }

            PrintError("Passwords do not match. Try again.");
        }

        account = new UserAccount(username, password);

        PrintSuccess("Your account has been created successfully.");
        PrintInfo("You can now log in using your new credentials.");

        Pause();
    }

    private bool Login()
    {
        PrintHeader("LOGIN");

        if (account == null)
        {
            PrintError("No account exists. Please create an account first.");
            Pause();
            return false;
        }

        const int maximumAttempts = 3;

        for (int attempt = 1; attempt <= maximumAttempts; attempt++)
        {
            string username = ReadRequired("Username: ");
            string password = ReadRequired("Password: ");

            if (username == account.Username && password == account.Password)
            {
                PrintSuccess("Login successful.");
                Pause();
                return true;
            }

            int attemptsLeft = maximumAttempts - attempt;

            PrintError("Incorrect username or password.");

            if (attemptsLeft > 0)
            {
                Console.WriteLine($"Attempts remaining: {attemptsLeft}");
            }
        }

        PrintError("Maximum login attempts reached.");
        Pause();

        return false;
    }

    // --------------------------------------------------
    // DASHBOARD
    // --------------------------------------------------

    private void Dashboard()
    {
        while (true)
        {
            PrintHeader("COLLEGE MANAGEMENT DASHBOARD");

            Console.WriteLine("1. Enquiry Form");
            Console.WriteLine("2. Student Registration");
            Console.WriteLine("3. Employee Registration");
            Console.WriteLine("4. Batch Plans");
            Console.WriteLine("5. Fee Management");
            Console.WriteLine("6. Leave / Cancel Batch Plan");
            Console.WriteLine("7. Attendance");
            Console.WriteLine("8. Certificate");
            Console.WriteLine("9. Career Guidance");
            Console.WriteLine("10. View Profile");
            Console.WriteLine("11. Logout");

            PrintLine();

            int choice = ReadChoice("What do you want to do? ", 1, 11);

            switch (choice)
            {
                case 1:
                    EnquiryForm();
                    break;

                case 2:
                    StudentRegistration();
                    break;

                case 3:
                    EmployeeRegistration();
                    break;

                case 4:
                    BatchPlans();
                    break;

                case 5:
                    FeeManagement();
                    break;

                case 6:
                    CancelBatchPlan();
                    break;

                case 7:
                    Attendance();
                    break;

                case 8:
                    Certificate();
                    break;

                case 9:
                    Career();
                    break;

                case 10:
                    ViewProfile();
                    break;

                case 11:
                    PrintInfo("You have been logged out.");
                    Pause();
                    return;
            }
        }
    }

    // --------------------------------------------------
    // ENQUIRY FORM
    // --------------------------------------------------

    private void EnquiryForm()
    {
        PrintHeader("ENQUIRY FORM");

        if (enquiry != null)
        {
            PrintInfo("An enquiry form has already been submitted.");
            Pause();
            return;
        }

        enquiry = new Enquiry();

        enquiry.EnquiryId = ReadRequired("Enquiry ID: ");
        enquiry.CandidateName = ReadRequired("Candidate name: ");
        enquiry.Qualification = ReadRequired("Qualification: ");
        enquiry.Gender = ReadRequired("Gender: ");
        enquiry.VisitBy = ReadRequired("Visit by: ");
        enquiry.MobileNumber = ReadRequired("Mobile number: ");
        enquiry.Remarks = ReadRequired("Remarks: ");

        PrintSuccess($"Enquiry form submitted for {enquiry.CandidateName}.");
        Pause();
    }

    // --------------------------------------------------
    // STUDENT REGISTRATION
    // --------------------------------------------------

    private void StudentRegistration()
    {
        PrintHeader("STUDENT REGISTRATION");

        if (enquiry == null)
        {
            PrintError("Please complete the enquiry form first.");
            Pause();
            return;
        }

        if (student != null)
        {
            PrintInfo("A student profile already exists.");
            Pause();
            return;
        }

        if (employee != null)
        {
            PrintError("This account is already registered as an employee.");
            Pause();
            return;
        }

        string enteredEnquiryId =
            ReadRequired("Enter your enquiry ID: ");

        if (enteredEnquiryId != enquiry.EnquiryId)
        {
            PrintError("Incorrect enquiry ID.");
            Pause();
            return;
        }

        student = new Student();

        student.EnquiryId = enquiry.EnquiryId;
        student.CandidateName = enquiry.CandidateName;
        student.RegistrationId = ReadRequired("Registration ID: ");
        student.FathersName = ReadRequired("Father's name: ");
        student.Address = ReadRequired("Address: ");
        student.Email = ReadRequired("Email: ");
        student.Age = ReadInteger("Age: ", 1);

        PrintSuccess($"Student registration completed for {student.CandidateName}.");
        Pause();
    }

    // --------------------------------------------------
    // EMPLOYEE REGISTRATION
    // --------------------------------------------------

    private void EmployeeRegistration()
    {
        PrintHeader("EMPLOYEE REGISTRATION");

        if (enquiry == null)
        {
            PrintError("Please complete the enquiry form first.");
            Pause();
            return;
        }

        if (employee != null)
        {
            PrintInfo("An employee profile already exists.");
            Pause();
            return;
        }

        if (student != null)
        {
            PrintError("This account is already registered as a student.");
            Pause();
            return;
        }

        string enteredEnquiryId =
            ReadRequired("Enter your enquiry ID: ");

        if (enteredEnquiryId != enquiry.EnquiryId)
        {
            PrintError("Incorrect enquiry ID.");
            Pause();
            return;
        }

        employee = new Employee();

        employee.EnquiryId = enquiry.EnquiryId;
        employee.Name = enquiry.CandidateName;
        employee.EmployeeId = ReadRequired("Employee ID: ");
        employee.Experience = ReadInteger("Experience in years: ");
        employee.Address = ReadRequired("Address: ");
        employee.Email = ReadRequired("Email: ");
        employee.Age = ReadInteger("Age: ", 1);

        PrintSuccess($"Employee registration completed for {employee.Name}.");
        Pause();
    }

    // --------------------------------------------------
    // BATCH PLANS
    // --------------------------------------------------

    private void BatchPlans()
    {
        PrintHeader("BATCH PLANS");

        if (student == null)
        {
            PrintError("Only registered students can select a batch.");
            Pause();
            return;
        }

        if (selectedBatch != null)
        {
            PrintInfo("You already have a selected batch.");
            DisplayBatchDetails();
            Pause();
            return;
        }

        Console.WriteLine("Available batch plans:\n");

        Console.WriteLine("1. Java");
        Console.WriteLine("   Fee: Rs. 10,000");
        Console.WriteLine("   Timing: 10 AM");
        Console.WriteLine("   Trainer: Anuj Sir\n");

        Console.WriteLine("2. C#");
        Console.WriteLine("   Fee: Rs. 15,000");
        Console.WriteLine("   Timing: 1 PM");
        Console.WriteLine("   Trainer: Ashutosh Sir\n");

        int choice = ReadChoice("Choose a batch plan: ", 1, 2);

        if (choice == 1)
        {
            selectedBatch = new Batch
            {
                Name = "Java",
                TotalFee = 10000,
                PaidFee = 0,
                Timing = "10 AM",
                Trainer = "Anuj Sir"
            };
        }
        else
        {
            selectedBatch = new Batch
            {
                Name = "C#",
                TotalFee = 15000,
                PaidFee = 0,
                Timing = "1 PM",
                Trainer = "Ashutosh Sir"
            };
        }

        PrintSuccess($"You selected the {selectedBatch.Name} batch.");
        PrintInfo("Please use Fee Management to pay the batch fee.");

        Pause();
    }

    private void DisplayBatchDetails()
    {
        if (selectedBatch == null)
        {
            PrintInfo("No batch selected.");
            return;
        }

        Console.WriteLine($"Batch name: {selectedBatch.Name}");
        Console.WriteLine($"Total fee: Rs. {selectedBatch.TotalFee}");
        Console.WriteLine($"Paid fee: Rs. {selectedBatch.PaidFee}");
        Console.WriteLine($"Remaining fee: Rs. {selectedBatch.RemainingFee}");
        Console.WriteLine($"Timing: {selectedBatch.Timing}");
        Console.WriteLine($"Trainer: {selectedBatch.Trainer}");
    }

    // --------------------------------------------------
    // FEE MANAGEMENT
    // --------------------------------------------------

    private void FeeManagement()
    {
        PrintHeader("FEE MANAGEMENT");

        if (selectedBatch == null)
        {
            PrintError("Please select a batch first.");
            Pause();
            return;
        }

        while (true)
        {
            PrintHeader("FEE MANAGEMENT");

            DisplayBatchDetails();

            Console.WriteLine("\n1. Pay Fee");
            Console.WriteLine("2. Check Fee");
            Console.WriteLine("3. Back");

            int choice = ReadChoice("Enter your choice: ", 1, 3);

            if (choice == 1)
            {
                PayFee();
            }
            else if (choice == 2)
            {
                PrintHeader("FEE DETAILS");
                DisplayBatchDetails();
                Pause();
            }
            else
            {
                return;
            }
        }
    }

    private void PayFee()
    {
        if (selectedBatch == null)
        {
            return;
        }

        if (selectedBatch.RemainingFee == 0)
        {
            PrintInfo("Your batch fee has already been paid completely.");
            Pause();
            return;
        }

        int amount = ReadInteger("Enter payment amount: ", 1);

        if (amount > selectedBatch.RemainingFee)
        {
            PrintError("Payment cannot exceed the remaining fee.");
        }
        else
        {
            selectedBatch.PaidFee += amount;

            PrintSuccess($"Payment of Rs. {amount} received.");
            Console.WriteLine($"Remaining fee: Rs. {selectedBatch.RemainingFee}");
        }

        Pause();
    }

    // --------------------------------------------------
    // CANCEL BATCH PLAN
    // --------------------------------------------------

    private void CancelBatchPlan()
    {
        PrintHeader("CANCEL BATCH PLAN");

        if (selectedBatch == null)
        {
            PrintInfo("You do not have an active batch plan.");
            Pause();
            return;
        }

        DisplayBatchDetails();

        Console.WriteLine("\nDo you want to cancel this batch?");
        Console.WriteLine("1. Yes");
        Console.WriteLine("2. No");

        int choice = ReadChoice("Enter your choice: ", 1, 2);

        if (choice == 1)
        {
            int refundAmount = selectedBatch.PaidFee;

            selectedBatch = null;

            PrintSuccess("Batch plan cancelled successfully.");
            Console.WriteLine($"Refund amount recorded: Rs. {refundAmount}");
        }
        else
        {
            PrintInfo("No changes were made.");
        }

        Pause();
    }

    // --------------------------------------------------
    // ATTENDANCE
    // --------------------------------------------------

    private void Attendance()
    {
        PrintHeader("ATTENDANCE");

        if (student == null)
        {
            PrintError("Attendance is available only for registered students.");
            Pause();
            return;
        }

        while (true)
        {
            PrintHeader("ATTENDANCE");

            Console.WriteLine("1. Mark Attendance");
            Console.WriteLine("2. View Attendance");
            Console.WriteLine("3. Back");

            int choice = ReadChoice("Enter your choice: ", 1, 3);

            if (choice == 1)
            {
                MarkAttendance();
            }
            else if (choice == 2)
            {
                ViewAttendance();
            }
            else
            {
                return;
            }
        }
    }

    private void MarkAttendance()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        foreach (AttendanceRecord record in attendanceRecords)
        {
            if (record.Date == today)
            {
                PrintInfo("Attendance has already been marked for today.");
                Pause();
                return;
            }
        }

        Console.WriteLine("1. Present");
        Console.WriteLine("2. Absent");

        int choice = ReadChoice("Enter your attendance: ", 1, 2);

        attendanceRecords.Add(new AttendanceRecord
        {
            Date = today,
            Present = choice == 1
        });

        PrintSuccess("Attendance recorded successfully.");
        Pause();
    }

    private void ViewAttendance()
    {
        PrintHeader("ATTENDANCE RECORD");

        if (attendanceRecords.Count == 0)
        {
            PrintInfo("No attendance records found.");
            Pause();
            return;
        }

        int presentCount = 0;

        foreach (AttendanceRecord record in attendanceRecords)
        {
            string status = record.Present ? "Present" : "Absent";

            Console.WriteLine($"{record.Date} - {status}");

            if (record.Present)
            {
                presentCount++;
            }
        }

        Console.WriteLine($"\nTotal days: {attendanceRecords.Count}");
        Console.WriteLine($"Present days: {presentCount}");

        double percentage =
            (double)presentCount / attendanceRecords.Count * 100;

        Console.WriteLine($"Attendance percentage: {percentage:F2}%");

        Pause();
    }

    // --------------------------------------------------
    // CERTIFICATE
    // --------------------------------------------------

    private void Certificate()
    {
        PrintHeader("CERTIFICATE SECTION");

        if (student == null)
        {
            PrintError("Certificate services are available only for students.");
            Pause();
            return;
        }

        Console.WriteLine("Certificate services:\n");

        Console.WriteLine("Student name: " + student.CandidateName);
        Console.WriteLine("Registration ID: " + student.RegistrationId);

        if (selectedBatch == null)
        {
            PrintInfo("Select a batch before requesting a certificate.");
        }
        else if (selectedBatch.RemainingFee > 0)
        {
            PrintInfo("Complete your batch fee before requesting a certificate.");
        }
        else if (attendanceRecords.Count == 0)
        {
            PrintInfo("Attendance records are required before certificate eligibility can be checked.");
        }
        else
        {
            PrintSuccess("You are eligible to request a course completion certificate.");
            Console.WriteLine("Certificate status: Eligible for generation");
        }

        Pause();
    }

    // --------------------------------------------------
    // CAREER GUIDANCE
    // --------------------------------------------------

    private void Career()
    {
        PrintHeader("CAREER GUIDANCE");

        Console.WriteLine("Choose a career path:\n");

        Console.WriteLine("1. C# / .NET Developer");
        Console.WriteLine("2. Java Developer");
        Console.WriteLine("3. Web Developer");
        Console.WriteLine("4. Database Developer");
        Console.WriteLine("5. Back");

        int choice = ReadChoice("Enter your choice: ", 1, 5);

        switch (choice)
        {
            case 1:
                PrintHeader("C# / .NET DEVELOPER");
                Console.WriteLine("Recommended learning path:");
                Console.WriteLine("- C# fundamentals");
                Console.WriteLine("- Object-oriented programming");
                Console.WriteLine("- LINQ");
                Console.WriteLine("- ASP.NET Core");
                Console.WriteLine("- Entity Framework Core");
                Console.WriteLine("- SQL and databases");
                Console.WriteLine("- Git and GitHub");
                Pause();
                break;

            case 2:
                PrintHeader("JAVA DEVELOPER");
                Console.WriteLine("Recommended learning path:");
                Console.WriteLine("- Java fundamentals");
                Console.WriteLine("- Object-oriented programming");
                Console.WriteLine("- Collections");
                Console.WriteLine("- Exception handling");
                Console.WriteLine("- JDBC");
                Console.WriteLine("- Spring Boot");
                Console.WriteLine("- SQL and databases");
                Pause();
                break;

            case 3:
                PrintHeader("WEB DEVELOPER");
                Console.WriteLine("Recommended learning path:");
                Console.WriteLine("- HTML");
                Console.WriteLine("- CSS");
                Console.WriteLine("- JavaScript");
                Console.WriteLine("- Responsive design");
                Console.WriteLine("- Git and GitHub");
                Console.WriteLine("- React or another frontend framework");
                Pause();
                break;

            case 4:
                PrintHeader("DATABASE DEVELOPER");
                Console.WriteLine("Recommended learning path:");
                Console.WriteLine("- SQL fundamentals");
                Console.WriteLine("- MySQL or SQL Server");
                Console.WriteLine("- Database design");
                Console.WriteLine("- Normalization");
                Console.WriteLine("- Joins and subqueries");
                Console.WriteLine("- Stored procedures");
                Console.WriteLine("- Database security");
                Pause();
                break;
        }
    }

    // --------------------------------------------------
    // PROFILE
    // --------------------------------------------------

    private void ViewProfile()
    {
        PrintHeader("PROFILE");

        if (account != null)
        {
            Console.WriteLine($"Username: {account.Username}");
        }

        if (enquiry != null)
        {
            Console.WriteLine("\nEnquiry status: Completed");
            Console.WriteLine($"Candidate name: {enquiry.CandidateName}");
            Console.WriteLine($"Enquiry ID: {enquiry.EnquiryId}");
        }
        else
        {
            Console.WriteLine("\nEnquiry status: Not completed");
        }

        if (student != null)
        {
            Console.WriteLine("\nRegistration type: Student");
            Console.WriteLine($"Registration ID: {student.RegistrationId}");
        }
        else if (employee != null)
        {
            Console.WriteLine("\nRegistration type: Employee");
            Console.WriteLine($"Employee ID: {employee.EmployeeId}");
        }
        else
        {
            Console.WriteLine("\nRegistration status: Not completed");
        }

        if (selectedBatch != null)
        {
            Console.WriteLine($"\nSelected batch: {selectedBatch.Name}");
            Console.WriteLine($"Remaining fee: Rs. {selectedBatch.RemainingFee}");
        }
        else
        {
            Console.WriteLine("\nBatch status: No batch selected");
        }

        Pause();
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        CollegeManagementSystem system = new CollegeManagementSystem();

        system.Start();
    }
}