using Microsoft.AspNetCore.Identity;

namespace HealthApp.Razor.Data
{
    //ca sert peu etre a rien en fait
    public class User : IdentityUser
    {
        public string Role { get; set; } // "Médecin" ou "Patient"
    }
}
