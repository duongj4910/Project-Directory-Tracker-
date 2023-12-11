using PTracking.Models;

namespace PTracking.ViewModel
{
	public class MultipleData
	{
		public IEnumerable<Employee> Employee { get; set; }
		public IEnumerable<Tickets> Tickets { get; set; }
		public IEnumerable<SalesEntity>	SalesEntity { get; set; }

	}
}
