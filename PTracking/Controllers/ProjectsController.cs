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

namespace PTracking.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Project.ToListAsync();

            // Retrieve other necessary data
            //int activeProjectCount = _context.Tickets.Count(project => project.Status == "In Progress");

            //int inActiveProjectCount = _context.Tickets.Count(project => project.Status == "Incomplete");

            // Other data retrieval logic...

            // Store data in ViewBag
                //ViewBag.Project = projects;
                //ViewBag.ActiveProjectCount = activeProjectCount;
                //ViewBag.InactiveProjectCount = inActiveProjectCount;

            // Get completion data
            int completeCount = _context.Project.Count(p => p.Status == "Completed");
            int incompleteCount = _context.Project.Count(p => p.Status == "Incomplete");
            int inProgressCount = _context.Project.Count(p => p.Status == "In Progress");

            ViewBag.Project = projects;
            ViewBag.CompleteCount = completeCount;
            ViewBag.IncompleteCount = incompleteCount;
            ViewBag.InProgressCount = inProgressCount;


            // Store completion data in a structured object
            var completionData = new
            {
                Labels = new List<string> { "Complete", "Incomplete", "In Progress" },
                ProjectCounts = new List<int> { completeCount, incompleteCount, inProgressCount }
            };

            ViewBag.ChartLabels = completionData.Labels;
            ViewBag.ChartData = completionData.ProjectCounts;

            var projectsWithNonNullUsers = _context.Project
            .Where(p => p.UsersAssigned != null) // Filter out null UsersAssigned
            .ToList(); // Retrieve projects first 

            var projectsByUser = projectsWithNonNullUsers
        .SelectMany(p => p.UsersAssigned.Split(',')) // Split users by ',' to count each user separately
        .Select(user => user.Trim()) // Trim to remove whitespace
        .GroupBy(user => user) // Group by individual user
        .Select(g => new { User = g.Key, Count = g.Count() })
        .ToList();

            var uniqueMembers = projectsByUser.Select(entry => entry.User).ToList();
            var memberOccurrences = projectsByUser.Select(entry => entry.Count).ToList();

            ViewBag.UniqueMembers = uniqueMembers;
            ViewBag.MemberOccurrences = memberOccurrences;


            var projectsByMonth = _context.Project
    .GroupBy(p => p.StartDate) // Group by StartDate
    .Select(g => new { StartDate = g.Key, ProjectCount = g.Count() })
    .OrderBy(entry => entry.StartDate) // Optional: Order by StartDate
    .ToList();

            var uniqueMonths = _context.Project
     .Select(p => p.StartDate)
     .Distinct()
     .ToList();

            var numOfProjects = projectsByMonth.Select(entry=>entry.ProjectCount).ToList();

            ViewBag.UniqueMonths = uniqueMonths;
            ViewBag.NumOfProjects = numOfProjects;

           
            int energryCount = _context.Project.Count(p => p.Category == "Energy Technology");
            int itCount = _context.Project.Count(p => p.Category == "Information and Technology");
            int cloudCount = _context.Project.Count(p => p.Category == "Cloud Services");


            var categoryData = new
            {
                CategoryLabels = new List<string> { "Energy Technology", "Information and Technology", "Cloud Services" },
                CategoryCounts = new List<int> { energryCount, itCount, cloudCount }
            };

            ViewBag.CategoryList = categoryData.CategoryLabels;
            ViewBag.CategoryCt = categoryData.CategoryCounts;



            //get employee 
            var employees = _context.Employee.ToList(); // Retrieve employees from your database

            // Assign employees to ViewBag in the desired format
            ViewBag.Employees = employees.Select(emp => new
            {
                Name = emp.Name,
                Email = emp.Email,
                Availability = emp.Availability
            });





            return View();
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
