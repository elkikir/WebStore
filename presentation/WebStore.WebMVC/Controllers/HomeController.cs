﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Errors = new Dictionary<string, string> { {"fatal", "Ошибка выполнения запроса" } }
            });
        }
    }
}
