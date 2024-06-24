using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment3.Models;
using System.Text.RegularExpressions;

namespace Assignment3.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly Assignment3Context _context;

        public AppUsersController(Assignment3Context context)
        {
            _context = context;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            return _context.AppUser != null
                ? View(await _context.AppUser.ToListAsync())
                : Problem("Entity set 'Assignment3Context.AppUser'  is null.");
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AppUser == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUser
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Fullname,Address,Email,Password")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                string password = appUser.Password;
                string checkPassword = ValidatePassword(password);
                if (!checkPassword.Equals(""))
                {
                    TempData["error"] += checkPassword;
                    return View(appUser);
                }

                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AppUser == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUser.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Fullname,Address,Email,Password")] AppUser appUser)
        {
            if (id != appUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string password = appUser.Password;
                    string checkPassword = ValidatePassword(password);
                    if (!checkPassword.Equals(""))
                    {
                        TempData["error"] += checkPassword;
                        return View(appUser);
                    }

                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AppUser == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUser
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AppUser == null)
            {
                return Problem("Entity set 'Assignment3Context.AppUser'  is null.");
            }

            var appUser = await _context.AppUser.FindAsync(id);
            if (appUser != null)
            {
                _context.AppUser.Remove(appUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
            return (_context.AppUser?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private string ValidatePassword(string password)
        {
            var hasNum = new Regex(@"[0-9]+");
            var hasUpper = new Regex(@"[A-Z]+");
            var hasSpec = new Regex(@"[.,!;'@#$%^&-_+=]+");

            string notification = string.Empty;

            if (password.Length < 8)
            {
                notification += "Password must have at least 8 characters!\n";
            }

            // if (!hasNum.IsMatch(password))
            // {
            //     notification += "Password must have at least 1 numeric!\n";
            // }

            if (!hasUpper.IsMatch(password))
            {
                notification += "Password must have at least 1 uppercase character!\n";
            }

            // if (!hasSpec.IsMatch(password))
            // {
            //     notification += "Password must have at least 1 special character!\n";
            // }

            return notification;
        }
    }
}