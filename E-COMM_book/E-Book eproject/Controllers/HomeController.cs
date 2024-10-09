using System;
using System.Diagnostics;

using E_Book_eproject.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;

namespace E_Book_eproject.Controllers
{
    public class HomeController : Controller
    {
        EProjectContext db = new EProjectContext();

      
        public IActionResult Index(Product product)
        {
            var data =db.Products.ToList();
            return View(data);
            //List<Product> data;
            //if (catid.HasValue)
            //{
            //    data = db.Products.Where(b => b.CatId == catid).ToList();

            //}
            //else
            //{
            //    data = db.Products.ToList();
            //}
            //return View();
        }


       
        public IActionResult shop(int? catid)
        {
            List<Product> data;
            if (catid.HasValue)
            {
                data = db.Products.Where(b => b.CatId == catid ||  b.SubId == catid ).ToList();

            }
            else
            {
                data = db.Products.ToList();
            }
            return View(data);
        }

        /*
                public async Task<IActionResult> shop(string seacrhstring)
                {
                    var shop = await EProjectContext.GetAllAsync();
                    if (!String.IsNullOrEmpty(seacrhstring))
                    {
                        shop = Product.where( n=>n.FirstNAme.Contains(seacrhstring) || n.lastName.Contains(seacrhstring)).Tolist() ;
                    }
                    return View(shop);
                }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}