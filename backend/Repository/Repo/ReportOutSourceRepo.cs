using ClosedXML.Excel;
using driver_api.Models;
using driver_api.Models.DTOs;
using driver_api.Models.ViewModels;
using driver_api.Repository.IRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;

namespace driver_api.Repository.Repo;

public class ReportOutSourceRepo : IReportOutSourceRepo
{
    private readonly WorkflowContext _wfContext;
    private readonly ILogger<ReportOutSourceRepo> _logger;

    public ReportOutSourceRepo(WorkflowContext wfContext,ILogger<ReportOutSourceRepo> logger)
    {
        _wfContext = wfContext;
        _logger = logger;
    }

    public async Task<List<DriverDTO>> GetListDriverAsync()
    {
        try
        {
            var driverList = await _wfContext.Driver_Employee.AsNoTracking()
                .Where(x => x.Driver_Position!.ToLower() == "driver"
                    && x.Driver_Status == "1")
                .OrderBy(x=>x.Driver_name)
                .ToListAsync();

            var data = driverList.Select(x => new DriverDTO
            {
                DriverName = x.Driver_name + " " + x.Driver_surname,
                ID = x.Driver_id,
                EmployeeCode = x.Driver_EmployeeCode
            }).ToList();

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error retrieving driver list");
            //_logger.LogError("{ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

    public async Task<List<Driver_Outsource>> GetReportDriverOutSourceAsync(VM_CalReport vM_CalReport)
    {
        try
        {
            var yearMonth = DateTime.ParseExact(vM_CalReport.Year + vM_CalReport.Month, "yyyyMM", null);
            var data = await _wfContext.Driver_Outsource.AsNoTracking()
                .Where(x => x.Check_In.Month == yearMonth.Month
                && x.Check_In.Year == yearMonth.Year
                && x.EmployeeCode == vM_CalReport.EmployeeCode).ToListAsync();

            if (data.Count > 0)
            {
                return data;
            }
            else
            {
                data = await CalculateOutsourceReportAsync(vM_CalReport.EmployeeCode, vM_CalReport.Year, vM_CalReport.Month);
            }
            return data;
            //}

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Cant get Report Driver Outsource");
            //_logger.LogError("{ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

    public async Task<List<Driver_Outsource>> RefreshReportDriverOutSourceAsync(VM_CalReport vM_CalReport)
    {
        try
        {
            var yearMonth = DateTime.ParseExact(vM_CalReport.Year + vM_CalReport.Month, "yyyyMM", null);
            var data = await CalculateOutsourceReportAsync(vM_CalReport.EmployeeCode, vM_CalReport.Year, vM_CalReport.Month);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Cant get Report Driver Outsource");
            throw;
        }
    }

    private async Task<List<Driver_Outsource>> CalculateOutsourceReportAsync(string EmployeeCode, string Year, string Month)
    {
        var ts00_00 = new TimeSpan(0, 0, 0);
        var ts22_00 = new TimeSpan(22, 0, 0);
        var ts22_30 = new TimeSpan(22, 30, 0);
        var ts23_30 = new TimeSpan(23, 30, 0);
        var ts05_30 = new TimeSpan(5, 30, 0);
        var ts07_30 = new TimeSpan(7, 30, 0);
        var ts16_30 = new TimeSpan(16, 30, 0);

        var dateIn = new DateTime();
        var dateOut = new DateTime();

        try
        {
            DateTime yearMonth = DateTime.ParseExact(Year + Month, "yyyyMM", null);
            var attendanceData = await _wfContext.Driver_TimeAttendance.AsNoTracking()
                .Where(x => x.Time_EmployeeCode == EmployeeCode &&
                x.Time_TodayIN.Value.Year == yearMonth.Year &&
                x.Time_TodayIN.Value.Month == yearMonth.Month &&
                x.Time_StatusDriver == "End")
                .ToListAsync();

            var calendarData = await _wfContext.Driver_Calendar.AsNoTracking()
                .Where(x => x.CalendarDay.Year == yearMonth.Year &&
                x.CalendarDay.Month == yearMonth.Month)
                .ToListAsync();

            await _wfContext.Driver_Outsource
                .Where(x => x.EmployeeCode == EmployeeCode &&
                x.Check_In.Year == yearMonth.Year &&
                x.Check_In.Month == yearMonth.Month)
                .ExecuteDeleteAsync();

            var addList = new List<Driver_Outsource>();

            foreach (var date in calendarData)
            {
                var workList = attendanceData.Where(x => x.Time_TodayIN.Value.Date == date.CalendarDay.Date).ToList();

                if (workList.Count > 0)
                {
                    Driver_TimeAttendance each = new Driver_TimeAttendance();
                    if (workList.Count >= 2)
                    {
                        each = workList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Time_Driver_UpdateTime));
                        if(each == null)
                        {
                            each = workList.OrderByDescending(x=>x.Time_Id).FirstOrDefault();
                        }
                    }
                    else
                    {
                        each = workList.FirstOrDefault();
                    }
                    each.Time_DDL_IN = each.Time_DDL_IN.Replace(":", ".");
                    each.Time_DDL_OUT = each.Time_DDL_OUT.Replace(":", ".");

                    var TimeDdlIn = each.Time_DDL_IN.Split(".")[0].Length == 1 ? "0" + each.Time_DDL_IN : each.Time_DDL_IN;
                    var TimeDdlOut = each.Time_DDL_OUT.Split(".")[0].Length == 1 ? "0" + each.Time_DDL_OUT : each.Time_DDL_OUT;

                    var CheckIn = DateTime.ParseExact(each.Time_TodayIN?.ToString("yyyy-MM-dd") + " " + TimeDdlIn + ":00", "yyyy-MM-dd HH.mm:ss", null);
                    var CheckOut = DateTime.ParseExact(each.Time_TodayOUT?.ToString("yyyy-MM-dd") + " " + TimeDdlOut + ":00", "yyyy-MM-dd HH.mm:ss", null);

                    var tsDdlIN = new TimeSpan(int.Parse(each.Time_DDL_IN.Split(".")[0]), int.Parse(each.Time_DDL_IN.Split(".")[1]), 0);
                    var tsDdlOUT = new TimeSpan(int.Parse(each.Time_DDL_OUT.Split(".")[0]), int.Parse(each.Time_DDL_OUT.Split(".")[1]), 0);

                    var tsCalIn = tsDdlIN;
                    var tsCalOut = tsDdlOUT;
                    var strCal_Time_In = tsCalIn.Hours.ToString("D2") + ":" + tsCalIn.Minutes.ToString("D2") + ":00";
                    var strCal_Time_Out = tsCalOut.Hours.ToString("D2") + ":" + tsCalOut.Minutes.ToString("D2") + ":00";
                    var taxi = 0;
                    var lunch = 0;

                    // if (each.Time_wfh_IN == "1" || !string.IsNullOrWhiteSpace(each.Time_DriveInstead))
                    // {
                    //     tsCalIn = tsCalIn - new TimeSpan(0, 30, 0);
                    //     strCal_Time_In = tsCalIn.Hours.ToString("D2") + ":" + tsCalIn.Minutes.ToString("D2") + ":00";
                    //     if (tsCalIn >= ts00_00 && tsCalIn <= ts05_30)
                    //     {
                    //         taxi++;
                    //     }
                    // }
                    // if (each.Time_wfh_OUT == "1" || !string.IsNullOrWhiteSpace(each.Time_DriveInstead))
                    // {
                    //     tsCalOut = tsCalOut + new TimeSpan(0, 15, 0);
                    //     strCal_Time_Out = tsCalOut.Hours.ToString("D2") + ":" + tsCalOut.Minutes.ToString("D2") + ":00";
                    //     if (tsCalOut >= ts22_30 || tsCalOut <= ts05_30)
                    //     {
                    //         taxi++;
                    //     }
                    // }

                    if (each.Time_wfh_IN == "1" || !string.IsNullOrWhiteSpace(each.Time_DriveInstead))
                    {
                        tsCalIn = tsCalIn - new TimeSpan(0, 30, 0);
                    }
                    strCal_Time_In = tsCalIn.Hours.ToString("D2") + ":" + tsCalIn.Minutes.ToString("D2") + ":00";
                    if (tsCalIn >= ts00_00 && tsCalIn <= ts05_30)
                    {
                        taxi++;
                    }

                    if (each.Time_wfh_OUT == "1" || !string.IsNullOrWhiteSpace(each.Time_DriveInstead))
                    {
                        tsCalOut = tsCalOut + new TimeSpan(0, 15, 0);
                    }
                    strCal_Time_Out = tsCalOut.Hours.ToString("D2") + ":" + tsCalOut.Minutes.ToString("D2") + ":00";
                    if (tsCalOut >= ts22_30 || tsCalOut <= ts05_30)
                    {
                        taxi++;
                    }

                    var isWorkingDay = calendarData.Any(x => x.CalendarDay.Date == each.Time_TodayIN!.Value.Date && x.CalendarWorking == "Working Day");
                    var isNextDayWorkingDay = calendarData.Any(x => x.CalendarDay.Date == each.Time_TodayIN!.Value.Date.AddDays(1) && x.CalendarWorking == "Working Day");

                    Driver_Outsource addObj = new Driver_Outsource
                    {
                        EmployeeCode = each.Time_EmployeeCode,
                        Check_In = CheckIn,
                        Check_Out = CheckOut,
                        Job_Type = 0, // 1 is in-out working day 2 is in-out holiday 3 is in working day out holiday 4 is in holiday out working day
                        Temp_Drive = each.Time_DriveInstead,
                        Use_NoUse = string.IsNullOrWhiteSpace(each.Time_wfh_IN.ToString()) ? "NoUse" : "Use",
                        Cal_Time_In = DateTime.ParseExact(each.Time_TodayIN?.ToString("yyyy-MM-dd") + " " + strCal_Time_In, "yyyy-MM-dd HH:mm:ss", null),
                        Cal_Time_Out = DateTime.ParseExact(each.Time_TodayOUT?.ToString("yyyy-MM-dd") + " " + strCal_Time_Out, "yyyy-MM-dd HH:mm:ss", null),
                        Taxi = (short)taxi,
                        Lunch = (short)lunch

                    };

                    if (each.Time_TodayIN.Value.Date != each.Time_TodayOUT.Value.Date)
                    {
                        switch (isWorkingDay, isNextDayWorkingDay)
                        {
                            case (true, false):
                                addObj.Job_Type = 3;
                                break;
                            case (false, true):
                                addObj.Job_Type = 4;
                                break;
                            case (true, true):
                                addObj.Job_Type = 5;
                                break;
                            case (false, false):
                                addObj.Job_Type = 6;
                                break;
                        }
                    }
                    else
                    {
                        addObj.Job_Type = isWorkingDay ? 1 : 2;
                    }

                    if (addObj.Cal_Time_Out - addObj.Cal_Time_In < new TimeSpan(4, 0, 0))
                    {
                        addObj.Cal_Time_Out = addObj.Cal_Time_In + new TimeSpan(4, 0, 0);
                        tsCalOut = tsCalIn + new TimeSpan(4, 0, 0);
                    }


                    if ((addObj.Cal_Time_Out - addObj.Cal_Time_In).TotalHours >= 4)
                    {
                        lunch++;
                    }
                    addObj.Lunch = (short)lunch;

                    if (addObj.Job_Type == 1)
                    {
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Work_OT1_5_Night = ts07_30 - tsCalIn;
                            //change time in to 07:30
                            tsCalIn = ts07_30;
                        }
                        if (tsCalOut >= ts07_30 && tsCalOut <= ts16_30)
                        {
                            addObj.Work_Reg = tsCalOut - tsCalIn;
                            // addObj.Work_Reg = (tsCalOut - tsCalIn) < new TimeSpan(4, 0, 0) ? new TimeSpan(4, 0, 0) : tsCalOut - ts07_30;
                            // if ((tsCalOut - tsCalIn) < new TimeSpan(4, 0, 0))
                            // {
                            //     addObj.Cal_Time_Out = addObj.Cal_Time_In.AddHours(4);
                            // }
                            addObj.Work_Reg = addObj.Work_Reg >= new TimeSpan(5, 0, 0) ? addObj.Work_Reg - new TimeSpan(1, 0, 0) : addObj.Work_Reg;
                        }
                        else
                        {
                            addObj.Work_Reg = ts16_30 - tsCalIn;
                            addObj.Work_Reg = addObj.Work_Reg >= new TimeSpan(5, 0, 0) ? addObj.Work_Reg - new TimeSpan(1, 0, 0) : addObj.Work_Reg;

                            //change time in to 16:30
                            tsCalIn = ts16_30;
                            if (tsCalOut <= ts22_00)
                            {
                                addObj.Work_OT1_5_Eve = tsCalOut - tsCalIn;
                            }
                            else
                            {
                                addObj.Work_OT1_5_Eve = ts22_00 - tsCalIn;

                                //change time in to 22:00
                                tsCalIn = ts22_00;
                                addObj.Work_OT2 = tsCalOut - tsCalIn;
                            }
                        }

                    }
                    else if (addObj.Job_Type == 2)
                    {
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Holi_OT3_0 = ts07_30 - tsCalIn;
                            //change time in to 07:30
                            tsCalIn = ts07_30;
                        }

                        if (tsCalOut >= ts07_30 && tsCalOut <= ts16_30)
                        {
                            addObj.Holi_OT2_0 = tsCalOut - tsCalIn;
                            // addObj.Holi_OT2_0 = (tsCalOut - tsCalIn) < new TimeSpan(4, 0, 0) ? new TimeSpan(4, 0, 0) : tsCalOut - tsCalIn;
                            // if ((tsCalOut - tsCalIn) < new TimeSpan(4, 0, 0))
                            // {
                            //     addObj.Cal_Time_Out = addObj.Cal_Time_In.AddHours(4);
                            // }
                            addObj.Holi_OT2_0 = addObj.Holi_OT2_0 >= new TimeSpan(5, 0, 0) ? addObj.Holi_OT2_0 - new TimeSpan(1, 0, 0) : addObj.Holi_OT2_0;
                        }
                        else
                        {
                            addObj.Holi_OT2_0 = ts16_30 - tsCalIn;
                            addObj.Holi_OT2_0 = addObj.Holi_OT2_0 >= new TimeSpan(5, 0, 0) ? addObj.Holi_OT2_0 - new TimeSpan(1, 0, 0) : addObj.Holi_OT2_0;

                            //change time in to 16:30
                            tsCalIn = ts16_30;
                            addObj.Holi_OT3_0_Eve = tsCalOut - tsCalIn;
                        }

                    }
                    else if (addObj.Job_Type == 3)
                    {
                        //Calculate for 1st Day (Working Day)
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Work_OT1_5_Night = ts07_30 - tsCalIn;
                            addObj.Work_Reg = new TimeSpan(8, 0, 0);
                            addObj.Work_OT1_5_Eve = ts22_00 - ts16_30;
                            addObj.Work_OT2 = new TimeSpan(2, 0, 0);
                            //tsCalIn = ts07_30;
                        }
                        else if (tsCalIn > ts07_30)
                        {

                            addObj.Work_Reg = ts16_30 - tsCalIn;
                            if (tsCalIn > ts16_30)
                            {
                                tsCalIn = ts16_30;
                            }
                            addObj.Work_OT1_5_Eve = ts22_00 - tsCalIn;
                            if (tsCalIn > ts22_00)
                            {
                                tsCalIn = ts22_00;
                            }
                            addObj.Work_OT2 = new TimeSpan(2, 0, 0);
                        }

                        addObj.Holi_OT3_0 = tsCalOut;

                        if (addObj.Holi_OT3_0 > new TimeSpan(7, 30, 0))
                        {
                            throw new Exception("EmployeeCode " + each.Time_EmployeeCode + " Date Login : " + each.Time_TodayIN + " Check Out Over 07:30 ");
                        }
                    }
                    else if (addObj.Job_Type == 4)
                    {

                        //Calculate for 1st Day (Working Day)
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Holi_OT3_0 = ts07_30 - tsCalIn;
                            addObj.Holi_OT2_0 = new TimeSpan(8, 0, 0);
                            addObj.Holi_OT3_0_Eve = new TimeSpan(24, 0, 0) - ts16_30;
                            //tsCalIn = ts07_30;
                        }
                        else if (tsCalIn > ts07_30)
                        {
                            addObj.Holi_OT2_0 = ts16_30 - tsCalIn;
                            if (tsCalIn > ts16_30)
                            {
                                tsCalIn = ts16_30;
                            }
                            addObj.Holi_OT3_0_Eve = new TimeSpan(24, 0, 0) - tsCalIn;
                        }

                        addObj.Work_OT1_5_Night = tsCalOut;

                        if (addObj.Work_OT1_5_Night > new TimeSpan(7, 30, 0))
                        {
                            throw new Exception("EmployeeCode " + each.Time_EmployeeCode + " Date Login : " + each.Time_TodayIN + " Check Out Over 07:30 ");
                        }
                    }
                    else if (addObj.Job_Type == 5)
                    {

                        //Calculate for 1st Day (Working Day)
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Work_OT1_5_Night = ts07_30 - tsCalIn;
                            addObj.Work_Reg = new TimeSpan(8, 0, 0);
                            addObj.Work_OT1_5_Eve = ts22_00 - ts16_30;
                            addObj.Work_OT2 = new TimeSpan(2, 0, 0);
                            //tsCalIn = ts07_30;
                        }
                        else if (tsCalIn > ts07_30)
                        {

                            addObj.Work_Reg = ts16_30 - tsCalIn;
                            if (tsCalIn > ts16_30)
                            {
                                tsCalIn = ts16_30;
                            }
                            addObj.Work_OT1_5_Eve = ts22_00 - tsCalIn;
                            if (tsCalIn > ts22_00)
                            {
                                tsCalIn = ts22_00;
                            }
                            addObj.Work_OT2 = new TimeSpan(2, 0, 0);
                        }

                        addObj.Work_OT1_5_Night += tsCalOut;

                    }
                    else if (addObj.Job_Type == 6)
                    {

                        //Calculate for 1st Day (Working Day)
                        if (tsCalIn > ts00_00 && tsCalIn <= ts07_30)
                        {
                            addObj.Holi_OT3_0 = ts07_30 - tsCalIn;
                            addObj.Holi_OT2_0 = new TimeSpan(8, 0, 0);
                            addObj.Holi_OT3_0_Eve = new TimeSpan(24, 0, 0) - ts16_30;
                            //tsCalIn = ts07_30;
                        }
                        else if (tsCalIn > ts07_30)
                        {
                            addObj.Holi_OT2_0 = ts16_30 - tsCalIn;
                            if (tsCalIn > ts16_30)
                            {
                                tsCalIn = ts16_30;
                            }
                            addObj.Holi_OT3_0_Eve = new TimeSpan(24, 0, 0) - tsCalIn;
                        }

                        addObj.Holi_OT3_0 += tsCalOut;

                    }

                    addObj.Work_Total_OT = addObj.Work_OT1_5_Night + addObj.Work_OT1_5_Eve + addObj.Work_OT2;
                    addObj.Holi_Total_OT = addObj.Holi_OT2_0 + addObj.Holi_OT3_0 + addObj.Holi_OT3_0_Eve;
                    addObj.All_Total_OT = addObj.Work_Total_OT + addObj.Holi_Total_OT;

                    //await _wfContext.Driver_Outsource.AddAsync(addObj);
                    addList.Add(addObj);
                }
                else
                {
                    Driver_Outsource addObj = new Driver_Outsource
                    {
                        EmployeeCode = EmployeeCode,
                        Check_In = date.CalendarDay,
                        Check_Out = date.CalendarDay,
                        Cal_Time_In = date.CalendarDay,
                        Cal_Time_Out = date.CalendarDay,
                        Job_Type = 0, // 1 is in-out working day 2 is in-out holiday 3 is in working day out holiday 4 is in holiday out working day
                        Temp_Drive = "",
                        Use_NoUse = "NoUse",
                        Lunch = 0,
                        Taxi = 0
                    };
                    //await _wfContext.Driver_Outsource.AddAsync(addObj);
                    addList.Add(addObj);
                }

            }
            var errorList = addList.Where(x=>x.All_Total_OT > new TimeSpan(23,59,59)).ToList();
            if(errorList.Count > 0)
            {
                string errorListJSON = JsonConvert.SerializeObject(errorList,Formatting.Indented);
                throw new Exception("Some Data Have Over 24hr of All Total Overtime" + Environment.NewLine + errorListJSON);
            }
            await _wfContext.Driver_Outsource.AddRangeAsync(addList);
            await _wfContext.SaveChangesAsync();

            return addList;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error calculating outsource report");
            throw;
        }
    }

    public async Task<VM_Total_Report_Outsource> SumCalculatedData(VM_CalReport vM_CalReport)
    {
        try
        {
            var yearMonth = DateTime.ParseExact(vM_CalReport.Year + vM_CalReport.Month, "yyyyMM", null);

            var raw = await _wfContext.Driver_Outsource
                .Where(x => x.EmployeeCode == vM_CalReport.EmployeeCode &&
                            x.Check_In.Year == yearMonth.Year &&
                            x.Check_In.Month == yearMonth.Month)
                // No need to GroupBy since we filter to a single month
                .Select(x => new
                {
                    Work_OT1_5_Night_Ticks = (x.Work_OT1_5_Night).Ticks,
                    Work_Reg_Ticks = (x.Work_Reg).Ticks,
                    Work_OT1_5_Eve_Ticks = (x.Work_OT1_5_Eve).Ticks,
                    Work_OT2_Ticks = (x.Work_OT2).Ticks,
                    Work_Total_OT_Ticks = (x.Work_Total_OT).Ticks,
                    Holi_OT3_0_Ticks = (x.Holi_OT3_0).Ticks,
                    Holi_OT2_0_Ticks = (x.Holi_OT2_0).Ticks,
                    Holi_OT3_0_Eve_Ticks = (x.Holi_OT3_0_Eve).Ticks,
                    Holi_Total_OT_Ticks = (x.Holi_Total_OT).Ticks,
                    All_Total_OT_Ticks = (x.All_Total_OT).Ticks,
                    Taxi = x.Taxi,
                    Lunch = x.Lunch,
                }).ToListAsync();

            if (raw.Count < 0)
            {
                throw new InvalidOperationException("Please Calculate Report Outsource Before !");
            }

            // Aggregate in memory and convert ticks -> minutes
            var result = new
            {
                TotalWork_OT1_5_Night = new TimeSpan(raw.Sum(r => r.Work_OT1_5_Night_Ticks)),
                TotalWork_Reg = new TimeSpan(raw.Sum(r => r.Work_Reg_Ticks)),
                TotalWork_OT1_5_Eve = new TimeSpan(raw.Sum(r => r.Work_OT1_5_Eve_Ticks)),
                TotalWork_OT2 = new TimeSpan(raw.Sum(r => r.Work_OT2_Ticks)),
                TotalWork_Total_OT = new TimeSpan(raw.Sum(r => r.Work_Total_OT_Ticks)),
                TotalHoli_OT3_0 = new TimeSpan(raw.Sum(r => r.Holi_OT3_0_Ticks)),
                TotalHoli_OT2_0 = new TimeSpan(raw.Sum(r => r.Holi_OT2_0_Ticks)),
                TotalHoli_OT3_0_Eve = new TimeSpan(raw.Sum(r => r.Holi_OT3_0_Eve_Ticks)),
                TotalHoli_Total_OT = new TimeSpan(raw.Sum(r => r.Holi_Total_OT_Ticks)),
                TotalAll_Total_OT = new TimeSpan(raw.Sum(r => r.All_Total_OT_Ticks)),
                TotalTaxi = raw.Sum(r => r.Taxi),
                TotalLunch = raw.Sum(r => r.Lunch),
            };

            var resultFormatted = new VM_Total_Report_Outsource
            {
                TotalWork_OT1_5_Night = $"{(int)result.TotalWork_OT1_5_Night.TotalHours:D2}:{(int)result.TotalWork_OT1_5_Night.Minutes:D2}",
                TotalWork_Reg = $"{(int)result.TotalWork_Reg.TotalHours:D2}:{(int)result.TotalWork_Reg.Minutes:D2}",
                TotalWork_OT1_5_Eve = $"{(int)result.TotalWork_OT1_5_Eve.TotalHours:D2}:{(int)result.TotalWork_OT1_5_Eve.Minutes:D2}",
                TotalWork_OT2 = $"{(int)result.TotalWork_OT2.TotalHours:D2}:{(int)result.TotalWork_OT2.Minutes:D2}",
                TotalWork_Total_OT = $"{(int)result.TotalWork_Total_OT.TotalHours:D2}:{(int)result.TotalWork_Total_OT.Minutes:D2}",
                TotalHoli_OT3_0 = $"{(int)result.TotalHoli_OT3_0.TotalHours:D2}:{(int)result.TotalHoli_OT3_0.Minutes:D2}",
                TotalHoli_OT2_0 = $"{(int)result.TotalHoli_OT2_0.TotalHours:D2}:{(int)result.TotalHoli_OT2_0.Minutes:D2}",
                TotalHoli_OT3_0_Eve = $"{(int)result.TotalHoli_OT3_0_Eve.TotalHours:D2}:{(int)result.TotalHoli_OT3_0_Eve.Minutes:D2}",
                TotalHoli_Total_OT = $"{(int)result.TotalHoli_Total_OT.TotalHours:D2}:{(int)result.TotalHoli_Total_OT.Minutes:D2}",
                TotalAll_Total_OT = $"{(int)result.TotalAll_Total_OT.TotalHours:D2}:{(int)result.TotalAll_Total_OT.Minutes:D2}",
                TotalTaxi = raw.Sum(r => r.Taxi),
                TotalLunch = raw.Sum(r => r.Lunch),
            };

            //Console.WriteLine("success");

            return resultFormatted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"SumCalculatedData Error");
            throw;
        }
    }

    public async Task<string> Authen(string brownserID, string brownserCurrent, string brownserDevices)
    {
        try
        {
            string sql = @$"SELECT TOP 1 LoginId,substring(convert(varchar,[ExpireDate],121),0,5)+''+substring(convert(varchar,[ExpireDate],121),6,2)+''+substring(convert(varchar,[ExpireDate],121),9,2)  as [ExpireDate],EmployeeCode AS VALUE,Permission
                        FROM [WorkFlow].[Driver_services].[TransectionLogin]
                        where ([Hash] = '{brownserID}'
                        and [Browser] = '{brownserCurrent}'
                        and [Device] = '{brownserDevices}'
                        and Event = 'Login'
                        and Permission = 'Admin'
                        and Status = '1' )";

            var EmployeeCode = await _wfContext.Database.SqlQueryRaw<string>(sql).SingleOrDefaultAsync();

            if (!string.IsNullOrEmpty(EmployeeCode))
            {
                sql = @$"SELECT TOP (1) Driver_name + ' ' + Driver_surname AS VALUE
                            FROM   Driver_services.Driver_Employee
                            WHERE (Driver_EmployeeCode = '{EmployeeCode}')";

                var UserName = await _wfContext.Database.SqlQueryRaw<string>(sql).SingleOrDefaultAsync();
                if (!string.IsNullOrEmpty(UserName))
                {
                    return UserName;
                }
                else
                {
                    throw new KeyNotFoundException("User Name Not Found");
                }
            }
            else
            {
                throw new KeyNotFoundException("Login Transaction Not Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Authen Error");
            throw;
        }
    }

    public async Task<List<Driver_TimeAttendanceDTO>> GetAllAttendanceWaitingData()
    {
        try
        {
            string sqlQuery = $@"SELECT DT.Time_Id,
                        Time_EmployeeCode AS EmployeeCode,
                        DE.Driver_name + ' ' + DE.Driver_surname AS EmployeeFullName,
                        DT.Time_TodayIN + ' ' + DT.Time_DDL_IN AS CheckIn,
                        DT.Time_TodayOUT + ' ' + DT.Time_DDL_OUT AS CheckOut,
                        DMJ.Title_Thai AS WorkTypeIn,
                        WTO.WorkTypeOut,
                        DT.Time_BossId AS BossID,
                        B.Boss_Name + ' ' + B.Boss_Sername AS BossFullName
                    FROM   Driver_services.Driver_TimeAttendance DT
                    JOIN Driver_services.Driver_Master_Job DMJ ON
                    DT.Time_wfh_IN = DMJ.id
                    JOIN Driver_services.Driver_Employee DE ON
                    DT.Time_EmployeeCode = DE.Driver_EmployeeCode
                    JOIN Driver_services.Boss B ON
                    DT.Time_BossId = B.Boss_EmployeeCode
                    JOIN
                        (
                            SELECT Time_Id, DMJ.Title_Thai AS WorkTypeOut
                            FROM   Driver_services.Driver_TimeAttendance DT
                            JOIN Driver_services.Driver_Master_Job DMJ ON
                                DT.Time_wfh_OUT = DMJ.id
                            WHERE Time_StatusDriver = 'waiting'
                                and Time_WaitOut = 'N'
                        ) WTO
                    ON WTO.Time_Id = DT.Time_Id
                    WHERE Time_StatusDriver = 'waiting'
                    and Time_WaitOut = 'N'
                    AND YEAR(DT.Time_TodayIN + ' ' + DT.Time_DDL_IN) = YEAR(GETDATE())
                    AND MONTH(DT.Time_TodayIN + ' ' + DT.Time_DDL_IN) = MONTH(DATEADD(MONTH,-1,GETDATE()))
                    AND Time_DDL_OUT IS NOT NULL
                    AND Time_DDL_IN IS NOT NULL
                    ";

            var data = await _wfContext.Driver_TimeAttendanceDTO.FromSqlRaw(sqlQuery).AsNoTracking().ToListAsync();

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError("GetAllAttendanceWaitingData Error : {ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

    public async Task ApproveAllData(List<int> ApproveIdList,string UserName)
    {
        try
        {
            foreach (var id in ApproveIdList)
            {
                var unApproveData = await _wfContext.Driver_TimeAttendance.Where(x => x.Time_Id == id).SingleOrDefaultAsync();
                if (unApproveData != null)
                {
                    unApproveData.Time_ApprovedIN = unApproveData.Time_ApprovedIN ?? UserName;
                    unApproveData.Time_ApprovedOUT = unApproveData.Time_ApprovedOUT ?? UserName;
                    unApproveData.Time_TimeApprovedIN = unApproveData.Time_TimeApprovedIN ?? DateTime.Now;
                    unApproveData.Time_TimeApprovedOUT = unApproveData.Time_TimeApprovedOUT ?? DateTime.Now;
                    unApproveData.Time_StatusDriver = "End";
                    unApproveData.Time_Working_D = "Auto";
                    _wfContext.Update(unApproveData);
                }
            }
            await _wfContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"ApproveAllData Error");
            throw;
        }
    }

    public async Task<string> AuthenDriver(string brownserID, string brownserCurrent, string brownserDevices)
    {
        try
        {
            string sql = @$"SELECT TOP 1 LoginId,substring(convert(varchar,[ExpireDate],121),0,5)+''+substring(convert(varchar,[ExpireDate],121),6,2)+''+substring(convert(varchar,[ExpireDate],121),9,2)  as [ExpireDate],EmployeeCode AS VALUE,Permission
                        FROM [WorkFlow].[Driver_services].[TransectionLogin]
                        where ([Hash] = '{brownserID}'
                        and [Browser] = '{brownserCurrent}'
                        and [Device] = '{brownserDevices}'
                        and Event = 'Login'
                        and Permission = 'Driver'
                        and Status = '1' )";

            var EmployeeCode = await _wfContext.Database.SqlQueryRaw<string>(sql).SingleOrDefaultAsync();

            if (!string.IsNullOrEmpty(EmployeeCode))
            {
                sql = @$"SELECT TOP (1) Driver_name + ' ' + Driver_surname AS VALUE
                            FROM   Driver_services.Driver_Employee
                            WHERE (Driver_EmployeeCode = '{EmployeeCode}')";

                var UserName = await _wfContext.Database.SqlQueryRaw<string>(sql).SingleOrDefaultAsync();
                if (!string.IsNullOrEmpty(UserName))
                {
                    return UserName;
                }
                else
                {
                    throw new SecurityTokenValidationException("User Name Not Found");
                }
            }
            else
            {
                throw new SecurityTokenValidationException("Login Transaction Not Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"AuthenDriver Error");
            throw;
        }
    }

    public async Task<List<DriverOT_DTOs>> GetDriverOTTime(string UserName)
    {
        try
        {
            string sql = @$"SELECT TOP (1) Driver_EmployeeCode AS VALUE
                            FROM   Driver_services.Driver_Employee
                            WHERE (Driver_name + ' ' + Driver_surname = '{UserName}')
                            AND Driver_Position = 'DRIVER' ";
            _logger.LogInformation("GetDriverOTTime SQL : {sql}",sql);
            var EmployeeCode = await _wfContext.Database.SqlQueryRaw<string>(sql).SingleOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(EmployeeCode))
            {
                throw new Exception("Employee Code Not Found");
            }

            var data = await _wfContext.Driver_Outsource.AsNoTracking().Where(x=>x.EmployeeCode == EmployeeCode
                && x.Check_In.Month == DateTime.Now.Month
                && x.Check_In.Year == DateTime.Now.Year
                ).ToListAsync();

            if(data.Count == 0)
            {
                var obj = new VM_CalReport
                {
                    EmployeeCode = EmployeeCode,
                    Year = DateTime.Now.ToString("yyyy"),
                    Month = DateTime.Now.ToString("MM")
                };

                data = await RefreshReportDriverOutSourceAsync(obj);
            }

            var DriverOTList = new List<DriverOT_DTOs>();

            foreach(var each in data)
            {
                var DriverOT = new DriverOT_DTOs
                {
                    CheckInReal = each.Check_In.ToString("dd/MM/yyyy HH:mm"),
                    CheckOutReal = each.Check_Out.ToString("dd/MM/yyyy HH:mm"),
                    CheckInCal = each.Cal_Time_In.ToString("dd/MM/yyyy HH:mm"),
                    CheckOutCal = each.Cal_Time_Out.ToString("dd/MM/yyyy HH:mm"),
                    WorkingHours = each.Work_Reg.ToString("hh\\:mm"),
                    SumOT_1_5 = (each.Work_OT1_5_Eve + each.Work_OT1_5_Night).ToString("hh\\:mm"),
                    SumOT_2_0 = (each.Holi_OT2_0 + each.Work_OT2).ToString("hh\\:mm"),
                    SumOT_3_0 = (each.Holi_OT3_0 + each.Holi_OT3_0_Eve).ToString("hh\\:mm"),
                    SumOT = TimeSpan.Zero.ToString("hh\\:mm"),
                    Lunch = each.Lunch,
                    Taxi = each.Taxi
                };
                DriverOT.SumOT = (each.Work_OT1_5_Eve + each.Work_OT1_5_Night
                    + each.Holi_OT2_0 + each.Work_OT2
                    + each.Holi_OT3_0 + each.Holi_OT3_0_Eve).ToString("hh\\:mm");

                DriverOTList.Add(DriverOT);
            }

            return DriverOTList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Can not Get Driver OT Time");
            //_logger.LogError("{ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

    public async Task<VM_Driver_Outsource_Report> GetFormatedResult(VM_CalReport vM)
    {
        try
        {

            var result = await RefreshReportDriverOutSourceAsync(vM);
            var referResult = await SumCalculatedData(vM);

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

            return data;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Can not Generate Excel file");
            //_logger.LogError("{ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

    public async Task<MemoryStream> GenerateExcelReport(VM_CalReport vM)
    {
        try
        {
            var memStream = new MemoryStream();
            string excelPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","excel_file","Driver_Master_ex.xlsx");
            DateTime YearMonth = DateTime.ParseExact(vM.Year + vM.Month,"yyyyMM",null);
            using var excelFile = new XLWorkbook(excelPath);
            List<VM_CalReport> attendanceData = new List<VM_CalReport>();

            if( string.IsNullOrWhiteSpace(vM.EmployeeCode) )
            {
                attendanceData = await _wfContext.Driver_TimeAttendance.AsNoTracking()
                    .Where(x => x.Time_TodayIN.Value.Year == YearMonth.Year &&
                    x.Time_TodayIN.Value.Month == YearMonth.Month &&
                    x.Time_StatusDriver.ToLower() == "end")
                    .Select(x=> new VM_CalReport
                    {
                        EmployeeCode = x.Time_EmployeeCode,
                        Month = x.Time_TodayIN.Value.Month.ToString("D2"),
                        Year = x.Time_TodayIN.Value.Year.ToString("D2")
                    })
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                attendanceData = await _wfContext.Driver_TimeAttendance.AsNoTracking()
                    .Where(x => x.Time_TodayIN.Value.Year == YearMonth.Year &&
                    x.Time_TodayIN.Value.Month == YearMonth.Month &&
                    x.Time_EmployeeCode == vM.EmployeeCode &&
                    x.Time_StatusDriver.ToLower() == "end")
                    .Select(x=> new VM_CalReport
                    {
                        EmployeeCode = x.Time_EmployeeCode,
                        Month = x.Time_TodayIN.Value.Month.ToString("D2"),
                        Year = x.Time_TodayIN.Value.Year.ToString("D2")
                    })
                    .Distinct()
                    .ToListAsync();
            }

            if( attendanceData.Count == 0)
            {
                throw new Exception($"Attendance Data Driver Not Found {vM.EmployeeCode} {vM.Year} {vM.Month}");
            }

            foreach (var each in attendanceData)
            {
                vM.EmployeeCode = each.EmployeeCode;
                vM.Month = each.Month;
                vM.Year = each.Year;
                var calDataList = await GetFormatedResult(vM);

                var getDriverData = await _wfContext.Driver_Employee.Where(x=>x.Driver_EmployeeCode == each.EmployeeCode)
                    //.Include(x=>x.Boss)
                    .OrderByDescending(x=>x.Driver_id)
                    .FirstOrDefaultAsync();

                if(getDriverData == null)
                {
                    throw new Exception("Driver Employee Not Found");
                }

                var getBossData = await _wfContext.Boss.Where(x=>x.Boss_Id.ToString() == getDriverData.Driver_Boss)
                    .OrderByDescending(x=>x.Boss_Id).FirstOrDefaultAsync();

                if(getBossData == null)
                {
                    throw new Exception("Driver Boss Not Found");
                }

                var masterExcel = excelFile.Worksheet("Master");
                var newSheet = masterExcel.CopyTo(getDriverData.Driver_name + "." + getDriverData.Driver_surname);
                newSheet.Cell("A1").Value = "Report Driver OT by daily";
                newSheet.Cell("A2").Value = "DRIVER : ";
                newSheet.Cell("A3").Value = "VENDOR : ";
                newSheet.Cell("A4").Value = "EMP.CODE : ";
                newSheet.Cell("A5").Value = "USER : ";
                newSheet.Cell("A6").Value = "POSITION : ";
                newSheet.Cell("A7").Value = "COMPANY : ";

                newSheet.Cell("B2").Value = getDriverData.Driver_name + "." + getDriverData.Driver_surname;
                newSheet.Cell("B3").Value = getDriverData.Driver_Vendor;
                newSheet.Cell("B4").Value = getDriverData.Driver_EmployeeCode;
                newSheet.Cell("B5").Value = getBossData.Boss_Name + "." + getBossData.Boss_Sername;
                newSheet.Cell("B6").Value = getBossData.Boss_Position;
                newSheet.Cell("B7").Value = getBossData.Boss_Company;

                newSheet.Cell("A9").Value = vM.Year + vM.Month;
                var indexCalData = 0;
                for(int i = 14; i <= 44; i++)
                {
                    if(indexCalData == calDataList.ReportList.Count)
                    {
                        continue;
                    }
                    var calData = calDataList.ReportList[indexCalData];

                    newSheet.Cell("A"+i.ToString()).Value = calData.Check_In.Split(" ")[0];
                    newSheet.Cell("B"+i.ToString()).Value = calData.Job_Type % 2 == 1 ? "Working Day" : "Holiday";
                    newSheet.Cell("C"+i.ToString()).Value = calData.Temp_Drive;
                    newSheet.Cell("D"+i.ToString()).Value = calData.Use_NoUse;
                    newSheet.Cell("E"+i.ToString()).Value = calData.Check_In.Split(" ")[1];
                    newSheet.Cell("F"+i.ToString()).Value = calData.Check_Out.Split(" ")[1];
                    newSheet.Cell("G"+i.ToString()).Value = calData.Cal_Time_In.Split(" ")[1];
                    newSheet.Cell("H"+i.ToString()).Value = calData.Cal_Time_Out.Split(" ")[1];
                    newSheet.Cell("I"+i.ToString()).Value = calData.Work_OT1_5_Night;
                    newSheet.Cell("J"+i.ToString()).Value = calData.Work_OT1_5_Eve;
                    newSheet.Cell("K"+i.ToString()).Value = calData.Work_OT2;
                    newSheet.Cell("L"+i.ToString()).Value = calData.Work_Total_OT;
                    newSheet.Cell("O"+i.ToString()).Value = calData.Holi_OT2_0;
                    //newSheet.Cell("P"+i.ToString()).Value = calData.Holi_OT2_0;
                    newSheet.Cell("P"+i.ToString()).Value = calData.Holi_OT3_0_Eve;
                    newSheet.Cell("Q"+i.ToString()).Value = calData.Holi_OT3_0;
                    newSheet.Cell("R"+i.ToString()).Value = calData.Holi_Total_OT;
                    newSheet.Cell("S"+i.ToString()).Value = calData.All_Total_OT;
                    newSheet.Cell("T"+i.ToString()).Value = calData.Taxi;

                    indexCalData++;
                }

                newSheet.Cell("I45").Value = calDataList.ReferReport.TotalWork_OT1_5_Night;
                newSheet.Cell("J45").Value = calDataList.ReferReport.TotalWork_OT1_5_Eve;
                newSheet.Cell("K45").Value = calDataList.ReferReport.TotalWork_OT2;
                newSheet.Cell("L45").Value = calDataList.ReferReport.TotalWork_Total_OT;
                newSheet.Cell("O45").Value = calDataList.ReferReport.TotalHoli_OT2_0;
                //newSheet.Cell("P45").Value = calDataList.ReferReport.TotalHoli_OT2_0;
                newSheet.Cell("P45").Value = calDataList.ReferReport.TotalHoli_OT3_0_Eve;
                newSheet.Cell("Q45").Value = calDataList.ReferReport.TotalHoli_OT3_0;
                newSheet.Cell("R45").Value = calDataList.ReferReport.TotalHoli_Total_OT;
                newSheet.Cell("S45").Value = calDataList.ReferReport.TotalAll_Total_OT;
                newSheet.Cell("T45").Value = calDataList.ReferReport.TotalTaxi;

                newSheet.Cell("L46").Value = calDataList.ReferReport.TotalWork_Total_OT.Replace(":15",":25").Replace(":30",":50").Replace(":45",":75").Replace(":",".");
                //newSheet.Cell("O46").Value = calDataList.ReferReport.TotalHoli_OT2_0.Replace(":15",":25").Replace(":30",":50").Replace(":45",":75").Replace(":",".");
                newSheet.Cell("R46").Value = calDataList.ReferReport.TotalHoli_Total_OT.Replace(":15",":25").Replace(":30",":50").Replace(":45",":75").Replace(":",".");
                newSheet.Cell("S46").Value = calDataList.ReferReport.TotalAll_Total_OT.Replace(":15",":25").Replace(":30",":50").Replace(":45",":75").Replace(":",".");

                newSheet.Cell("S47").Value = calDataList.ReferReport.TotalAll_Total_OT;
                newSheet.Cell("S48").Value = "100:00";
                var RemainOT_Hours = 100 - int.Parse(calDataList.ReferReport.TotalAll_Total_OT.Split(":")[0]);
                //var RemainOT_Mins = 00 - int.Parse(calDataList.ReferReport.TotalAll_Total_OT.Split(":")[1]);

                newSheet.Cell("S49").Value = RemainOT_Hours.ToString() + ":" + calDataList.ReferReport.TotalAll_Total_OT.Split(":")[1];
                newSheet.Cell("F50").Value = calDataList.ReferReport.TotalLunch;
            }

            //excelFile.SaveAs("Driver_" + vM.Year + vM.Month);
            excelFile.Worksheet("Master").Delete();
            excelFile.SaveAs(memStream);

            memStream.Position = 0;
            return memStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Can not Generate Excel file");
            //_logger.LogError("{ex.InnerException?.Message} {ex.Message}",ex.InnerException?.Message,ex.Message);
            throw;
        }
    }

}
