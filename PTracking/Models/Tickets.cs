using System.ComponentModel.DataAnnotations;

namespace PTracking.Models
{
    public class Tickets
    {
        [Key]
        public int ID { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public string Company { get; set; }
        public string UserAssigned { get; set; }
        public string AssignedBy { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public string DueBy { get; set; }
        public string? UpdatedDate { get; set; }
        public string? StartDate { get; set; }
        public string Quarter { get; set; }
        public int MaxTicketsPerSprint { get; set; }
        public int SprintStoryPointLimit { get; set; }
        public int PointPerTicket { get; set; }
    }
}
