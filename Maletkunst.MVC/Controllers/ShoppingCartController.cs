using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;
using Maletkunst.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Maletkunst.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IPaintingsDataAccess _client;
        ShoppingCart cart = new ShoppingCart();

        public ShoppingCartController(IPaintingsDataAccess client)
        {
            _client = client;
        }
        
        public IActionResult Index()
        {
            return View(GetCartFromCookie(HttpContext));
        }

		public IActionResult Add(int id)
		{
			cart = GetCartFromCookie(HttpContext);

			var painting = _client.GetPaintingById(id);

			if (painting != null)
			{
				var existingItem = cart.Items.Find(i => i.Id == painting.Id);

				if (existingItem == null)
				{
					var shoppingCartItem = new ShoppingCartItem
					{
						Id = painting.Id,
						Name = painting.Title,
						Price = painting.Price,
						Quantity = 1
					};
					cart.Items.Add(shoppingCartItem);
				}
			}
			SaveCartToCookie(cart);
			return RedirectToAction("Index", "ShoppingCart");
		}

		private void SaveCartToCookie(ShoppingCart cart)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(7);
            cookieOptions.Path = "/";
            Response.Cookies.Append("ShoppingCart", JsonSerializer.Serialize(cart), cookieOptions);
        }

        public ShoppingCart GetCartFromCookie(HttpContext httpContext)
        {
            httpContext.Request.Cookies.TryGetValue("ShoppingCart", out string? cookie);
            if (cookie == null) { return new ShoppingCart(); }
            return JsonSerializer.Deserialize<ShoppingCart>(cookie) ?? new ShoppingCart();
        }

        public IActionResult Delete(int id)
        {
            ShoppingCart cart = GetCartFromCookie(HttpContext);
            ShoppingCartItem item = cart.Items.Find(i => i.Id == id);

            if (item != null)
            {
                cart.Items.Remove(item);
            }

            SaveCartToCookie(cart);
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult EmptyCart()
        {
            ShoppingCart cart = new ShoppingCart();
            SaveCartToCookie(cart);
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult ShoppingCart()
        {
            return View("~/Views/ShoppingCart/Index.cshtml", GetCartFromCookie(HttpContext));
        }

		public IActionResult CustomerInformation(string serializedShoppingCart)
		{
			var viewModel = new CustomerInformationViewModel
			{
				ShoppingCart = serializedShoppingCart
			};

			return View(viewModel);
		}
	}
}
