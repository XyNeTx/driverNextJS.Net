using driver_api.Models;
using driver_api.Models.DTOs;
using driver_api.Models.ViewModels;
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
            throw new Exception("Cant get Report Driver Outsource");
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
            throw new Exception("Cant get Report Driver Outsource");
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
                        each = workList.Where(x => !string.IsNullOrWhiteSpace(x.Time_Driver_UpdateTime)).FirstOrDefault();
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
            await _wfContext.Driver_Outsource.AddRangeAsync(addList);
            await _wfContext.SaveChangesAsync();

            return addList;

        }
        catch (Exception ex)
        {
            throw new Exception("Error calculating outsource report", ex);
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
                throw new Exception("Please Calculate Report Outsource Before !");
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
            throw new Exception("Sum Calculated Data Error !" + ex.Message);
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

            var EmployeeCode = await _wfContext.Database.SqlQueryRaw<string>(sql).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(EmployeeCode))
            {
                sql = @$"SELECT TOP (1) Driver_name + ' ' + Driver_surname AS VALUE
                            FROM   Driver_services.Driver_Employee
                            WHERE (Driver_EmployeeCode = '{EmployeeCode}')";

                var UserName = await _wfContext.Database.SqlQueryRaw<string>(sql).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(UserName))
                {
                    return UserName;
                }
                else
                {
                    throw new Exception("User Name Not Found");
                }
            }
            else
            {
                throw new Exception("Login Transaction Not Found");
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Please Login then Try Again ", ex.InnerException ?? ex);
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

                    //

            var data = await _wfContext.Driver_TimeAttendanceDTO.FromSqlRaw(sqlQuery).AsNoTracking().ToListAsync();

            return data;
        }
        catch (Exception ex)
        {
            throw new Exception("Can't Get not Approved Data ", ex.InnerException ?? ex);
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
            throw new Exception("Can't Approve Data ", ex.InnerException ?? ex);
        }
    }

}
