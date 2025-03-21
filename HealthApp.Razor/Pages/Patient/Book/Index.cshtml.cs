using System.Globalization;
using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthApp.Razor.Pages.Patient.Book
{
    public class PatientBookModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private List<IdentityUser> Doctors { get; set; }

        [BindProperty(SupportsGet = true)] public string? DoctorId { get; set; }
        [BindProperty(SupportsGet = true)] public int DaysToShow { get; set; } = 7;

        public List<Appointment> ReservedAppointments { get; set; }
        public List<SelectListItem> DoctorList { get; set; }

        [BindProperty] public string selectedDate { get; set; }
        [BindProperty] public string selectedTime { get; set; }
        [BindProperty] public string selectedDoctor { get; set; }

        public Appointment Appointment { get; set; }

        public PatientBookModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            if (DaysToShow < 7) DaysToShow = 7;
            LoadData();
        }

        public IActionResult OnPost()
        {
            if (DaysToShow < 7) DaysToShow = 7;
            LoadData();

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return RedirectToPage("/Identity/Account/Login", new { area = "Identity" });

            if (string.IsNullOrWhiteSpace(selectedDate) || string.IsNullOrWhiteSpace(selectedTime))
            {
                ModelState.AddModelError(string.Empty, "Date and time must be provided.");
                return Page();
            }

            if (!DateTime.TryParseExact($"{selectedDate} {selectedTime}", "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                ModelState.AddModelError(string.Empty, "Invalid date or time format.");
                return Page();
            }

            var dateTimeMilli = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            var isAlreadyBooked = _context.Appointments
                .Any(a => a.DoctorId == selectedDoctor && a.DateTimeMilli == dateTimeMilli && a.IsConfirmed != -1);

            if (isAlreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "This appointment slot is already booked.");
                return Page();
            }

            Appointment = new Appointment
            {
                IsConfirmed = 0,
                UserId = userId,
                DoctorId = selectedDoctor,
                DateTimeMilli = dateTimeMilli
            };

            _context.Appointments.Add(Appointment);
            _context.SaveChanges();

            return RedirectToPage("/Patient/Index");
        }

        public IActionResult OnGetUpdateAppointments(string doctorId)
        {
            ReservedAppointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToList();

            return new JsonResult(ReservedAppointments);
        }

        private void LoadData()
        {
            Doctors = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "1"))
                .ToList();

            DoctorId ??= Doctors.FirstOrDefault()?.Id ?? "";

            ReservedAppointments = _context.Appointments
                .Where(a => a.DoctorId == DoctorId)
                .ToList();

            DoctorList = Doctors.Select(d => new SelectListItem
            {
                Value = d.Id,
                Text = d.UserName,
                Selected = DoctorId == d.Id
            }).ToList();
        }
    }
}
