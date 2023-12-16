using PTracking.Models;

namespace PTracking.Services
{
	public interface IEmployeeService
	{
		Task<List<Employee>> GetAllEmployeesAsync();
	}
}
