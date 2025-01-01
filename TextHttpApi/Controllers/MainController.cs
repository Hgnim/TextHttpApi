using Microsoft.AspNetCore.Mvc;
using static TextHttpApi.DataCore;

namespace TextHttpApi.Controllers {
	public class MainController : Controller {
		public IActionResult Index() {
			//return View();
			return Redirect("/api");
		}
		[Route("api")]
		public string Api() {
			return allText[new Random().Next(0, allText.Count)];
		}
	}
}
