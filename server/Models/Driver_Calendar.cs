using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace driver_api.Models;

[Table("Driver_Calendar", Schema = "Driver_services")]
[Keyless]
public class Driver_Calendar
{
    public DateTime CalendarDay { get; set; }
    public string CalendarWorking { get; set; }
    public DateTime cd { get; set; }
    public int CalendarID { get; set; }
}
