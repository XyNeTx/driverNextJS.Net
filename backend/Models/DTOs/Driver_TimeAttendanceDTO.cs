using Microsoft.EntityFrameworkCore;

namespace driver_api.Models.DTOs;

[Keyless]
public class Driver_TimeAttendanceDTO
{
    public int Time_Id { get; set; }
    public string EmployeeCode { get; set; }
    public string EmployeeFullName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string WorkTypeIn { get; set; }
    public string WorkTypeOut { get; set; }
    public string BossID { get; set; }
    public string BossFullName { get; set; }
}