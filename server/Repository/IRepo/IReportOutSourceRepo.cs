using driver_api.Models;
using driver_api.Models.ViewModels;

namespace driver_api.Repository.IRepo;

public interface IReportOutSourceRepo
{
    Task<List<Driver_Employee>> GetListDriverAsync();
    //public Task CalculateOutsourceReportAsync(string EmployeeCode, string Year, string Month);
    Task<List<Driver_Outsource>> GetReportDriverOutSourceAsync(VM_CalReport vM_CalReport);
    Task<VM_Total_Report_Outsource> SumCalculatedData(VM_CalReport vM_CalReport);
    Task<string> Authen(string brownserID, string brownserCurrent, string brownserDevices);
}
