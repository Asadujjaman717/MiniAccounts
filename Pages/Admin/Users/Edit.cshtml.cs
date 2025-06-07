using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiniAccounts.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string UserEmail { get; set; }

        [BindProperty]
        public List<RoleCheckbox> Roles { get; set; }

        public class RoleCheckbox
        {
            public string RoleName { get; set; }

            [Display(Name = "Assigned")]
            public bool IsSelected { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            UserId = user.Id;
            UserEmail = user.Email;
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.AsNoTracking().ToListAsync();

            Roles = allRoles.Select(role => new RoleCheckbox
            {
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return NotFound();

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

            var rolesToAdd = selectedRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(selectedRoles);

            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            TempData["SuccessMessage"] = "Roles updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}