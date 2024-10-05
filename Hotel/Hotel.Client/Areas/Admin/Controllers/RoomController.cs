using Microsoft.AspNetCore.Mvc;

namespace Hotel.Client.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RoomController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult CreateRoom()
		{
			return View();
		}
	}
}
