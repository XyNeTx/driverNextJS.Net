using driver_api.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace driver_api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReportOutSourceController : ControllerBase
{

    private readonly IReportOutSourceRepo _IReportOutSourceRepo;

    public ReportOutSourceController(IReportOutSourceRepo IReportOutSourceRepo)
    {
        _IReportOutSourceRepo = IReportOutSourceRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetDriverName()
    {
        try
        {
            var driverList = await _IReportOutSourceRepo.GetListDriverAsync();

            var data = driverList.Select(x => new
            {
                DriverName = x.Driver_name + " " + x.Driver_surname,
                ID = x.Driver_id,
                EmployeeCode = x.Driver_EmployeeCode
            }).ToList();

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetReportDriverOutSource([FromQuery] VM_CalReport vM)
    {
        try
        {
            var result = await _IReportOutSourceRepo.GetReportDriverOutSourceAsync(vM);

            var data = result.Select(x => new
            {
                x.ID,
                Check_In = x.Check_In.ToString("dd/MM/yyyy HH:mm"),
                Check_Out = x.Check_Out.ToString("dd/MM/yyyy HH:mm"),
                x.Job_Type,
                x.Temp_Drive,
                x.Use_NoUse,
                Cal_Time_In = x.Cal_Time_In.ToString("dd/MM/yyyy HH:mm"),
                Cal_Time_Out = x.Cal_Time_Out.ToString("dd/MM/yyyy HH:mm"),
                Work_OT1_5_Night = x.Work_OT1_5_Night.Hours.ToString("D2") + ":" +x.Work_OT1_5_Night.Minutes.ToString("D2"),
                Work_Reg = x.Work_Reg.Hours.ToString("D2") + ":" +x.Work_Reg.Minutes.ToString("D2"),
                Work_OT1_5_Eve = x.Work_OT1_5_Eve.Hours.ToString("D2") + ":" +x.Work_OT1_5_Eve.Minutes.ToString("D2"),
                Work_OT2 = x.Work_OT2.Hours.ToString("D2") + ":" +x.Work_OT2.Minutes.ToString("D2"),
                Work_Total_OT = x.Work_Total_OT.Hours.ToString("D2") + ":" +x.Work_Total_OT.Minutes.ToString("D2"),
                Holi_OT3_0 = x.Holi_OT3_0.Hours.ToString("D2") + ":" +x.Holi_OT3_0.Minutes.ToString("D2"),
                Holi_OT2_0 = x.Holi_OT2_0.Hours.ToString("D2") + ":" +x.Holi_OT2_0.Minutes.ToString("D2"),
                Holi_OT3_0_Eve = x.Holi_OT3_0_Eve.Hours.ToString("D2") + ":" +x.Holi_OT3_0_Eve.Minutes.ToString("D2"),
                Holi_Total_OT = x.Holi_Total_OT.Hours.ToString("D2") + ":" +x.Holi_Total_OT.Minutes.ToString("D2"),
                All_Total_OT = x.All_Total_OT.Hours.ToString("D2") + ":" +x.All_Total_OT.Minutes.ToString("D2"),
                x.Taxi,
                x.Lunch
            });

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}