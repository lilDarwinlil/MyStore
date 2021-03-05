using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class CartController : Controller
    {
        readonly IBookRepository bookRepository;
        readonly IOrderRepositoty orderRepositoty;

        public CartController(IBookRepository bookRepository,IOrderRepositoty orderRepositoty)
        {
            this.bookRepository = bookRepository;
            this.orderRepositoty = orderRepositoty;
        }
        public IActionResult Add(int id)
        {            
            Order order;
            Cart cart;
            if (HttpContext.Session.TryGetCart(out cart))
                order = orderRepositoty.GetById(cart.OrderId);
            else
                order = orderRepositoty.Create();
                cart = new Cart(order.Id);

            var book = bookRepository.GetById(id);
            order.AddItem(book, 1);
            orderRepositoty.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { id });
        }
    }
}
