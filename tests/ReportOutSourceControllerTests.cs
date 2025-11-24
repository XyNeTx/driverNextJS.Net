using driver_api.Controllers;
using driver_api.Models.DTOs;
using driver_api.Repository.IRepo;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace driver_test
{
    public class ReportOutSourceControllerTests
    {

        private readonly ILogger<ReportOutSourceController> logger;

        [Fact]
        public async Task GetReportDriverOutSourceTests()
        {
            var mockData = new List<DriverOT_DTOs> {
                new DriverOT_DTOs
                {
                    CheckInReal = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01T00:00:00")),
                    CheckOutReal= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01T00:00:00")),
                    CheckInCal = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01T00:00:00")),
                    CheckOutCal = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01T00:00:00")),
                    WorkingHours = new TimeSpan(0,0,0),
                    SumOT_1_5 = new TimeSpan(0,0,0),
                    SumOT_2_0 = new TimeSpan(0,0,0),
                    SumOT_3_0 = new TimeSpan(0,0,0),
                    SumOT = new TimeSpan(0,0,0),
                    Lunch= 0,
                    Taxi= 0
                },
            };

            var mockService = new Mock<IReportOutSourceRepo>();
            mockService.Setup(s => s.GetDriverOTTime("Sitthiporn Polmart")).ReturnsAsync(mockData);

            var controller = new ReportOutSourceController(mockService.Object,logger);

            var result = await controller.GetDriverOTTime();

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;

            var list = ok!.Value as List<DriverOT_DTOs>;

            list.Should().NotBeNull();
            list!.Should().BeEquivalentTo(mockData);

        }

        [Fact]
        public async Task DriverCheckOTTests()
        {
            var mockData = new List<DriverDTO> {
                new DriverDTO
                {
                    ID = 10,
                    DriverName = "NOOSIN SEISAI",
                    EmployeeCode = "20122757"
                },
                new DriverDTO
                {
                    ID = 12,
                    DriverName = "SOMCHATPONG PENGDAENG",
                    EmployeeCode = "BS000036"
                }
            };

            var mockService = new Mock<IReportOutSourceRepo>();
            mockService.Setup(s => s.GetListDriverAsync()).ReturnsAsync(mockData);

            var controller = new ReportOutSourceController(mockService.Object,logger);

            var result = await controller.GetDriverName();

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;

            var list = ok!.Value as List<DriverDTO>;

            list.Should().NotBeNull();
            list![0].ID.Should().Be(10);
            list![1].ID.Should().Be(12);

        }

    }
}