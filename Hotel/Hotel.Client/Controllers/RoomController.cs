using Microsoft.AspNetCore.Mvc;

namespace Hotel.Client.Controllers
{
	public class RoomController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
