using driver_api.Models;
using driver_api.Repository.IRepo;
using Microsoft.EntityFrameworkCore;
using System;

namespace driver_api.Repository.Repo;

public class ReportOutSourceRepo : IReportOutSourceRepo
{
    private readonly WorkflowContext _wfContext;

    public ReportOutSourceRepo(WorkflowContext wfContext)
    {
        _wfContext = wfContext;
    }

    public async Task<List<Driver_Employee>> GetListDriverAsync()
    {
        try
        {
            var driverList = await _wfContext.Driver_Employee.AsNoTracking()
                .Where(x => x.Driver_Position!.ToLower() == "driver")
                .ToListAsync();

            return driverList;
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving driver list", ex);
        }
    }

    public async Task CalculateOutsourceReportAsync(string EmployeeCode, string Year , string Month)
    {
        var ts00_00 = new TimeSpan(0, 0, 0);
        var ts22_00 = new TimeSpan(22, 0, 0);
        var ts22_30 = new TimeSpan(22, 30, 0);
        var ts05_30 = new TimeSpan(5, 30, 0);
        var ts07_30 = new TimeSpan(7, 30, 0);
        var ts16_30 = new TimeSpan(16, 30, 0);

        try
        {
            DateTime yearMonth = DateTime.ParseExact(Year + Month, "yyyyMM",null);
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

            foreach(var date in calendarData)
            {
                var each = attendanceData.FirstOrDefault(x => x.Time_TodayIN.Value.Date == date.CalendarDay.Date);
                
                if(each != null)
                {
                    each.Time_HomeIN = each.Time_HomeIN.Replace(":", ".");
                    each.Time_HomeOUT = each.Time_HomeOUT.Replace(":", ".");

                    var TimeHomeIn = each.Time_HomeIN.Split(".")[0].Length == 1 ? "0" + each.Time_HomeIN : each.Time_HomeIN;
                    var TimeHomeOut = each.Time_HomeOUT.Split(".")[0].Length == 1 ? "0" + each.Time_HomeOUT : each.Time_HomeOUT;

                    var CheckIn = DateTime.ParseExact(each.Time_TodayIN?.ToString("yyyy-MM-dd") + " " + TimeHomeIn + ":00", "yyyy-MM-dd HH.mm:ss", null);
                    var CheckOut = DateTime.ParseExact(each.Time_TodayOUT?.ToString("yyyy-MM-dd") + " " + TimeHomeOut + ":00", "yyyy-MM-dd HH.mm:ss", null);

                    var tsHomeIN = new TimeSpan(int.Parse(each.Time_HomeIN.Split(".")[0]), int.Parse(each.Time_HomeIN.Split(".")[1]), 0);
                    var tsHomeOUT = new TimeSpan(int.Parse(each.Time_HomeOUT.Split(".")[0]), int.Parse(each.Time_HomeOUT.Split(".")[1]), 0);

                    var tsCalIn = tsHomeIN;
                    var tsCalOut = tsHomeOUT;
                    var strCal_Time_In = tsCalIn.Hours.ToString("D2") + ":" + tsCalIn.Minutes.ToString("D2") + ":00";
                    var strCal_Time_Out = tsCalOut.Hours.ToString("D2") + ":" + tsCalOut.Minutes.ToString("D2") + ":00";
                    var taxi = 0;
                    var lunch = 0;

                    if (each.Time_wfh_IN == "1")
                    {
                        tsCalIn = tsCalIn - new TimeSpan(0, 30, 0);
                        if (tsCalIn >= ts00_00 && tsCalIn <= ts05_30)
                        {
                            taxi++;
                        }
                        strCal_Time_In = tsCalIn.Hours.ToString("D2") + ":" + tsCalIn.Minutes.ToString("D2") + ":00";
                    }
                    if (each.Time_wfh_OUT == "1")
                    {
                        tsCalOut = tsCalOut + new TimeSpan(0, 15, 0);
                        strCal_Time_Out = tsCalOut.Hours.ToString("D2") + ":" + tsCalOut.Minutes.ToString("D2") + ":00";
                        if (tsCalOut >= ts22_30 || tsCalOut <= ts05_30)
                        {
                            taxi++;
                        }
                    }
                    if (tsCalOut - tsCalIn >= new TimeSpan(4, 0, 0))
                    {
                        lunch++;
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
                            case (true, true):
                                addObj.Job_Type = 1;
                                break;
                            case (false, false):
                                addObj.Job_Type = 2;
                                break;
                            case (true, false):
                                addObj.Job_Type = 3;
                                break;
                            case (false, true):
                                addObj.Job_Type = 4;
                                break;
                        }
                    }
                    else
                    {
                        addObj.Job_Type = isWorkingDay ? 1 : 2;
                    }

                    // OT Night IN before 07:30
                    if (tsCalIn > ts00_00 && tsCalIn < ts07_30)
                    {
                        if (addObj.Job_Type == 1 || addObj.Job_Type == 3)
                        {
                            addObj.Work_OT1_5_Night = ts07_30 - tsCalIn;
                        }
                        else if (addObj.Job_Type == 2 || addObj.Job_Type == 4)
                        {
                            addObj.Holi_OT3_0 = ts07_30 - tsCalIn;
                        }
                    }
                    // Normal time IN 07:30 - OUT 16:30
                    if (tsCalIn <= ts07_30 && tsCalOut >= ts16_30)
                    {
                        if (addObj.Job_Type == 1 || addObj.Job_Type == 3)
                        {
                            addObj.Work_Reg = ts16_30 - ts07_30;
                            addObj.Work_Reg = addObj.Work_Reg >= new TimeSpan(4, 0, 0) ? addObj.Work_Reg - new TimeSpan(1, 0, 0) : addObj.Work_Reg;
                        }
                        else if (addObj.Job_Type == 2 || addObj.Job_Type == 4)
                        {
                            addObj.Holi_OT2_0 = ts16_30 - ts07_30;
                            addObj.Holi_OT2_0 = addObj.Holi_OT2_0 >= new TimeSpan(4, 0, 0) ? addObj.Holi_OT2_0 - new TimeSpan(1, 0, 0) : addObj.Holi_OT2_0;
                        }
                    }
                    // OT Evening OUT after 16:30 - 22:00
                    if (tsCalOut > ts16_30 && tsCalOut <= ts22_00)
                    {
                        if (addObj.Job_Type == 1 || addObj.Job_Type == 3)
                        {
                            addObj.Work_OT1_5_Eve = tsCalOut - ts16_30;
                        }
                        else if (addObj.Job_Type == 2 || addObj.Job_Type == 4)
                        {
                            addObj.Holi_OT3_0_Eve = tsCalOut - ts16_30;
                        }
                    }
                    if (tsCalOut > ts22_00)
                    {
                        if (addObj.Job_Type == 1 || addObj.Job_Type == 3)
                        {
                            addObj.Work_OT2 = tsCalOut - ts22_00;
                        }
                        else if (addObj.Job_Type == 2 || addObj.Job_Type == 4)
                        {
                            addObj.Holi_OT3_0_Eve = addObj.Holi_OT3_0_Eve + (tsCalOut - ts22_00);
                        }
                    }
                    if (tsCalOut > ts00_00 && tsCalOut < ts07_30)
                    {
                        if (addObj.Job_Type == 1 || addObj.Job_Type == 4)
                        {
                            addObj.Work_OT2 = addObj.Work_OT2 + (tsCalOut - ts00_00);
                        }
                        else if (addObj.Job_Type == 2 || addObj.Job_Type == 3)
                        {
                            addObj.Holi_OT3_0_Eve = addObj.Holi_OT3_0_Eve + (tsCalOut - ts00_00);
                        }
                    }

                    addObj.Work_Total_OT = addObj.Work_OT1_5_Night + addObj.Work_OT1_5_Eve + addObj.Work_OT2;
                    addObj.Holi_Total_OT = addObj.Holi_OT2_0 + addObj.Holi_OT3_0 + addObj.Holi_OT3_0_Eve;
                    addObj.All_Total_OT = addObj.Work_Total_OT + addObj.Holi_Total_OT;

                    await _wfContext.Driver_Outsource.AddAsync(addObj);
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
                    await _wfContext.Driver_Outsource.AddAsync(addObj);
                }

            }
            await _wfContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new Exception("Error calculating outsource report", ex);
        }
    }

}
