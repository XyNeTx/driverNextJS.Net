namespace driver_api.Models.DTOs;

public class DriverOT_DTOs
{
    public DateTime CheckInReal { get; set; }
    public DateTime CheckOutReal { get; set; }
    public DateTime CheckInCal { get; set; }
    public DateTime CheckOutCal { get; set; }
    public TimeSpan WorkingHours { get; set; }
    public TimeSpan SumOT_1_5 { get; set; }
    public TimeSpan SumOT_2_0 { get; set; }
    public TimeSpan SumOT_3_0 { get; set; }
    public TimeSpan SumOT { get; set; }
    public int Lunch { get; set; }
    public int Taxi { get; set; }
}
