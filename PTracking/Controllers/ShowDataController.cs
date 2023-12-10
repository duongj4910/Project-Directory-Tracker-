using Microsoft.AspNetCore.Mvc;

namespace PTracking.Controllers
{
	public class ShowDataController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
