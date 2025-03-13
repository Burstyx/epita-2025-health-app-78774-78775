using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages.Patient;

[Authorize(Roles = "Patient")]
public class PatientModel : PageModel
{
    private readonly ApplicationDbContext _context;
    
    public List<Appointment> ReservedAppointments { get; set; }

    public PatientModel(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ReservedAppointments = _context.Appointments.Where(a => a.UserId == userId).ToList();
    }
    
    public string GetDoctorName(string doctorId)
    {
        return _context.Users.Find(doctorId)?.UserName ?? doctorId;
    }
    
    public IActionResult OnPost(int selectedAppointmentId)
    {
        Appointment appointment = _context.Appointments.Find(selectedAppointmentId);
        if (appointment == null)
            return NotFound();
        
        _context.Appointments.Remove(appointment);
        _context.SaveChanges();
        
        return RedirectToPage("/Patient/Index");
    }
}