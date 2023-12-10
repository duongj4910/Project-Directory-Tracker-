using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using PTracking.ViewModel;
using System.Diagnostics;

namespace PTracking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
			_context = context;
        }

		//public IActionResult Index()
		//{
		//    return View();
		//}
		public IActionResult Index()
		{
			// Fetch data from different tables using Entity Framework
			var employees = _context.Employee.ToList();
			var tickets = _context.Tickets.ToList();
			var salesData = _context.SalesData.ToList();

			var viewModel = new MultipleData
			{
				Employee = employees,
				Tickets = tickets,
				SalesEntity = salesData

			};

			return View(viewModel);
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