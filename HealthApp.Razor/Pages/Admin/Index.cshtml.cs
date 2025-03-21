using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using HealthApp.Razor.Data;

public class AdminModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    
    public List<Appointment> Appointments { get; set; }

    public AdminModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public List<IdentityUser> Users { get; set; }

    [BindProperty]
    public Dictionary<string, string> UserRoles { get; set; } = new();

    public SelectList RoleSelectList { get; set; }

    public async Task OnGetAsync()
    {
        Users = _userManager.Users.ToList();
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        RoleSelectList = new SelectList(roles);
        
        Appointments = _context.Appointments.ToList();

        foreach (var user in Users)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            UserRoles[user.Id] = role;
        }
    }

    public async Task<IActionResult> OnPostSaveUserRolesAsync()
    {
        foreach (var userRole in UserRoles)
        {
            var user = await _userManager.FindByIdAsync(userRole.Key);
            if (user == null) continue;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = userRole.Value;

            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(newRole))
                await _userManager.AddToRoleAsync(user, newRole);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAppointmentAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
    
    public string GetDoctorName(string doctorId)
    {
        return _context.Users.Find(doctorId)?.UserName ?? doctorId;
    }

    public string GetPatientName(string patientId)
    {
        return _context.Users.Find(patientId)?.UserName ?? patientId;
    }
}
