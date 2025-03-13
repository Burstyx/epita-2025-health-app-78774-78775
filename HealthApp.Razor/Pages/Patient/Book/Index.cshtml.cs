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

    public Appointment Appointment { get; set; }
    public List<SelectListItem> DoctorList { get; set; }

    public PatientBookModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        Doctors = _context.Users.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "987094ad-cd8e-40f5-9f44-cf1088065b2a")).ToList();
        DoctorList = Doctors.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.UserName
        }).ToList();
    }

    public IActionResult OnPost(string selectedDate, string selectedTime, string selectedDoctor)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return RedirectToPage("/Identity/Account/Login", new { area = "Identity" });
        
        Console.WriteLine(selectedDoctor);
        Console.WriteLine("dfdsf");
        
        Appointment = new Appointment
        {
            IsConfirmed = false,
            UserId = userId,
            DoctorId = selectedDoctor,
            DateTimeMilli = new DateTimeOffset(DateTime.ParseExact($"{selectedDate} {selectedTime}", "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None)).ToUnixTimeMilliseconds(),
        };

        _context.Appointments.Add(Appointment);
        _context.SaveChanges();

        return RedirectToPage("/Patient/Index");
    }
}