using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Contractors;
using Store.Web.Contractors;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        readonly IBookRepository bookRepository;
        readonly IOrderRepositoty orderRepositoty;
        readonly IEnumerable<IDeliveryService> deliveryServices;
        readonly IEnumerable<IPaymentService> paymentServices;
        readonly IEnumerable<IWebContractorService> webContractorServices;
        readonly INotificationService notificationService;

        public OrderController(
            IBookRepository bookRepository,
            IOrderRepositoty orderRepositoty,
            IEnumerable<IDeliveryService> deliveryServices,
            IEnumerable<IPaymentService> paymentServices,
            IEnumerable<IWebContractorService> webContractorServices,
            INotificationService notificationService)
        {
            this.bookRepository = bookRepository;
            this.orderRepositoty = orderRepositoty;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webContractorServices = webContractorServices;
            this.notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                var order = orderRepositoty.GetById(cart.OrderId);
                OrderModel model = Map(order);

                return View(model);
            }

            return View("Empty");
        }

        OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(i => i.BookId);
            var books = bookRepository.GetAllByIds(bookIds);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = item.Price,
                                 Count = item.Count,
                             };
            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice
            };
        }

        [HttpPost]
        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            var book = bookRepository.GetById(bookId);
            order.AddOrUpdateItem(book, count);
            SaveOrderAndCart(order, cart);
            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            order.Get(bookId).Count = count;
            SaveOrderAndCart(order, cart);
            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public IActionResult RemoveItem (int bookId)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            order.RemoveItem(bookId);
            SaveOrderAndCart(order, cart);
            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var order = orderRepositoty.GetById(id);
            var model = Map(order);

            if (!IsValidCellPhone(cellPhone))
            {
                model.Errors["cellPhone"] = "Номер неверный";
                return View("Index", model);
            }

            var code = 1111;
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(cellPhone, code);

            return View("Confirmation", new ConfirmationModel { OrderId = id, CellPhone = cellPhone });
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if (storedCode == null)            
                return View("Confirmation", new ConfirmationModel { OrderId = id, CellPhone = cellPhone, 
                    Errors = new Dictionary<string, string> { { "code", "Пустой код" }}});
            

            if (storedCode != code)            
                return View("Confirmation", new ConfirmationModel { OrderId = id, CellPhone = cellPhone, 
                    Errors = new Dictionary<string, string> { { "code", "Неверный код" } } });

            var order = orderRepositoty.GetById(id);
            order.CellPhone = cellPhone;
            orderRepositoty.Update(order);

            HttpContext.Session.Remove(cellPhone);

            var model = new DeliveryModel
            {
                OrderId = id,
                Methods = deliveryServices.ToDictionary(s => s.UniqueCode, s => s.Title),
            };

            return View("DeliveryMethod", model);
        }

        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(s => s.UniqueCode == uniqueCode);
            var order = orderRepositoty.GetById(id);
            var form = deliveryService.CreateForm(order);
            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode, int step, Dictionary<string,string> values)
        {
            var deliveryService = deliveryServices.Single(s => s.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNextForm(id, step, values);
            
            if (form.IsFinal)
            {
                var order = orderRepositoty.GetById(id);
                order.Delivery = deliveryService.GetDelivery(form);
                orderRepositoty.Update(order);

                var model = new DeliveryModel
                {
                    OrderId = id,
                    Methods = paymentServices.ToDictionary(s => s.UniqueCode, s => s.Title),
                };

                return View("PaymentMethod", model);
            }

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult StartPayment(int id, string uniqueCode)
        {
            var paymentService = paymentServices.Single(s => s.UniqueCode == uniqueCode);
            var order = orderRepositoty.GetById(id);
            var form = paymentService.CreateForm(order);

            var webContractorService = webContractorServices.SingleOrDefault(s => s.UniqueCode == uniqueCode);
            if (webContractorService != null)
                return Redirect(webContractorService.GetUri);

            return View("PaymentStep", form);
        }

        [HttpPost]
        public IActionResult NextPayment(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var _paymentService = paymentServices.Single(s => s.UniqueCode == uniqueCode);

            var form = _paymentService.MoveNextForm(id, step, values);

            if (form.IsFinal)
            {
                var order = orderRepositoty.GetById(id);
                order.Payment = _paymentService.GetPayment(form);
                orderRepositoty.Update(order);

                return View("Finish");
            }

            return View("PaymentStep", form);
        }

        public IActionResult Finish()
        {
            HttpContext.Session.RemoveCart();
            return View();
        }

        private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
                return false;

            cellPhone = cellPhone.Replace(" ", "")
                                 .Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{11}$");
        }

        private (Order o, Cart c) GetOrCreateOrderAndCart()
        {
            Order order;
            if (HttpContext.Session.TryGetCart(out Cart cart))
                order = orderRepositoty.GetById(cart.OrderId);
            else
            {
                order = orderRepositoty.Create();
                cart = new Cart(order.Id);
            }
            return (order, cart);
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepositoty.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
            HttpContext.Session.Set(cart);
        }

    }

}
