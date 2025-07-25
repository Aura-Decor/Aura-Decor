using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Helpers;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Repositoriy.Migrations;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuraDecor.APIs.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IResponseCacheService _cacheService;

        public CartController(ICartService cartService, IMapper mapper, IResponseCacheService cacheService)
        {
            _cartService = cartService;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound(new ApiResponse(404, "Cart not found"));
            }
            var cartDto = _mapper.Map<Cart, CartDto>(cart);
            return Ok(cartDto);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddToCart(AddToCartDto addToCartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId, addToCartDto.FurnitureId, addToCartDto.Quantity);
            
            await CacheInvalidationHelper.InvalidateCartCacheAsync(_cacheService);
            
            return Ok();
        }

        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveFromCart(RemoveFromCartDto removeFromCartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(userId, removeFromCartDto.FurnitureId);
            
            await CacheInvalidationHelper.InvalidateCartCacheAsync(_cacheService);
            
            return Ok();
        }
        [HttpDelete("clear-cart")]
        public async Task<ActionResult> RemoveAllItemsFromCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
                await _cartService.RemoveAllItemsFromCartAsync(userId);
                await CacheInvalidationHelper.InvalidateCartCacheAsync(_cacheService);
            return Ok();
        }
    }
}