namespace driver_api.Models.ViewModels;

public class Formated_Driver_Outsource
{
    public int ID { get; set; }
    public string EmployeeCode { get; set; }
    public string Check_In { get; set; }
    public string Check_Out { get; set; }
    public int Job_Type { get; set; }
    public string Temp_Drive { get; set; }
    public string Use_NoUse { get; set; }
    public string Cal_Time_In { get; set; }
    public string Cal_Time_Out { get; set; }
    public string Work_OT1_5_Night { get; set; }
    public string Work_Reg { get; set; }
    public string Work_OT1_5_Eve { get; set; }
    public string Work_OT2 { get; set; }
    public string Work_Total_OT { get; set; }
    public string Holi_OT3_0 { get; set; }
    public string Holi_OT2_0 { get; set; }
    public string Holi_OT3_0_Eve { get; set; }
    public string Holi_Total_OT { get; set; }
    public string All_Total_OT { get; set; }
    public short Taxi { get; set; }
    public short Lunch { get; set; }
}
