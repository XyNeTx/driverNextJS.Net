using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace driver_api.Models;

[Table("Boss",Schema = "Driver_services")]
[PrimaryKey("Boss_Id")]
public class Boss
{
	public int Boss_Id { get; set; }
	public string Boss_EmployeeCode { get; set; }
	public string? Boss_Name { get; set; }
	public string? Boss_Sername { get; set; }
	public string? Boss_Factory { get; set; }
	public string? Boss_status { get; set; }
	public DateTime? Boss_cd { get; set; }
	public DateTime? Boss_ud { get; set; }
	public string? Boss_cb { get; set; }
	public string? Boss_ub { get; set; }
	public string? Boss_Position { get; set; }
	public string? Boss_Address { get; set; }
	public string? Boss_longitude { get; set; }
	public string? Boss_latitude { get; set; }
	public string? Boss_image { get; set; }
	public string? Boss_loginName { get; set; }
	public string? Boss_loginPass { get; set; }
	public string? Boss_Phone { get; set; }
	public string? Boss_TimeSt { get; set; }
	public string? Boss_TimeEnd { get; set; }
	public string? Boss_HomeName { get; set; }
	public string? Boss_MaxKilometr { get; set; }
	public string? Boss_Email { get; set; }
	public string? Boss_Country { get; set; }
	public string? Boss_Company { get; set; }
	public string? Card_Number { get; set; }

    //public ICollection<Driver_Employee> Driver_Employees { get; set; }
}
