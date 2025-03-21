using System.Globalization;
using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthApp.Razor.Pages.Patient.Book;

public class PatientBookModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private List<IdentityUser> Doctors { get; set; }

    [BindProperty(SupportsGet = true)] public string? DoctorId { get; set; }
    public List<Appointment> ReservedAppointments { get; set; }
    public Appointment Appointment { get; set; }
    public List<SelectListItem> DoctorList { get; set; }

    public PatientBookModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        Doctors = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "2")).ToList();
        if(DoctorId == null) DoctorId = Doctors.FirstOrDefault()?.Id ?? "";
        ReservedAppointments = _context.Appointments.Where(a => a.DoctorId == DoctorId).ToList();
        
        DoctorList = Doctors.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.UserName,
            Selected = DoctorId == d.Id.ToString()
        }).ToList();
    }

    public IActionResult OnPost(string selectedDate, string selectedTime, string selectedDoctor)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return RedirectToPage("/Identity/Account/Login", new { area = "Identity" });
        
        Appointment = new Appointment
        {
            IsConfirmed = 0,
            UserId = userId,
            DoctorId = selectedDoctor,
            DateTimeMilli = new DateTimeOffset(DateTime.ParseExact($"{selectedDate} {selectedTime}", "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None)).ToUnixTimeMilliseconds(),
        };

        _context.Appointments.Add(Appointment);
        _context.SaveChanges();

        return RedirectToPage("/Patient/Index");
    }
    
    public IActionResult OnGetUpdateAppointments(string doctorId)
    {
        ReservedAppointments = _context.Appointments.Where(a => a.DoctorId == doctorId).ToList();
        return new JsonResult(ReservedAppointments);
    }
}