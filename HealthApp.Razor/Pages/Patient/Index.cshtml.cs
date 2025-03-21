using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages.Patient
{
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
            ReservedAppointments = _context.Appointments
                .Where(a => a.UserId == userId)
                .ToList();
        }

        public IActionResult OnPostCancelAppointment(int selectedAppointmentId)
        {
            var appointment = _context.Appointments.Find(selectedAppointmentId);
            if (appointment == null)
                return NotFound();

            appointment.IsConfirmed = -1;
            _context.Appointments.Update(appointment);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public string GetDoctorName(string doctorId)
        {
            return _context.Users.Find(doctorId)?.UserName ?? "Unknown";
        }

        public string GetAppointmentColor(int isConfirmed)
        {
            return isConfirmed switch
            {
                1 => "green",
                0 => "orange",
                -1 => "red",
                _ => "gray"
            };
        }
    }
}