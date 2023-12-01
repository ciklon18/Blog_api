using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using BlogAPI.Utils;

namespace BlogAPI.services.Impl;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _db;

    public AddressService(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<SearchAddressResponse> Search(int parentObjectId, string? query)
    {
        var searchHierarchyList = GetHierarchyAddressList(parentObjectId);

        var responseList = ConvertAddressListToAddressResponseList(searchHierarchyList);
        responseList = GetAddressesResponseList(responseList);
        responseList = GetAddressesAndHousesResponseList(responseList);
        responseList = query != null ? GetFilteredAddressedByQuery(responseList, query) : responseList;

        return responseList;
    }

    public List<ChainAddressResponse> Chain(Guid objectGuid)
    {
        var objectId = GetElementByGuid(objectGuid);
        return objectId switch
        {
            Address address => GetChainResponseList(address.ObjectId),
            HousesAddress house => GetChainResponseList(house.ObjectId),
            _ => throw new AddressElementNotFound("Address element not found")
        };
    }

    private List<ChainAddressResponse> GetChainResponseList(long houseIdObjectId)
    {
        var path = GetPath(houseIdObjectId);
        var pathItems = GetPathItems(path);
        var chainResponseList = new List<ChainAddressResponse>(pathItems.Count);

        foreach (var objectId in pathItems)
        {
            var addressItem = GetAddressByObjectId(objectId);
            if (addressItem != null)
            {
                chainResponseList.Add(ConvertAddressToChainResponse(addressItem));
                continue;
            }
            var houseItem = GetHouseByObjectId(objectId);
            if (houseItem != null) chainResponseList.Add(ConvertHouseToChainResponse(houseItem));
        }

        return chainResponseList;
    }


    private string GetPath(long objectId)
    {
        return _db.HierarchyAddresses.FirstOrDefault(address => address.ObjectId == objectId)?.Path ?? "";
    }

    private object? GetElementByGuid(Guid objectGuid)
    {
        var addressItem = GetAddressByObjectId(objectGuid);
        if (addressItem != null) return addressItem;
        var houseItem = GetHouseByObjectId(objectGuid);
        return houseItem;
    }

    private Address? GetAddressByObjectId(long objectId)
    {
        return _db.Addresses
            .Where(address => address.ObjectId == objectId)
            .OrderByDescending(address => address.StartDate)
            .FirstOrDefault();
    }

    private HousesAddress? GetHouseByObjectId(long objectId)
    {
        return _db.HousesAddresses
            .Where(address => address.ObjectId == objectId)
            .OrderByDescending(address => address.StartDate)
            .FirstOrDefault();
    }


    private Address? GetAddressByObjectId(Guid objectGuid)
    {
        return _db.Addresses
            .Where(address => address.ObjectGuid == objectGuid)
            .OrderByDescending(address => address.StartDate)
            .FirstOrDefault();
    }

    private HousesAddress? GetHouseByObjectId(Guid objectGuid)
    {
        return _db.HousesAddresses
            .Where(address => address.ObjectGuid == objectGuid)
            .OrderByDescending(address => address.StartDate)
            .FirstOrDefault();
    }


    private static List<SearchAddressResponse> GetFilteredAddressedByQuery(List<SearchAddressResponse> addressedList,
        string query)
    {
        return addressedList
            .Where(address => address.Text != null && address.Text.Contains(query))
            .ToList();
    }

    private List<SearchAddressResponse> GetAddressesResponseList(List<SearchAddressResponse> responseList)
    {
        var updatedResponseList = new List<SearchAddressResponse>(responseList.Count);

        foreach (var searchAddressResponse in responseList)
        {
            var addressItem = _db.Addresses
                .Where(address => address.ObjectId == searchAddressResponse.ObjectId)
                .OrderByDescending(address => address.StartDate)
                .FirstOrDefault();
            updatedResponseList.Add(addressItem != null
                ? GetAddressListItem(searchAddressResponse, addressItem)
                : searchAddressResponse);
        }

        return updatedResponseList;
    }

    private List<SearchAddressResponse> GetAddressesAndHousesResponseList(List<SearchAddressResponse> responseList)
    {
        var updatedResponseList = new List<SearchAddressResponse>(responseList.Count);

        foreach (var searchAddressResponse in responseList)
        {
            var addressItem = GetHouseByObjectId(searchAddressResponse.ObjectId);
            updatedResponseList.Add(addressItem != null
                ? GetHouseListItem(searchAddressResponse, addressItem)
                : searchAddressResponse);
        }

        return updatedResponseList;
    }

    private static SearchAddressResponse GetAddressListItem(SearchAddressResponse searchAddressResponse,
        Address addressItem)
    {
        return searchAddressResponse with
        {
            ObjectGuid = addressItem.ObjectGuid,
            Text = $"{addressItem.TypeName} {addressItem.Name}",
            ObjectLevel = AddressConverterUtil.GetObjectLevelWithInt(addressItem.Level).ToString(),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithInt(addressItem.Level)
        };
    }

    private static SearchAddressResponse GetHouseListItem(SearchAddressResponse searchAddressResponse,
        HousesAddress addressItem)
    {
        return searchAddressResponse with
        {
            ObjectGuid = addressItem.ObjectGuid,
            Text = $"{addressItem.HouseNum}",
            ObjectLevel = ObjectLevel.Building.ToString(),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithObjectLevel(ObjectLevel.Building)
        };
    }


    private List<HierarchyAddress> GetHierarchyAddressList(int parentObjectId)
    {
        return _db.HierarchyAddresses
            .Where(hierarchyAddress => hierarchyAddress.ParentObjectId == parentObjectId)
            .ToList();
    }


    private List<long> GetPathItems(string path)
    {
        return path.Split(".")
            .Where(item => item != "")
            .Select(long.Parse)
            .ToList();
    }

    private static List<SearchAddressResponse> ConvertAddressListToAddressResponseList(
        IEnumerable<HierarchyAddress> searchHierarchyResult)
    {
        return searchHierarchyResult
            .Select(address => new SearchAddressResponse
            {
                ObjectId = address.ObjectId,
                ObjectLevel = "",
                ObjectLevelText = "",
                Text = ""
            })
            .ToList();
    }

    private static ChainAddressResponse ConvertAddressToChainResponse(Address address)
    {
        return new ChainAddressResponse
        {
            ObjectId = address.ObjectId,
            ObjectGuid = address.ObjectGuid.ToString(),
            Text = $"{address.TypeName} {address.Name}",
            ObjectLevel = AddressConverterUtil.GetObjectLevelWithInt(address.Level).ToString(),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithInt(address.Level)
        };
    }

    private static ChainAddressResponse ConvertHouseToChainResponse(HousesAddress house)
    {
        return new ChainAddressResponse
        {
            ObjectId = house.ObjectId,
            ObjectGuid = house.ObjectGuid.ToString(),
            Text = $"{house.HouseNum}",
            ObjectLevel = ObjectLevel.Building.ToString(),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithObjectLevel(ObjectLevel.Building)
        };
    }
}