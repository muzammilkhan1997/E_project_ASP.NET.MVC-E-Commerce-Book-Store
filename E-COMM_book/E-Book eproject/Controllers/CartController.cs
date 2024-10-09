using E_Book_eproject.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Security.Claims;

namespace E_Book_eproject.Controllers
{
    public class CartController : Controller
    {
        private static Random _random = new Random();
        EProjectContext db = new EProjectContext();
        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult AddToCart(Cart cart, int qty)
        {
            // Check if the cart parameter is null
            if (cart == null)
            {
                TempData["msg"] = "Cart cannot be null.";
                return RedirectToAction("MyCart");
            }

            var userId = User.FindFirstValue(ClaimTypes.Sid);

            // Redirect to login if the user is not authenticated
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Convert userId to an integer
            cart.UserId = Convert.ToInt32(userId);

            // Check for an existing cart item
            var duplicate = db.Carts.FirstOrDefault(c => c.ProductId == cart.ProductId && c.UserId == cart.UserId);

            if (duplicate != null)
            {
                // Ensure qty is a valid number before adding
                if (qty <= 0)
                {
                    TempData["msg"] = "Quantity must be greater than zero.";
                    return RedirectToAction("MyCart");
                }

                // Update the quantity of the existing cart item
                duplicate.Quantity += qty; // Assuming qty is the number to add
                db.Carts.Update(duplicate);
                TempData["msg"] = "Cart updated successfully.";
            }
            else
            {
                // Get product details and add new cart item
                var productDetails = db.Products.FirstOrDefault(x => x.Id == cart.ProductId);

                if (productDetails != null)
                {
                    cart.Price = productDetails.Price; // Set the price from the product details
                    cart.Quantity = qty; // Use the passed qty
                    db.Carts.Add(cart);
                    TempData["msg"] = "Item added to cart successfully.";
                }
                else
                {
                    TempData["msg"] = "Product not found.";
                    return RedirectToAction("MyCart");
                }
            }

            // Save changes to the database
            db.SaveChanges();
            return RedirectToAction("MyCart");
        }

        /*public IActionResult AddToCart(Cart cart, int qty)
        {



            var userId = User.FindFirstValue(ClaimTypes.Sid);

            var Duplicate = db.Carts.FirstOrDefault(c => c.ProductId == cart.ProductId && c.UserId == cart.UserId);

            if (userId != null)
            {
                cart.UserId = Convert.ToInt32(userId);


                Duplicate.Quantity += cart.Quantity; // = 2 + 1 
              

                db.Carts.Update(Duplicate);
                db.SaveChanges();
                TempData["msg"] = "Cart updated successfully.";
            }
            else
            {
                var productdetails = db.Products.FirstOrDefault(x => x.Id == cart.ProductId);
                cart.Price = productdetails?.Price;
                cart.Quantity = qty;

                db.Carts.Add(cart);
                db.SaveChanges();

                return RedirectToAction("MyCart");

               
            }
            return RedirectToAction("Login", "Auth");

        }*/
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Delete(int cartid)
        {
            var cart = db.Carts.Find(cartid);

            // Check if the cart is found
            //if (cart == null)
            //{
            //    // Optionally, return a custom error view or a redirect to an error page
            //    return NotFound();  // or handle it based on your application's logic
            //}

            db.Carts.Remove(cart);
            db.SaveChanges();

            return RedirectToAction("MyCart"); // It's better to redirect instead of returning a view
        }

        [Authorize(Roles = "User")]
        public IActionResult MyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid);

            if (userId != null)
            {
                int UserId = Convert.ToInt32(userId);

                // Joining Cart with Product using ProductId
                var cartItems = (from cart in db.Carts
                                 join product in db.Products
                                 on cart.ProductId equals product.Id
                                 where cart.UserId == UserId
                                 select new CheckoutViewModel
                                 {
                                     Id = cart.Id,
                                     Quantity = cart.Quantity,
                                     Name = product.Name,
                                     Price = (int)product.Price,
                                     Image = product.Image
                                 });

                // Pass the data to the view
                return View(cartItems.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

        }
       
        public IActionResult checkout()
        {

            return View();
        }



        [Authorize(Roles = "User")]
        // product details check out 
        [HttpPost]
        public IActionResult checkout(Order ord)
        {
            var UserId = User.FindFirstValue(ClaimTypes.Sid);
            if (UserId != null)
            {

                int userId = Convert.ToInt32(UserId);
                var data = db.Carts.Where(x => x.UserId == userId).ToList();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                var ordernumber = new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());

                int deliveryCharges = 0;
                if (ord.DeliveryDistance < 3)
                {
                    deliveryCharges = 0;

                }
                else
                {
                    deliveryCharges = (int)(ord.DeliveryDistance * 70);
                }
                decimal totalAmount = deliveryCharges;
                foreach (var item in data)
                {
                    totalAmount = (decimal)(totalAmount + (item.Price * item.Quantity));




                }


                Order order = new Order()
                {
                    OrderNumber = ordernumber,
                    TotalAmount = totalAmount,
                    DeliveryDistance = ord.DeliveryDistance,
                    CustomerId = userId,

                };

                var addOrder = db.Orders.Add(order);
                db.SaveChanges();


                foreach (var item in data)
                {
                    OrderDetail orderDetails = new OrderDetail()
                    {
                        OrderId = addOrder.Entity.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = (decimal)item.Price,

                    };

                    db.OrderDetails.Add(orderDetails);
                    db.Carts.Remove(item);
                    db.SaveChanges();



                }


            }
            return RedirectToAction("Index", "Home");
        }
        /*        [Authorize(Roles = "User")]
                [HttpGet]
                public IActionResult OrdreConfir()
                {
                    var userId = User.FindFirstValue(ClaimTypes.Sid);

                    if (userId != null)
                    {
                        var orderlis = (from Order in db.Orders
                                         join User in db.Users
                                         on Order.CustomerId equals User.Id
                                         where Order.CustomerId == User.Id
                                         select new orderlist
                                         {
                                             OrderId = Order.OrderId,
                                          *//*   Quantity = cart.Quantity,
                                             Name = product.Name,
                                             Price = (int)product.Price,
                                             Image = product.Image*//*
                                         });
                    }
                    return View();
                }*/
        [Authorize(Roles = "User")]
        public IActionResult Orderlist(Order order)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
            var data = db.Orders.Where(u=>u.CustomerId==userId).ToList();
            if(data != null  && userId != null)
            {
                return View(data);
            }
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult OrderDetails(int id)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
            var data = db.OrderDetails.Where(u=>u.OrderId == id).ToList();
          
                return View(data);
            }
          
        
        
    }
}
