using JobPortal.DTO;
using JobPortal.Model;
using Microsoft.AspNetCore.Identity;

namespace JobPortal.Services
{
    public class EmployeeService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EmployeeService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterEmployeeAsync(RegisterEmployeeDto registerEmployeeDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerEmployeeDto.Email,
                Email = registerEmployeeDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerEmployeeDto.Password);

            if (result.Succeeded)
            {
                // Assign an "Employee" role to the user (optional)
                await _userManager.AddToRoleAsync(user, "Employee");
            }

            return result;
        }

        public async Task<SignInResult> LoginEmployeeAsync(LoginDto loginDto)
        {
            return await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
        }
    }
}
