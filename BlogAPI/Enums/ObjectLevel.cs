using System.Text.Json.Serialization;

namespace BlogAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ObjectLevel
{
    Region,
    AdministrativeArea,
    MunicipalArea,
    RuralUrbanSettlement,
    City,
    Locality,
    ElementOfPlanningStructure,
    ElementOfRoadNetwork,
    Land,
    Building,
    Room,
    RoomInRooms,
    AutonomousRegionLevel,
    IntracityLevel,
    AdditionalTerritoriesLevel,
    LevelOfObjectsInAdditionalTerritories,
    CarPlace,
    Empty
    
}