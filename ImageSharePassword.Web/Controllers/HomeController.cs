using ImageSharePassword.Data;
using ImageSharePassword.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
            return View(mgr.GetImageById(id));
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var mgr = new DatabaseManager();
            var img = mgr.GetImageById(id);
            if(password == img.Password)
            {
                HttpContext.Session.Set($"{id}", true);
            }
            return View(img);
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