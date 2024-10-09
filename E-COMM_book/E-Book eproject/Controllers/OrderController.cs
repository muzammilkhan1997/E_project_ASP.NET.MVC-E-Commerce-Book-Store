using E_Book_eproject.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Book_eproject.Controllers
{
    public class OrderController : Controller
    {
        EProjectContext db = new EProjectContext();
        public IActionResult Index()
        {
            var data = db.Orders.ToList();
            return View(data);
        }
       
        public IActionResult Details(int OrderId)
        {
            var orderDetail = (from od in db.OrderDetails
                               join o in db.Orders on od.OrderId equals o.OrderId
                               join p in db.Products on od.ProductId equals p.Id
                               join u in db.Users on o.CustomerId equals u.Id
                               where od.OrderId == OrderId
                               select new OrderDetailViewModel
                               {
                                   OrderId = o.OrderId,
                                   Quantity = od.Quantity,
                                   
                                   Price = od.UnitPrice,
                               }).ToList();



            return View(orderDetail);
        }
       
        [HttpPost]
        public IActionResult Edit(int OrderID)
        {
            var data = db.Orders.Find(OrderID);
            db.Orders.Update(data);
            db.SaveChanges();
            return View();
        }
    }
}
