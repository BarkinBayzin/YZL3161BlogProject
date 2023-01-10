using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YZL3161BlogProject.Filters;
using YZL3161BlogProject.Models;
using YZL3161BlogProject.Models.Data;
using YZL3161BlogProject.ViewModels.Home.Overview;
using YZL3161BlogProject.ViewModels.Home.Profile;

namespace YZL3161BlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<ArticleViewModel> list = _context.Articles.OrderByDescending(x => x.CreatedTime)
                                           .Take(20)
                                           .Select(x => new ArticleViewModel()
                                           {
                                               Id = x.Id,
                                               AuthorId = x.Author.Id.ToString(),
                                               AuthorName = x.Author.Username,
                                               ArticlePicture = string.IsNullOrEmpty(x.ArticlePicture) ? "null.png" : x.ArticlePicture,
                                               Title = x.Title,
                                               Content = x.Content,
                                               CreatedTime = x.CreatedTime
                                           }).ToList();
            return View(list);
        }

        [HttpGet("[controller]/[action]/{username}")]
        public IActionResult Profile(string username)
            {
            List<ArticleViewModel> list = _context.Articles
                .Where(x => x.Author.Username.Equals(username))
                .OrderByDescending(x => x.CreatedTime)
                .Select(x => new ArticleViewModel()
                {
                    Id = x.Id,
                    AuthorId = x.Author.Id.ToString(),
                    AuthorName = x.Author.Username,
                    ArticlePicture = string.IsNullOrEmpty(x.ArticlePicture) ? "null.png" : x.ArticlePicture,
                    Title = x.Title,
                    Content = x.Content,
                    CreatedTime = x.CreatedTime
                }).ToList();
            return View(list);
        }

        public IActionResult Overview(int id)
        {
            /*
             * İster select kullan istersen sonradan class oluşuturup o class'a değerleri ata.
             * Ama önerilen Select kullanımıdır.
             * (Lazy Loading) (VİRTUAL SAYESİNDE UNUTMA!!!!!11!!!!!)
             * 
             *    var model = _context.Articles.Select(x => new OverviewViewModel()
            {
                Id = x.Id,
                Author = x.Author.Username,
                ArticlePicture = string.IsNullOrEmpty(x.ArticlePicture) ? "null.png" : x.ArticlePicture,
                Title = x.Title,
                Content = x.Content,
                CreatedTime = x.CreatedTime
            }).FirstOrDefault(x => x.Id.Equals(id));
             * 
             * 
             * (Eager Loading)
             * var data = _context.Articles.Include(x => x.Author).FirstOrDefault(x => x.Id.Equals(id));
             * var model = new OverviewViewModel()
            {
                Id = x.Id,
                Author = x.Author.Username,
                ArticlePicture = string.IsNullOrEmpty(x.ArticlePicture) ? "null.png" : x.ArticlePicture,
                Title = x.Title,
                Content = x.Content,
                CreatedTime = x.CreatedTime
            };
             */

            var model = _context.Articles.Select(x => new OverviewViewModel()
            {
                Id = x.Id,
                Author = x.Author.Username,
                ArticlePicture = string.IsNullOrEmpty(x.ArticlePicture) ? "null.png" : x.ArticlePicture,
                Title = x.Title,
                Content = x.Content,
                CreatedTime = x.CreatedTime
            }).FirstOrDefault(x => x.Id.Equals(id));

            return View(model);
        }

        [LoggedUser]
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
