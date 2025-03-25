using Microsoft.AspNetCore.Identity;

public class AssignRoleMiddleware
{
    private readonly RequestDelegate _next;

    public AssignRoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    await userManager.AddToRoleAsync(user, "Patient");
                    await signInManager.RefreshSignInAsync(user);
                }
            }
        }

        await _next(context);
    }
}