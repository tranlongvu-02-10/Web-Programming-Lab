using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Lab04.WebsiteBanHang.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Lab04.WebsiteBanHang.Interfaces;
using Lab04.WebsiteBanHang.Extensions;
using Lab04.WebsiteBanHang.Data;

namespace Lab04.WebsiteBanHang.Controllers
{   
    [Authorize]
    public class ShoppingCartController : Controller 
    { 
        private readonly IProductRepository _productRepository; 
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;
 
        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository) 
        { 
            _productRepository = productRepository; 
            _context = context; 
            _userManager = userManager; 
        } 

        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost] 
        public async Task<IActionResult> Checkout(Order order) 
        { 
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart"); 
            if (cart == null || !cart.Items.Any()) 
            { 
                return RedirectToAction("Index"); 
            } 
  
            var user = await _userManager.GetUserAsync(User); 
            order.UserId = user.Id; 
            order.OrderDate = DateTime.UtcNow; 
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity); 
            order.OrderDetails = cart.Items.Select(i => new OrderDetail 
            { 
                ProductId = i.ProductId, 
                Quantity = i.Quantity, 
                Price = i.Price 
            }).ToList(); 
  
            _context.Orders.Add(order); 
            await _context.SaveChangesAsync(); 
            HttpContext.Session.Remove("Cart"); 
  
            return View("OrderCompleted", order.Id);
        } 

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1) 
        { 
            var product = await GetProductFromDatabase(productId); 
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart(); 
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem 
                { 
                    ProductId = productId, 
                    Name = product.Name, 
                    Price = product.Price, 
                    Quantity = quantity 
                }; 
                cart.AddItem(cartItem); 
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart); 
            return RedirectToAction("Index"); 
        }

        public async Task<IActionResult> SubtractFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    cart.Items.Remove(item);
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Index() 
        { 
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart(); 
            return View(cart); 
        } 

        private async Task<Product> GetProductFromDatabase(int productId) 
        { 
            var product = await _productRepository.GetByIdAsync(productId); 
            return product; 
        } 
 
        public IActionResult RemoveFromCart(int productId) 
        { 
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart"); 
            if (cart is not null) 
            { 
                cart.RemoveItem(productId); 
                HttpContext.Session.SetObjectAsJson("Cart", cart); 
            } 
            return RedirectToAction("Index"); 
        } 
    }
}