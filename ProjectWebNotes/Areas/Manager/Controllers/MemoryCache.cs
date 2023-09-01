using Domain.IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProjectWebNotes.Models;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MemoryCache : Controller
    {
        public IMemoryCache _cache;

        [TempData]
        public string StatusMessage { get; set; }
        public MemoryCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            return View(MemoryCaheKey.KeyMemorys.ToList());
        }

        [HttpPost]
        public IActionResult Delete() 
        {
            foreach (var key in MemoryCaheKey.KeyMemorys.ToList())
            {
                _cache.Remove(key);
                
            }
           
            StatusMessage = $"Bộ nhớ cache đã bị xóa.";

            return RedirectToAction("index");

        }
    }
}
