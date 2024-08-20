﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
//using Contracts;
//using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    //private readonly IAuctionRepository _repo;
    private readonly IMapper _mapper;
    private readonly AuctionDbContext _context;

    //private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(/*IAuctionRepository repo,*/ IMapper mapper,AuctionDbContext context
       /* IPublishEndpoint publishEndpoint*/)
    {
        //_repo = repo;
        _mapper = mapper;
        _context = context;
        //_publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
         var auctions = await _context.Auctions.Include(x=> x.Item).OrderBy(x=> x.Item.Make).ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction =  _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefault(x => x.Id == id);

        if (auction == null) return NotFound();

        return  _mapper.Map<AuctionDto>(auction);
    }

    //[Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        var newAuction =new AuctionDto();
        try
        {
           

            auction.Seller = "Test";
            auction.Winner = string.Empty;
            _context.Auctions.Add(auction);

             newAuction = _mapper.Map<AuctionDto>(auction);

            //await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");
        }
        catch (Exception ex)
        {

            throw;
        }

        return CreatedAtAction(nameof(GetAuctionById),
            new { auction.Id }, newAuction);
    }

    //[Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions
            .Include(x=>x.Item)
            .FirstOrDefaultAsync(x=> x.Id == id);

        if (auction == null) return NotFound();

       auction.Seller = string.Empty;
        auction.Winner = string.Empty;

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;


        //await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }

    //[Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
           .Include(x => x.Item)
           .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        auction.Seller = string.Empty;
        auction.Winner = string.Empty;

        _context.Auctions.Remove(auction);

        // await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await _context.SaveChangesAsync() >0;

        if (!result) return BadRequest("Could not update DB");

        return Ok();
    }
}
