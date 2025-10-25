using driver_api.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost]
    public async Task<IActionResult> CalculateOutsourceReport([FromBody] VM_CalReport request)
    {
        try
        {
            string EmployeeCode = request.EmployeeCode;
            string Year = request.Year;
            string Month = request.Month;

            await _IReportOutSourceRepo.CalculateOutsourceReportAsync(EmployeeCode, Year, Month);

            return Ok("Outsource report calculated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}