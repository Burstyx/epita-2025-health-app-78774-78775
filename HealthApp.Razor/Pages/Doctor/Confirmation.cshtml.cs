using HealthApp.Razor.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class ConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

        // ?? ? **UN SEUL CONSTRUCTEUR**
        public ConfirmationModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void OnGet()
        {
            string? doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(doctorId))
            {
                Console.WriteLine("? M�decin non connect� !");
                return;
            }

            Appointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.IsConfirmed == 0)
                .ToList();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
            {
                return NotFound("Rendez-vous non trouv�.");
            }

            string? doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (appointment.DoctorId != doctorId)
            {
                return Unauthorized("Vous ne pouvez confirmer que vos propres rendez-vous.");
            }

            appointment.IsConfirmed = 1;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        private IActionResult Unauthorized(string v)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return NotFound("Rendez-vous non trouv�.");

            string? doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (appointment.DoctorId != doctorId)
                return Unauthorized("Vous ne pouvez annuler que vos propres rendez-vous.");

            appointment.IsConfirmed = -1; // ? Rendez-vous annul�
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
