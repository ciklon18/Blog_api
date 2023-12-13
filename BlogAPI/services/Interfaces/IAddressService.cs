using BlogAPI.Models;
using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface IAddressService
{
    List<SearchAddressModel> Search(int parentObjectId, string? query);
    List<SearchAddressModel> Chain(Guid objectGuid);
}