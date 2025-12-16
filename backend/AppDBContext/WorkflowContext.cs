using driver_api.Models;
using driver_api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class WorkflowContext : DbContext
{
    public WorkflowContext(DbContextOptions<WorkflowContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Driver_Employee>()
        //     .HasOne(e => e.Boss)
        //     .WithMany(b=>b.Driver_Employees)
        //     .HasForeignKey(e=>e.Driver_Boss);
    }

    public DbSet<Driver_Employee> Driver_Employee { get; set; }
    public DbSet<Boss> Boss { get; set; }
    public DbSet<Driver_TimeAttendance> Driver_TimeAttendance { get; set; }
    public DbSet<Driver_Outsource> Driver_Outsource { get; set; }
    public DbSet<Driver_Calendar> Driver_Calendar { get; set; }
    public DbSet<Driver_TimeAttendanceDTO> Driver_TimeAttendanceDTO { get; set; }
}