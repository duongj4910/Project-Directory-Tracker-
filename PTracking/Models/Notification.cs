namespace PTracking.Models
{
	public class Notification
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public string Viewed { get; set; }

		public string Recipient { get; set; }

	}
}
