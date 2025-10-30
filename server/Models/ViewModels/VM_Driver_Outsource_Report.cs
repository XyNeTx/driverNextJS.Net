namespace driver_api.Models.ViewModels;

public class VM_Driver_Outsource_Report
{
    public List<Formated_Driver_Outsource> ReportList { get; set; }
    public VM_Total_Report_Outsource ReferReport { get; set; }
}
