using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;

namespace PTracking.Services
{
	
	public class EmployeeService : IEmployeeService
	{
		private readonly ApplicationDbContext _context;

		public EmployeeService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Employee>> GetAllEmployeesAsync()
		{

				return await _context.Employee.ToListAsync();

		}
    		
	}

}
