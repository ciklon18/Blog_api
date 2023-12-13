using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models;
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

    public List<SearchAddressModel> Search(int parentObjectId, string? query)
    {
        var searchHierarchyList = GetHierarchyAddressList(parentObjectId);

        var responseList = ConvertAddressListToAddressModelList(searchHierarchyList);
        responseList = GetAddressesModelList(responseList);
        responseList = GetAddressesAndHousesModelList(responseList);
        responseList = query != null ? GetFilteredAddressedByQuery(responseList, query) : responseList;

        return responseList;
    }

    public List<SearchAddressModel> Chain(Guid objectGuid)
    {
        var objectId = GetElementByGuid(objectGuid);
        return objectId switch
        {
            Address address => GetAddressModelList(address.ObjectId),
            HousesAddress house => GetAddressModelList(house.ObjectId),
            _ => throw new AddressElementNotFound("Address element not found")
        };
    }

    private List<SearchAddressModel> GetAddressModelList(long houseIdObjectId)
    {
        var path = GetPath(houseIdObjectId);
        var pathItems = GetPathItems(path);
        var chainResponseList = new List<SearchAddressModel>(pathItems.Count);

        foreach (var objectId in pathItems)
        {
            var addressItem = GetAddressByObjectId(objectId);
            if (addressItem != null)
            {
                chainResponseList.Add(ConvertAddressToAddressModel(addressItem));
                continue;
            }
            var houseItem = GetHouseByObjectId(objectId);
            if (houseItem != null) chainResponseList.Add(ConvertHouseToAddressModel(houseItem));
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


    private static List<SearchAddressModel> GetFilteredAddressedByQuery(IEnumerable<SearchAddressModel> addressedList,
        string query)
    {
        return addressedList
            .Where(address => address.Text != null && address.Text.Contains(query))
            .ToList();
    }

    private List<SearchAddressModel> GetAddressesModelList(List<SearchAddressModel> responseList)
    {
        var updatedResponseList = new List<SearchAddressModel>(responseList.Count);

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

    private List<SearchAddressModel> GetAddressesAndHousesModelList(List<SearchAddressModel> responseList)
    {
        var updatedResponseList = new List<SearchAddressModel>(responseList.Count);

        foreach (var searchAddressResponse in responseList)
        {
            var addressItem = GetHouseByObjectId(searchAddressResponse.ObjectId);
            updatedResponseList.Add(addressItem != null
                ? GetHouseListItem(searchAddressResponse, addressItem)
                : searchAddressResponse);
        }

        return updatedResponseList;
    }

    private static SearchAddressModel GetAddressListItem(SearchAddressModel searchAddressResponse,
        Address addressItem)
    {
        return searchAddressResponse with
        {
            ObjectGuid = addressItem.ObjectGuid,
            Text = $"{addressItem.TypeName} {addressItem.Name}",
            ObjectLevel = AddressConverterUtil.GetObjectLevelWithInt(addressItem.Level),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithInt(addressItem.Level)
        };
    }

    private static SearchAddressModel GetHouseListItem(SearchAddressModel searchAddressResponse,
        HousesAddress addressItem)
    {
        return searchAddressResponse with
        {
            ObjectGuid = addressItem.ObjectGuid,
            Text = $"{addressItem.HouseNum}",
            ObjectLevel = ObjectLevel.Building,
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithObjectLevel(ObjectLevel.Building)
        };
    }


    private IEnumerable<HierarchyAddress> GetHierarchyAddressList(int parentObjectId)
    {
        return _db.HierarchyAddresses
            .Where(hierarchyAddress => hierarchyAddress.ParentObjectId == parentObjectId)
            .ToList();
    }


    private static List<long> GetPathItems(string path)
    {
        return path.Split(".")
            .Where(item => item != "")
            .Select(long.Parse)
            .ToList();
    }

    private static List<SearchAddressModel> ConvertAddressListToAddressModelList(
        IEnumerable<HierarchyAddress> searchHierarchyResult)
    {
        return searchHierarchyResult
            .Select(address => new SearchAddressModel
            {
                ObjectId = address.ObjectId,
                ObjectLevel = ObjectLevel.Empty,
                ObjectLevelText = "",
                Text = ""
            })
            .ToList();
    }

    private static SearchAddressModel ConvertAddressToAddressModel(Address address)
    {
        return new SearchAddressModel
        {
            ObjectId = address.ObjectId,
            ObjectGuid = address.ObjectGuid,
            Text = $"{address.TypeName} {address.Name}",
            ObjectLevel = AddressConverterUtil.GetObjectLevelWithInt(address.Level),
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithInt(address.Level)
        };
    }

    private static SearchAddressModel ConvertHouseToAddressModel(HousesAddress house)
    {
        return new SearchAddressModel
        {
            ObjectId = house.ObjectId,
            ObjectGuid = house.ObjectGuid,
            Text = $"{house.HouseNum}",
            ObjectLevel = ObjectLevel.Building,
            ObjectLevelText = AddressConverterUtil.GetObjectLevelTextWithObjectLevel(ObjectLevel.Building)
        };
    }
}