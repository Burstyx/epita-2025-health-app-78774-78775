namespace HealthApp.Razor.Data;

public class Appointment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string DoctorId { get; set; }
    public long DateTimeMilli { get; set; }
    public int IsConfirmed { get; set; }
}