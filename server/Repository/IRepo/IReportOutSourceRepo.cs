using driver_api.Models;

namespace driver_api.Repository.IRepo;

public interface IReportOutSourceRepo
{
    public Task<List<Driver_Employee>> GetListDriverAsync();
    public Task CalculateOutsourceReportAsync(string EmployeeCode, string Year, string Month);
}
