using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using PTracking.Services;
using PTracking.ViewModel;
using System.Diagnostics;

namespace PTracking.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;
		private readonly ITicketService _ticketService;
		private readonly IEmployeeService _employeeService;
		private readonly IProjectService _projectService;


		public HomeController(ITicketService ticketService, IProjectService projectService, IEmployeeService employeeService, ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
			_ticketService = ticketService;
			_employeeService = employeeService;
			_projectService = projectService;
		}


		public async Task<IActionResult> Index()
		{

			int completeCount = _context.Project.Count(p => p.Status == "Completed");
			int incompleteCount = _context.Project.Count(p => p.Status == "Incomplete");
			int inProgressCount = _context.Project.Count(p => p.Status == "In Progress");

			ViewBag.CompleteCount = completeCount;
			ViewBag.IncompleteCount = incompleteCount;
			ViewBag.InProgressCount = inProgressCount;

			var tickets = await _ticketService.GetAllTicketsAsync();
			var activeTicketCount = await _ticketService.GetActiveTicketCountAsync();
			var priorityTickets = await _ticketService.GetPriorityTicketsAsync();
			var perSprint = await _ticketService.GetPerSprintAsync();
			var ticketAssignments = await _ticketService.GetTicketAssignmentsAsync();

			ViewBag.Tickets = tickets;
			ViewBag.ActiveTicketCount = activeTicketCount;
			ViewBag.PriorityTickets = priorityTickets;
			ViewBag.PerSprint = perSprint;

			ViewBag.UniqueMembers = ticketAssignments.Users;
			ViewBag.MemberOccurrences = ticketAssignments.TicketCounts;

			var projects = await _projectService.GetAllProjectsAsync();
			ViewBag.Projects = projects;

			var projectCompletionData = await _projectService.GetCompletionDataAsync();
			ViewBag.ProjectChartLabels = projectCompletionData.Labels;
			ViewBag.ProjectChartData = projectCompletionData.ProjectCounts;


			//PROJECTS 


			var projectsWithNonNullUsers = _context.Project
			  .Where(p => p.UsersAssigned != null) // Filter out null UsersAssigned
			  .ToList(); // Retrieve projects first 

			var projectsByUser = projectsWithNonNullUsers
				.SelectMany(p => p.UsersAssigned.Split(',')) // Split users by ',' to count each user separately
				.Select(user => user.Trim()) // Trim to remove whitespace
				.GroupBy(user => user) // Group by individual user
				.Select(g => new { User = g.Key, Count = g.Count() })
				.ToList();

			//card
			var uniqueMembers = projectsByUser.Select(entry => entry.User).ToList();
			var memberOccurrences = projectsByUser.Select(entry => entry.Count).ToList();

			ViewBag.UniqueMembers = uniqueMembers;
			ViewBag.MemberOccurrences = memberOccurrences;

			//card
			var uniqueCompanyCount = await _projectService.GetUniqueCompanyCountAsync();
			ViewBag.UniqueCompanyCount = uniqueCompanyCount;

			var projectsByMonth = _context.Project
			.GroupBy(p => p.StartDate) // Group by StartDate
			.Select(g => new { StartDate = g.Key, ProjectCount = g.Count() })
			.OrderBy(entry => entry.StartDate) // Optional: Order by StartDate
			.ToList();

			var uniqueMonths = _context.Project
				 .Select(p => p.StartDate)
				 .Distinct()
				 .ToList();

			var numOfProjects = projectsByMonth.Select(entry => entry.ProjectCount).ToList();

			ViewBag.UniqueMonths = uniqueMonths;
			ViewBag.NumOfProjects = numOfProjects;

			// Fetch completion data
			var completionData = await _ticketService.GetCompletionDataAsync();
			ViewBag.ChartLabels = completionData.Labels;
			ViewBag.ChartData = completionData.TicketCounts;

            var employees = await GetEmployeeViewModelsAsync();
            ViewBag.Employees = employees;

			var totalNumEmployees = await GetTotalEmployeeCountAsync();
			ViewBag.TotalNumEmployees = totalNumEmployees;
			return View();
		}
        public async Task<IActionResult> PopulateEmployeeData()
        {
            var employees = await GetEmployeeViewModelsAsync();
            return View(employees);
        }


        public async Task<List<EmployeeViewModel>> GetEmployeeViewModelsAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            var employeeViewModels = employees.Select(employee => new EmployeeViewModel
            {
                Name = employee.Name,
                Email = employee.Email,
                Availability = employee.Availability,
                icon = employee.icon,
                // Map other properties as needed
            }).ToList();

            return employeeViewModels;
        }

		public async Task<int> GetTotalEmployeeCountAsync()
		{
			var employees = await _employeeService.GetAllEmployeesAsync();
			return employees.Count;
		}


		[HttpPost]
		public IActionResult GetUserAssigned()
		{
			var ticketsByUser = _context.Tickets
				.GroupBy(t => t.UserAssigned)
				.Select(g => new { User = g.Key, Count = g.Count() })
				.ToList();

			var ticketNames = _context.Tickets
				.Select(t => t.Name)
				.Distinct()
				.ToList();

			List<string> users = ticketsByUser.Select(entry => entry.User).ToList();
			List<int> counts = ticketsByUser.Select(entry => entry.Count).ToList();

			return Json(new { chartLabels = ticketNames, chartData = counts });
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}