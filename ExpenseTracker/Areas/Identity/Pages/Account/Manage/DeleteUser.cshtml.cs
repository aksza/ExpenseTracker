using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExpenseTracker.Areas.Identity.Pages.Account.Manage
{
    public class DeleteUserModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public DeleteUserModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Soft delete
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error occurred while trying to delete your account.";
                return Page();
            }

            await _signInManager.SignOutAsync();
            StatusMessage = "Your account has been deleted.";
            return RedirectToPage("/Index", new { area = "" });
        }
    }
}
