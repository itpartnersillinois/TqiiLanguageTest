using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TqiiLanguageTest.BusinessLogic;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.Pages.Admin {

    public class PermissionsModel : PageModel {
        private readonly LanguageDbContext _context;
        private readonly PermissionsHandler _permissions;

        public PermissionsModel(LanguageDbContext context, PermissionsHandler permissions) {
            _context = context;
            _permissions = permissions;
        }

        [BindProperty]
        public int NewId { get; set; }

        [BindProperty]
        public bool NewIsAdmin { get; set; } = default!;

        [BindProperty]
        public bool NewIsItemWriter { get; set; } = default!;

        [BindProperty]
        public bool NewIsReviewer { get; set; } = default!;

        [BindProperty]
        public string NewPermissionEmail { get; set; } = default!;

        public IList<Permission> Permissions { get; set; } = default!;

        public async Task OnGetAsync(int id) {
            if (!_permissions.IsAdmin(User.Identity?.Name ?? "")) {
                throw new Exception("Unauthorized");
            }

            if (_context.Permissions != null) {
                Permissions = await _context.Permissions.OrderBy(r => r.Email).ToListAsync();
            }
            NewId = id;
            if (id != 0) {
                NewPermissionEmail = Permissions.FirstOrDefault(p => p.Id == id)?.Email ?? "";
                NewIsAdmin = Permissions.FirstOrDefault(p => p.Id == id)?.IsAdministrator ?? false;
                NewIsReviewer = Permissions.FirstOrDefault(p => p.Id == id)?.IsReviewer ?? false;
                NewIsItemWriter = Permissions.FirstOrDefault(p => p.Id == id)?.IsItemWriter ?? false;
            }
        }

        public async Task<IActionResult> OnPostAsync() {
            if (_context.Permissions == null) {
                return Page();
            }

            if (NewId == 0) {
                var permission = new Permission {
                    Email = NewPermissionEmail,
                    IsAdministrator = NewIsAdmin,
                    IsItemWriter = NewIsItemWriter,
                    IsReviewer = NewIsReviewer
                };
                _context.Permissions.Add(permission);
            } else if (string.IsNullOrWhiteSpace(NewPermissionEmail)) {
                var basePermission = _context.Permissions.First(t => t.Id == NewId);
                _context.Permissions.Remove(basePermission);
            } else {
                var basePermission = _context.Permissions.AsNoTracking().First(t => t.Id == NewId);
                basePermission.Email = NewPermissionEmail;
                basePermission.IsAdministrator = NewIsAdmin;
                basePermission.IsItemWriter = NewIsItemWriter;
                basePermission.IsReviewer = NewIsReviewer;
                _context.Permissions.Update(basePermission);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Permissions");
        }
    }
}