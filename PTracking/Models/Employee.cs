using System.ComponentModel.DataAnnotations;

namespace PTracking.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }
        public string Name  { get; set; }
        public string Email { get; set; }
        public string ProjectsAssigned { get; set; }
        public string ClientsAssigned { get; set; }    
        public string Phone { get; set; }
        public string TimeZone { get; set; }
        public string DisctinctRole { get; set; }
        public string Role { get; set; }
        public string TeamLead { get; set; }
        public int TeamId { get; set; }
        public string ProjectManager { get; set; }

        public string Availability { get; set; }

        public string icon { get; set; }

        public string PTO { get; set; }

    }
}
