using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(string Searchtext, int? pages)
        {
            var pageSizes = 8;
            if (pages == null)
            {
                pages = 1;
            }
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            var pageIndex = pages.HasValue ? Convert.ToInt32(pages) : 1;
            items = items.ToPagedList(pageIndex, pageSizes);
            ViewBag.PageSize = pageSizes;
            ViewBag.Page = pages;
            return View(items);
        }
        public ActionResult Detail(string alias,int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            
            return View(item);
        }
        public ActionResult ProductCategory(string Searchtext,string alias,int id,int? pages)

        {
            var pageSizes = 8;
            if (pages == null)
            {
                pages = 1;
            }
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }  
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            var pageIndex = pages.HasValue ? Convert.ToInt32(pages) : 1;
            items = items.ToPagedList(pageIndex, pageSizes);
            ViewBag.PageSize = pageSizes;
            ViewBag.Page = pages;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {

            var items = db.Products.Where(x => x.IsActive && x.IsFeature).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductSales()
        {
            var items = db.Products.Where(x => x.IsSale && x.IsActive && x.IsHot).ToList();
            return PartialView(items);
        }
    }
}