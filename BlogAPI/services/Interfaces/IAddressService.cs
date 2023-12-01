using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface IAddressService
{
    List<SearchAddressResponse> Search(int parentObjectId, string? query);
    List<ChainAddressResponse> Chain(Guid objectGuid);
}