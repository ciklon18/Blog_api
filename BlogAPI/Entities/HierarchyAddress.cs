using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Entities;

[Table("hierarchy_address")]
public class HierarchyAddress
{
    [Key] 
    [Column("id")] 
    public long Id { get; set; }

    [Column("object_id")]
    public long ObjectId { get; set; }

    [Column("parent_obj_id")] 
    public long? ParentObjectId { get; set; }

    [Column("change_id")]
    public long? ChangeId { get; set; }

    [Column("path")] 
    public string? Path { get; set; }

    [Column("prev_id")]
    public long? PrevId { get; set; }

    [Column("next_id")] 
    public long? NextId { get; set; }

    [Column("region_code")]
    public string? RegionCode { get; set; }

    [Column("area_code")]
    public string? AreaCode { get; set; }

    [Column("city_code")]
    public string? CityCode { get; set; }

    [Column("place_code")]
    public string? PlaceCode { get; set; }

    [Column("plan_code")]
    public string? PlanCode { get; set; }

    [Column("street_code")]
    public string? StreetCode { get; set; }
    
    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public int? IsActive { get; set; }
}
