using BlogAPI.Enums;

namespace BlogAPI.Utils;

public static class AddressConverterUtil
{

    public static ObjectLevel GetObjectLevelWithInt(int objectLevel)
    {
        return objectLevel switch
        {
            1 => ObjectLevel.Region,
            2 => ObjectLevel.AdministrativeArea,
            3 => ObjectLevel.MunicipalArea,
            4 => ObjectLevel.RuralUrbanSettlement,
            5 => ObjectLevel.City,
            6 => ObjectLevel.Locality,
            7 => ObjectLevel.ElementOfPlanningStructure,
            8 => ObjectLevel.ElementOfRoadNetwork,
            9 => ObjectLevel.Land,
            10 => ObjectLevel.Building,
            11 => ObjectLevel.Room,
            12 => ObjectLevel.RoomInRooms,
            13 => ObjectLevel.AutonomousRegionLevel,
            14 => ObjectLevel.IntracityLevel,
            15 => ObjectLevel.AdditionalTerritoriesLevel,
            16 => ObjectLevel.LevelOfObjectsInAdditionalTerritories,
            17 => ObjectLevel.CarPlace,
            _ => throw new ArgumentOutOfRangeException(nameof(objectLevel), objectLevel, null)
        };
    }
    public static string GetObjectLevelTextWithInt(int objectLevel)
    {
        return objectLevel switch
        {
            1 => "Субъект РФ",
            2 => "Административный район",
            3 => "Муниципальный район",
            4 => "Сельско-городская территория",
            5 => "Город",
            6 => "Населенный пункт",
            7 => "Элемент планировочной структуры",
            8 => "Элемент улично-дорожной сети",
            9 => "Земельный участок",
            10 => "Здание",
            11 => "Помещение",
            12 => "Помещение в помещении",
            13 => "Автономный округ",
            14 => "Внутригородской уровень",
            15 => "Дополнительные территории",
            16 => "Уровень объектов на дополнительных территориях",
            17 => "Машиноместо",
            _ => ""
        };
    }
    public static string GetObjectLevelTextWithObjectLevel(ObjectLevel objectLevel)
    {
        return objectLevel switch
        {
            ObjectLevel.Region => "Регион",
            ObjectLevel.AdministrativeArea => "Административный район",
            ObjectLevel.MunicipalArea => "Муниципальный район",
            ObjectLevel.RuralUrbanSettlement => "Сельско-городская территория",
            ObjectLevel.City => "Город",
            ObjectLevel.Locality => "Населенный пункт",
            ObjectLevel.ElementOfPlanningStructure => "Элемент планировочной структуры",
            ObjectLevel.ElementOfRoadNetwork => "Элемент дорожной сети",
            ObjectLevel.Land => "Земельный участок",
            ObjectLevel.Building => "Здание (сооружение)",
            ObjectLevel.Room => "Помещение",
            ObjectLevel.RoomInRooms => "Помещение в помещении",
            ObjectLevel.AutonomousRegionLevel => "Автономный округ",
            ObjectLevel.IntracityLevel => "Внутригородской уровень",
            ObjectLevel.AdditionalTerritoriesLevel => "Дополнительные территории",
            ObjectLevel.LevelOfObjectsInAdditionalTerritories => "Уровень объектов на дополнительных территориях",
            ObjectLevel.CarPlace => "Машиноместо",
            _ => ""
        };
    }

    public static string GetHouseTypeStringWithInt(int? houseType)
    {
        var result = houseType switch
        {
            1 => "Владение",
            2 => "Здание (сооружение)",
            3 => "Дом",
            4 => "Гараж",
            5 => "Здание",
            6 => "Шахта",
            7 => "Строение",
            8 => "Сооружение",
            9 => "Литера",
            10 => "Корпус",
            11 => "Подвал",
            12 => "Котельная",
            13 => "Погреб",
            14 => "Объект незавершенного строительства",
            _ => ""
        };
        return result;
    }
}