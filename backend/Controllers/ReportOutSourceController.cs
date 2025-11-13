using driver_api.Models.ViewModels;
using driver_api.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace driver_api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReportOutSourceController : ControllerBase
{

    private readonly IReportOutSourceRepo _IReportOutSourceRepo;
    private readonly ILogger<ReportOutSourceController> _logger;

    public ReportOutSourceController(IReportOutSourceRepo IReportOutSourceRepo, ILogger<ReportOutSourceController> logger)
    {
        _IReportOutSourceRepo = IReportOutSourceRepo;
        _logger = logger;
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
    public async Task<IActionResult> GetReportDriverOutSource([FromQuery] string EmployeeCode, string Month, string Year)
    {
        try
        {
            VM_CalReport vM = new VM_CalReport
            {
                EmployeeCode = EmployeeCode,
                Month = Month,
                Year = Year
            };
            var result = await _IReportOutSourceRepo.GetReportDriverOutSourceAsync(vM);
            var referResult = await _IReportOutSourceRepo.SumCalculatedData(vM);
            var formattedResult = result.Select(x => new Formated_Driver_Outsource
            {
                ID = x.ID,
                Check_In = x.Check_In.ToString("dd/MM/yyyy HH:mm"),
                Check_Out = x.Check_Out.ToString("dd/MM/yyyy HH:mm"),
                Job_Type = x.Job_Type,
                Temp_Drive = x.Temp_Drive,
                Use_NoUse = x.Use_NoUse,
                Cal_Time_In = x.Cal_Time_In.ToString("dd/MM/yyyy HH:mm"),
                Cal_Time_Out = x.Cal_Time_Out.ToString("dd/MM/yyyy HH:mm"),
                Work_OT1_5_Night = x.Work_OT1_5_Night.Hours.ToString("D2") + ":" + x.Work_OT1_5_Night.Minutes.ToString("D2"),
                Work_Reg = x.Work_Reg.Hours.ToString("D2") + ":" + x.Work_Reg.Minutes.ToString("D2"),
                Work_OT1_5_Eve = x.Work_OT1_5_Eve.Hours.ToString("D2") + ":" + x.Work_OT1_5_Eve.Minutes.ToString("D2"),
                Work_OT2 = x.Work_OT2.Hours.ToString("D2") + ":" + x.Work_OT2.Minutes.ToString("D2"),
                Work_Total_OT = x.Work_Total_OT.Hours.ToString("D2") + ":" + x.Work_Total_OT.Minutes.ToString("D2"),
                Holi_OT3_0 = x.Holi_OT3_0.Hours.ToString("D2") + ":" + x.Holi_OT3_0.Minutes.ToString("D2"),
                Holi_OT2_0 = x.Holi_OT2_0.Hours.ToString("D2") + ":" + x.Holi_OT2_0.Minutes.ToString("D2"),
                Holi_OT3_0_Eve = x.Holi_OT3_0_Eve.Hours.ToString("D2") + ":" + x.Holi_OT3_0_Eve.Minutes.ToString("D2"),
                Holi_Total_OT = x.Holi_Total_OT.Hours.ToString("D2") + ":" + x.Holi_Total_OT.Minutes.ToString("D2"),
                All_Total_OT = x.All_Total_OT.Hours.ToString("D2") + ":" + x.All_Total_OT.Minutes.ToString("D2"),
                Taxi = x.Taxi,
                Lunch = x.Lunch
            }).ToList();

            var data = new VM_Driver_Outsource_Report
            {
                ReportList = formattedResult,
                ReferReport = referResult
            };

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> RefreshReportDriverOutSource([FromQuery] string EmployeeCode, string Month, string Year)
    {
        try
        {
            VM_CalReport vM = new VM_CalReport
            {
                EmployeeCode = EmployeeCode,
                Month = Month,
                Year = Year
            };
            var result = await _IReportOutSourceRepo.RefreshReportDriverOutSourceAsync(vM);
            var referResult = await _IReportOutSourceRepo.SumCalculatedData(vM);
            var formattedResult = result.Select(x => new Formated_Driver_Outsource
            {
                ID = x.ID,
                Check_In = x.Check_In.ToString("dd/MM/yyyy HH:mm"),
                Check_Out = x.Check_Out.ToString("dd/MM/yyyy HH:mm"),
                Job_Type = x.Job_Type,
                Temp_Drive = x.Temp_Drive,
                Use_NoUse = x.Use_NoUse,
                Cal_Time_In = x.Cal_Time_In.ToString("dd/MM/yyyy HH:mm"),
                Cal_Time_Out = x.Cal_Time_Out.ToString("dd/MM/yyyy HH:mm"),
                Work_OT1_5_Night = x.Work_OT1_5_Night.Hours.ToString("D2") + ":" + x.Work_OT1_5_Night.Minutes.ToString("D2"),
                Work_Reg = x.Work_Reg.Hours.ToString("D2") + ":" + x.Work_Reg.Minutes.ToString("D2"),
                Work_OT1_5_Eve = x.Work_OT1_5_Eve.Hours.ToString("D2") + ":" + x.Work_OT1_5_Eve.Minutes.ToString("D2"),
                Work_OT2 = x.Work_OT2.Hours.ToString("D2") + ":" + x.Work_OT2.Minutes.ToString("D2"),
                Work_Total_OT = x.Work_Total_OT.Hours.ToString("D2") + ":" + x.Work_Total_OT.Minutes.ToString("D2"),
                Holi_OT3_0 = x.Holi_OT3_0.Hours.ToString("D2") + ":" + x.Holi_OT3_0.Minutes.ToString("D2"),
                Holi_OT2_0 = x.Holi_OT2_0.Hours.ToString("D2") + ":" + x.Holi_OT2_0.Minutes.ToString("D2"),
                Holi_OT3_0_Eve = x.Holi_OT3_0_Eve.Hours.ToString("D2") + ":" + x.Holi_OT3_0_Eve.Minutes.ToString("D2"),
                Holi_Total_OT = x.Holi_Total_OT.Hours.ToString("D2") + ":" + x.Holi_Total_OT.Minutes.ToString("D2"),
                All_Total_OT = x.All_Total_OT.Hours.ToString("D2") + ":" + x.All_Total_OT.Minutes.ToString("D2"),
                Taxi = x.Taxi,
                Lunch = x.Lunch
            }).ToList();

            var data = new VM_Driver_Outsource_Report
            {
                ReportList = formattedResult,
                ReferReport = referResult
            };

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Authen()
    {
        try
        {
            var BROWSERCURRENT = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERCURRENT").Value;
            var BROWSERDEVICES = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERDEVICES").Value;
            var BROWSERID = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERID").Value;
            var UserName = await _IReportOutSourceRepo.Authen(BROWSERID, BROWSERCURRENT, BROWSERDEVICES);

            _logger.LogInformation("Login Authen Complete {UserName}", UserName);
            return Ok(UserName);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAttendanceWaitingData()
    {
        try
        {
            var data = await _IReportOutSourceRepo.GetAllAttendanceWaitingData();
            //_logger.LogInformation("Test Log {data}", JsonConvert.SerializeObject(data));
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAllWaitingData(List<int> unApproveList)
    {
        try
        {
            var BROWSERCURRENT = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERCURRENT").Value;
            var BROWSERDEVICES = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERDEVICES").Value;
            var BROWSERID = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "BROWSERID").Value;
            var UserName = await _IReportOutSourceRepo.Authen(BROWSERID, BROWSERCURRENT, BROWSERDEVICES);

            await _IReportOutSourceRepo.ApproveAllData(unApproveList, UserName);

            _logger.LogInformation("{UserName} was Approve all Waiting data", UserName);
            
            return Ok("Approved All Data Complete !");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}