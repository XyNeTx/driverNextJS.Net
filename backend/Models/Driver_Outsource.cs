using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace driver_api.Models;

[Table("Driver_Outsource", Schema = "Driver_services")]
public class Driver_Outsource
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string EmployeeCode { get; set; }
    public DateTime Check_In { get; set; }
    public DateTime Check_Out { get; set; }
    public int Job_Type { get; set; }
    public string Temp_Drive { get; set; }
    public string Use_NoUse { get; set; }
    public DateTime Cal_Time_In { get; set; }
    public DateTime Cal_Time_Out { get; set; }
    public TimeSpan Work_OT1_5_Night { get; set; }
    public TimeSpan Work_Reg { get; set; }
    public TimeSpan Work_OT1_5_Eve { get; set; }
    public TimeSpan Work_OT2 { get; set; }
    public TimeSpan Work_Total_OT { get; set; }
    public TimeSpan Holi_OT3_0 { get; set; }
    public TimeSpan Holi_OT2_0 { get; set; }
    public TimeSpan Holi_OT3_0_Eve { get; set; }
    public TimeSpan Holi_Total_OT { get; set; }
    public TimeSpan All_Total_OT { get; set; }
    public short Taxi { get; set; }
    public short Lunch { get; set; }
}