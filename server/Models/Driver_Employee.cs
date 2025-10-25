
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace driver_api.Models;

[Table("Driver_Employee", Schema = "Driver_services")]
public class Driver_Employee
{
    [Key]
    public int Driver_id { get; set; }
    public string? Driver_name { get; set; }
    public string? Driver_surname { get; set; }
    public string? Driver_Position { get; set; }
    public string? Driver_Vendor { get; set; }
    public string? Driver_fac { get; set; }
    public string? Driver_img { get; set; }
    public DateTime? Driver_cd { get; set; }
    public DateTime? Driver_ud { get; set; }
    public string? Driver_cb { get; set; }
    public string? Driver_ub { get; set; }
    public string? Driver_LoginName { get; set; }
    public string? Driver_LoginPass { get; set; }
    public string? Driver_Code { get; set; }
    public DateTime? Driver_LastLogin { get; set; }
    public string? Driver_Phone { get; set; }
    public string? Driver_Address { get; set; }
    public string? Driver_Boss { get; set; }
    public string Driver_EmployeeCode { get; set; }
    public string? Driver_Status { get; set; }
    public string? Driver_NameTH { get; set; }
    public string? Driver_SurnameTH { get; set; }
    public string? Driver_Email { get; set; }

}