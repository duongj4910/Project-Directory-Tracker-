using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using PTracking.Services;
using PTracking.ViewModel;

namespace PTracking.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly ITicketService _ticketService;
		private readonly IEmployeeService _employeeService;
		private readonly IProjectService _projectService;


		public ProjectsController(ApplicationDbContext context, IEmployeeService employeeService, ITicketService ticketService, IProjectService projectService)
        {
            _context = context;
			_ticketService = ticketService;
			_employeeService = employeeService;
			_projectService = projectService;
		}

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Project.ToListAsync();

            var projectsByName = await _context.Project
                .GroupBy(p => p.Name)
                .Select(g => new { ProjectName = g.Key, Count = g.Count() })
                .ToListAsync();

            int completeCount = await _context.Project.CountAsync(p => p.Status == "Completed");
            int incompleteCount = await _context.Project.CountAsync(p => p.Status == "Incomplete");
            int inProgressCount = await _context.Project.CountAsync(p => p.Status == "In Progress");
            int uniqueProjectNamesCount = projectsByName.Count;
            int highPriorityProjects = await _context.Project.CountAsync(p => p.Priority == "High");

           
            

            ViewBag.Project = projects;
            ViewBag.CompleteCount = completeCount;
            ViewBag.IncompleteCount = incompleteCount;
            ViewBag.InProgressCount = inProgressCount;
            ViewBag.UniqueProjectNamesCount = uniqueProjectNamesCount;
            ViewBag.HighPriorityProjects = highPriorityProjects;

            var completionData = new
            {
                Labels = new List<string> { "Complete", "Incomplete", "In Progress" },
                ProjectCounts = new List<int> { completeCount, incompleteCount, inProgressCount }
            };

            ViewBag.ChartLabels = completionData.Labels;
            ViewBag.ChartData = completionData.ProjectCounts;

            var projectsWithNonNullUsers = await _context.Project
                .Where(p => p.UsersAssigned != null)
                .ToListAsync();

            var projectsByUser = projectsWithNonNullUsers
                .SelectMany(p => p.UsersAssigned.Split(','))
                .Select(user => user.Trim())
                .GroupBy(user => user)
                .Select(g => new { User = g.Key, Count = g.Count() })
                .ToList();

            var uniqueMembers = projectsByUser.Select(entry => entry.User).ToList();
            var memberOccurrences = projectsByUser.Select(entry => entry.Count).ToList();

            ViewBag.UniqueMembers = uniqueMembers;
            ViewBag.MemberOccurrences = memberOccurrences;

            var projectsByMonth = await _context.Project
                .GroupBy(p => p.StartDate)
                .Select(g => new { StartDate = g.Key, ProjectCount = g.Count() })
                .OrderBy(entry => entry.StartDate)
                .ToListAsync();

            var uniqueMonths = await _context.Project
                .Select(p => p.StartDate)
                .Distinct()
                .ToListAsync();

            var numOfProjects = projectsByMonth.Select(entry => entry.ProjectCount).ToList();

            ViewBag.UniqueMonths = uniqueMonths;
            ViewBag.NumOfProjects = numOfProjects;

            int energyCount = await _context.Project.CountAsync(p => p.Category == "Energy Technology");
            int itCount = await _context.Project.CountAsync(p => p.Category == "Information and Technology");
            int cloudCount = await _context.Project.CountAsync(p => p.Category == "Cloud Services");

            var categoryData = new
            {
                CategoryLabels = new List<string> { "Energy Technology", "Information and Technology", "Cloud Services" },
                CategoryCounts = new List<int> { energyCount, itCount, cloudCount }
            };

            ViewBag.CategoryList = categoryData.CategoryLabels;
            ViewBag.CategoryCt = categoryData.CategoryCounts;

           

            var employees = await GetEmployeeViewModelsAsync();
            ViewBag.Employees = employees;

            return View();
        }


        public async Task<IActionResult> DisplayAllProjects()
        {
            var projects = await _projectService.GetProjectsTotalAsync();

            return View(projects);

        }

        public async Task<IActionResult> DisplayActiveProjects()
		{
            var incompleteOrInProgressTickets = await _projectService.GetProjectsByStatusAsync();

			return View(incompleteOrInProgressTickets);


		}

        public async Task<IActionResult> DisplayCompletedProjects()
        {
            var complete = await _projectService.GetProjectsByCompletedStatusAsync();

            return View(complete);


        }


        public async Task<IActionResult> DisplayPriority()
        {
            var highPriorityProjects = await _projectService.GetProjectsByPriorityAsync();

            return View(highPriorityProjects);


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
                icon = employee.icon,
                Name = employee.Name,
				Email = employee.Email,
				Availability = employee.Availability
				
				// Map other properties as needed
			}).ToList();

			return employeeViewModels;
		}


		// GET: Projects/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,ProjectName,Company,UsersAssigned,AssignedBy,Status,StatusMessage,Completion,Priority,Category,DueBy,UpdatedDate,StartDate,icon")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,ProjectName,Company,UsersAssigned,AssignedBy,Status,StatusMessage,Completion,Priority,Category,DueBy,UpdatedDate,StartDate,icon")] Project project)
        {
            if (id != project.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Project'  is null.");
            }
            var project = await _context.Project.FindAsync(id);
            if (project != null)
            {
                _context.Project.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Project?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
