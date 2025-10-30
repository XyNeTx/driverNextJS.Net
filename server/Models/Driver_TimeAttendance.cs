using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace driver_api.Models;

[Table("Driver_TimeAttendance", Schema = "Driver_services")]
public class Driver_TimeAttendance
{
    [Key]
    public int Time_Id { get; set; }
    public string? Time_Count { get; set; }
    public string? Time_EmployeeCode { get; set; }
    public string? Time_StatusIN { get; set; }
    public DateTime? Time_TodayIN { get; set; }
    public DateTime? cd { get; set; }
    public DateTime? ud { get; set; }
    public string? cb { get; set; }
    public string? ub { get; set; }
    public string? Time_latitudeIN { get; set; }
    public string? Time_longitudeIN { get; set; }
    public string? Status { get; set; }
    public string? Time_St { get; set; }
    public string? Time_HomeIN { get; set; }
    public string? Time_WaitOut { get; set; }
    public DateTime? Time_TodayOUT { get; set; }
    public string? Time_End { get; set; }
    public string? Time_StatusOUT { get; set; }
    public string? Time_HomeOUT { get; set; }
    public string? Time_latitudeOUT { get; set; }
    public string? Time_longitudeOUT { get; set; }
    public string? Time_ApprovedIN { get; set; }
    public string? Time_ApprovedOUT { get; set; }
    public DateTime? Time_TimeApprovedIN { get; set; }
    public DateTime? Time_TimeApprovedOUT { get; set; }
    public string? Time_WayIN { get; set; }
    public string? Time_WayOUT { get; set; }
    public string? Time_DriverId { get; set; }
    public string? Time_BossId { get; set; }
    public string? Time_NewLocationIN { get; set; }
    public string? Time_NewLocationOUT { get; set; }
    public string? Time_StatusDriver { get; set; }
    public string? Time_DDL_IN { get; set; }
    public string? Time_DDL_OUT { get; set; }
    public string? Time_Revise_IN_By { get; set; }
    public string? Time_Revise_OUT_By { get; set; }
    public string? Time_DriveInstead { get; set; }
    public string? Time_Working_D { get; set; }
    public string? Time_Workfromhome { get; set; }
    public string? Time_wfh_IN { get; set; }
    public string? Time_wfh_OUT { get; set; }
    public string? Time_Location_IN { get; set; }
    public string? Time_Location_OUT { get; set; }
    public string? Time_Driver_UpdateTime { get; set; }
    public string? Time_Note { get; set; }
    public string? Time_Overnight { get; set; }
    public string? Time_SystemApproved { get; set; }
    public DateTime? Time_SystemApprove_Date { get; set; }
}
