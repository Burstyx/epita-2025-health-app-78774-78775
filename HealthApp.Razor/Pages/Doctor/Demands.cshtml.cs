using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages
{
    public class DemandsModel : PageModel
    {
        public void OnGet()
        {
        }
    private readonly ApplicationDbContext _context;

    public PatientAppointmentsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<AppointmentViewModel> Appointments { get; set; }

    public async Task OnGetAsync()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            RedirectToPage("/Identity/Account/Login");
            return;
        }

        Appointments = await _context.Appointments
            .Where(a => a.UserId == userId)
            .Join(_context.Users, a => a.DoctorId, d => d.Id, (a, d) => new AppointmentViewModel
            {
                DateTimeMilli = a.DateTimeMilli,
                DoctorName = d.UserName,
                IsConfirmed = a.IsConfirmed
            })
            .ToListAsync();
    }
}

public class AppointmentViewModel
{
    public long DateTimeMilli { get; set; }
    public string DoctorName { get; set; }
    public bool IsConfirmed { get; set; }
}
}
