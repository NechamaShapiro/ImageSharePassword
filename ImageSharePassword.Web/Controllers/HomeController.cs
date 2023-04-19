using ImageSharePassword.Data;
using ImageSharePassword.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace ImageSharePassword.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
            //var mgr = new DatabaseManager();
            //return View(mgr.GetAll());
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            var imageName = $"{Guid.NewGuid()}-{image.FileName}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", imageName);
            using var fs = new FileStream(imagePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var mgr = new DatabaseManager();
            int id = mgr.UploadImage(imageName, imagePath, password);
            Image img = mgr.GetImageById(id);
            return View(img);
        }
        public IActionResult ViewImage(int id)
        {
            var mgr = new DatabaseManager();
            var vm = new ImageViewModel();
            vm.Image = mgr.GetImageById(id);
            vm.ShowImage = false;
            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var mgr = new DatabaseManager();
            var vm = new ImageViewModel();
            vm.Image = mgr.GetImageById(id);
            List<int> ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (password == vm.Image.Password)
            {
                ids.Add(id);
                vm.ShowImage = true;
                mgr.IncrementViewsById(id);
            }
            else
            {
                TempData["invalid-password-message"] = "Invalid password. Please try again.";
                vm.ShowImage = false;
            }
            
            HttpContext.Session.Set("ids", ids);

            if (TempData["invalid-password-message"] != null)
            {
                ViewBag.Message = (string)TempData["invalid-password-message"];
            }
            return View(vm);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}