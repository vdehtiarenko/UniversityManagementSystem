namespace UniversityManagementSystem.DTO
{
    public class StudentCsvModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public StudentCsvModel(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
