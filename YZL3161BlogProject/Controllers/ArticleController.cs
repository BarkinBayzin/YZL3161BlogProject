using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using YZL3161BlogProject.Filters;
using YZL3161BlogProject.Managers;
using YZL3161BlogProject.Models.Data;
using YZL3161BlogProject.Models.Entity;
using YZL3161BlogProject.ViewModels.Article.Create;
using YZL3161BlogProject.ViewModels.Article.Edit;

namespace YZL3161BlogProject.Controllers
{
    [LoggedUser]
    public class ArticleController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArticleController(DatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            this._context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Create(string yonlen)
        {
            ViewBag.yonlen = yonlen;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateViewModel model, string yonlen)
        {
            if (ModelState.IsValid)
            {
                Article article = new Article
                {
                    Title = model.Title,
                    Content = model.Content,
                    AuthorId = int.Parse(HttpContext.Session.GetString("userId")),
                    ArticlePicture = model.ArticlePicture.GetUniqueNameAndSavePhotoToDisk(_webHostEnvironment)
                };
                _context.Articles.Add(article);
                _context.SaveChanges();
                TempData["message"] = "Article Created..!";
                if (yonlen == null) return RedirectToAction("Index", "Home");
                return Redirect(yonlen);
            }
            else return View(model);
        }

        public IActionResult Edit(int id)
        {
            Article article = _context.Articles.FirstOrDefault(x => x.Id.Equals(id) && x.AuthorId.ToString().Equals(HttpContext.Session.GetString("userId")));

            if (article is not null) return View(new EditViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ArticlePictureName = article.ArticlePicture
            });
            else
            {
                TempData["error"] = "Data could'n find";
                return RedirectToAction("Profile", "Home", new { username = HttpContext.Session.GetString("username") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Makalenin yazarı tarafından mı editleniyor kontrolü yapıyorum.
                Article article = _context.Articles.FirstOrDefault(x => x.Id.Equals(model.Id) && x.AuthorId.ToString().Equals(HttpContext.Session.GetString("userId")));
                //Eğer bu makalenin yazarı değilse veya id'den dolayı da data bana dolu geliyormu diye kontrol ediyorum.
                if (article is null)
                {
                    ViewData["error"] = "Edit failed..!";
                    return View(model);
                }
                //Editlenecek datanın mapping işlemlerini gerçekleştiriyorum
                article.Title = model.Title;
                article.Content = model.Content;
                //Edit gerçekleştirirken, resim de güncellenmek istiyor mu?
                if (model.ArticlePicture != null)
                {
                    //Eklenmeye çalışılan resmi disk'e kayıt ediyorum.
                    article.ArticlePicture = model.ArticlePicture.GetUniqueNameAndSavePhotoToDisk(_webHostEnvironment);
                    //Madem yeni bir resim var, o zaman eski resmimi silmeliyim.
                    FileManager.RemoveImageFromDisk(model.ArticlePictureName, _webHostEnvironment);
                }

                _context.SaveChanges();

                TempData["message"] = "Article Editing Completed..!";
                return RedirectToAction("Profile", "Home", new { username = HttpContext.Session.GetString("username") });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            //Makalenin yazarı tarafından mı editleniyor kontrolü yapıyorum.
            Article article = _context.Articles.FirstOrDefault(x => x.Id.Equals(id) && x.AuthorId.ToString().Equals(HttpContext.Session.GetString("userId")));

            //Silinmeye çalışılan makalenin yazar bilgisi, silmeye çalışan kişi ile eşleşiyorsa(idleri), silme başlar..
            if (article is not null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
                FileManager.RemoveImageFromDisk(article.ArticlePicture, _webHostEnvironment);
                TempData["message"] = "Delete Completed..!";
            }
            else TempData["error"] = "Data couldn't find";

            return RedirectToAction("Profile", "Home", new { username = HttpContext.Session.GetString("username") });
        }
    }
}
