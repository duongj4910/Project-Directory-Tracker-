using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using PTracking.Services;

namespace PTracking.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeService _employeeService;

        public EmployeesController(ApplicationDbContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employee.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult ChangeUserStatus(int employeeId, string newStatus)
        {
            var employee = _context.Employee.Find(employeeId);

            if (employee != null)
            {
                employee.Availability = newStatus;

                _context.SaveChanges(); // Save changes to the database

                return Ok("Status updated successfully");
            }
            else
            {
                return NotFound("Employee not found");
            }
        }
    


    // GET: Category/AddOrEdit
    public IActionResult AddOrEdit(int id = 0)
		{
			if (id == 0)
				return View(new Employee());
			else
				return View(_context.Employee.Find(id));

		}

		// POST: Category/AddOrEdit
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddOrEdit([Bind("ID,Name,Email,ProjectsAssigned,ClientsAssigned,Phone,TimeZone,DisctinctRole,Role,TeamLead,TeamId,ProjectManager,Availability,icon,PTO")] Employee employee)
		{
			if (ModelState.IsValid)
			{
				if (employee.ID == 0)
					_context.Add(employee);
				else
					_context.Update(employee);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(employee);
		}

		//// POST: Employees/Create
		//// To protect from overposting attacks, enable the specific properties you want to bind to.
		//// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
  //      [ValidateAntiForgeryToken]
  //      public async Task<IActionResult> Create([Bind("ID,Name,Email,ProjectsAssigned,ClientsAssigned,Phone,TimeZone,DisctinctRole,Role,TeamLead,TeamId,ProjectManager,Availability,icon,PTO")] Employee employee)
  //      {
  //          if (ModelState.IsValid)
  //          {
  //              _context.Add(employee);
  //              await _context.SaveChangesAsync();
  //              return RedirectToAction(nameof(Index));
  //          }
  //          return View(employee);
  //      }

        // GET: Employees/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Employee == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employee.FindAsync(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(employee);
        //}

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Email,ProjectsAssigned,ClientsAssigned,Phone,TimeZone,DisctinctRole,Role,TeamLead,TeamId,ProjectManager,Availability,icon,PTO")] Employee employee)
        //{
        //    if (id != employee.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(employee);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EmployeeExists(employee.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(employee);
        //}


        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employee'  is null.");
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employee?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}

// GET: Employees/Delete/5
//public async Task<IActionResult> Delete(int? id)
//{
//	if (id == null || _context.Employee == null)
//	{
//		return NotFound();
//	}

//	var employee = await _context.Employee
//		.FirstOrDefaultAsync(m => m.ID == id);
//	if (employee == null)
//	{
//		return NotFound();
//	}

//	return View(employee);
//}