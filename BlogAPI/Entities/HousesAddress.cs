using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Entities;

[Table("houses_address")]
public class HousesAddress
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("object_id")]
    public long ObjectId { get; set; }

    [Column("object_guid")]
    public Guid ObjectGuid { get; set; }

    [Column("change_id")]
    public long ChangeId { get; set; }

    [Column("house_num")]
    public string? HouseNum { get; set; }

    [Column("add_num1")]
    public string? AddNum1 { get; set; }

    [Column("add_num2")]
    public string? AddNum2 { get; set; }

    [Column("house_type")]
    public int? HouseType { get; set; }

    [Column("addtype1")]
    public int? AddType1 { get; set; }

    [Column("addtype2")]
    public int? AddType2 { get; set; }

    [Column("oper_type_id")]
    public int? OperTypeId { get; set; }

    [Column("prev_id")]
    public long? PrevId { get; set; }

    [Column("next_id")]
    public long? NextId { get; set; }

    [Column("update_date")]
    public DateTime? UpdateDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_actual")]
    public int? IsActual { get; set; }

    [Column("is_active")]
    public int? IsActive { get; set; }
}
