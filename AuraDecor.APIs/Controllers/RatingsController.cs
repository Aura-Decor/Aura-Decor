using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ApiBaseController
    {
        private readonly IRatingService _ratingService;
        private readonly IMapper _mapper;

        public RatingsController(IRatingService ratingService, IMapper mapper)
        {
            _ratingService = ratingService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RatingDto>> AddRating([FromBody] AddRatingDto addRatingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rating = _mapper.Map<Rating>(addRatingDto);
            rating.UserId = userId;

            var result = await _ratingService.AddRatingAsync(rating);

            var ratingDto = _mapper.Map<RatingDto>(result);

            return Ok(ratingDto);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IReadOnlyList<RatingDto>>> GetRatingsForProduct(Guid productId)
        {
            var ratings = await _ratingService.GetRatingsForProductAsync(productId);
            var ratingsDto = _mapper.Map<IReadOnlyList<RatingDto>>(ratings);
            return Ok(ratingsDto);
        }
    }
} 