using Microsoft.AspNetCore.Mvc;

namespace Hotel.Client.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ServiceController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Add()
		{
			return View();
		}
		public IActionResult Edit()
		{
			return View();
		}
	}
}
