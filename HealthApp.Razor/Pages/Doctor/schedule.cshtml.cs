using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Razor.Pages.Doctor
{
    [Authorize(Roles = "Doctor")] // S'assurer que seuls les médecins peuvent voir cette page
    public class scheduleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Dictionary<string, object>> Events { get; set; } = new List<Dictionary<string, object>>();

        public scheduleModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void OnGet()
        {
            Console.WriteLine("?? OnGet() de Schedule.cshtml.cs a été appelé !");

            // ?? Récupérer l'ID du médecin connecté
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("? Utilisateur non connecté !");
                return;
            }

            // ?? Récupérer les rendez-vous du médecin connecté
            Events = _context.Appointments
                .Where(a => a.DoctorId == userId)
                .Select(a => new Dictionary<string, object>
                {
                    { "id", a.Id },
                    { "title", $"{_context.Users.Where(x => x.Id == a.UserId).FirstOrDefault()!.UserName}" },
                    { "start", DateTimeOffset.FromUnixTimeMilliseconds(a.DateTimeMilli).UtcDateTime.ToString("yyyy-MM-ddTHH:mm") },
                    { "end", DateTimeOffset.FromUnixTimeMilliseconds(a.DateTimeMilli).UtcDateTime.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "color", a.IsConfirmed == 1 ? "#28a745" :a.IsConfirmed == -1? "#ff0000" : "#ffc107" },
                    { "IsConfirmed", a.IsConfirmed }
                })
                .ToList();

            Console.WriteLine($"?? {Events.Count} événements trouvés pour le médecin {userId}");
        }
    }
}
