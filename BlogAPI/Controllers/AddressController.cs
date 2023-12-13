using BlogAPI.Models;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    
    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    [HttpGet("search")]
    public List<SearchAddressResponse> Search([FromQuery] int parentObjectId, [FromQuery] string? query)
    {
        var searchResponse =  _addressService.Search(parentObjectId, query);
        return searchResponse;
    }
    
    [HttpGet("chain")]
    public List<SearchAddressModel> Chain([FromQuery] Guid objectGuid)
    {
        var chainResponse = _addressService.Chain(objectGuid);
        return chainResponse;
    }
    
}