using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment3.Models;
using Microsoft.AspNetCore.SignalR;

namespace Assignment3.Controllers
{
    public class PostsController : Controller
    {
        private readonly Assignment3Context _context;
        private readonly IHubContext<SignalrServer> _signalRHub;
        private readonly IConfiguration Configuration;

        public PostsController(Assignment3Context context, IHubContext<SignalrServer> signalRHub,
            IConfiguration configuration)
        {
            _context = context;
            _signalRHub = signalRHub;
            Configuration = configuration;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchTitle, string searchContent,
            string searchStartDate, string searchEndDate)
        {
            DateTime minDate = Convert.ToDateTime(searchStartDate).Date;
            DateTime maxDate = Convert.ToDateTime(searchEndDate).Date;

            List<Post> posts = await
                _context.Post
                    .Where(p => p.PublishStatus == 1)
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .OrderByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.UpdatedDate)
                    .ToListAsync();

            if (!String.IsNullOrEmpty(searchTitle))
            {
                posts = posts.Where(p => p.Title.ToLower().Trim()
                    .Contains(searchTitle.ToLower().Trim())).ToList();
            }

            if (!String.IsNullOrEmpty(searchContent))
            {
                posts = posts.Where(p => p.Content.ToLower().Trim()
                    .Contains(searchContent.ToLower().Trim())).ToList();
            }

            if (!String.IsNullOrEmpty(searchStartDate))
            {
                posts = posts.Where(p => p.CreatedDate.Date >= minDate.Date).ToList();
            }

            if (!String.IsNullOrEmpty(searchEndDate))
            {
                posts = posts.Where(p => p.CreatedDate.Date <= maxDate.Date).ToList();
            }

            ViewData["searchTitle"] = searchTitle;
            ViewData["searchContent"] = searchContent;
            ViewData["searchStartDate"] = searchStartDate;
            ViewData["searchEndDate"] = searchEndDate;

            return View(posts);
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            var res = _context.Post
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.UpdatedDate)
                .ToList();
            return Ok(res);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.AppUser, "UserId", "Fullname");
            ViewData["CategoryId"] = new SelectList(_context.PostCategory, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PostId,AuthorId,Title,Content,PublishStatus,CategoryId")]
            Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedDate = DateTime.Now;
                post.UpdatedDate = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadPosts");
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.AppUser, "UserId", "Fullname", post.AuthorId);
            ViewData["CategoryId"] =
                new SelectList(_context.PostCategory, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = new SelectList(_context.AppUser, "UserId", "Fullname", post.AuthorId);
            ViewData["CategoryId"] =
                new SelectList(_context.PostCategory, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PostId,
            [Bind("PostId,AuthorId,CreatedDate,Title,Content,PublishStatus,CategoryId")]
            Post post)
        {
            if (PostId != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.UpdatedDate = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadPosts");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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

            ViewData["AuthorId"] = new SelectList(_context.AppUser, "UserId", "Fullname", post.AuthorId);
            ViewData["CategoryId"] =
                new SelectList(_context.PostCategory, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PostId)
        {
            if (_context.Post == null)
            {
                return Problem("Entity set 'Assignment3Context.Post'  is null.");
            }

            var post = await _context.Post.FindAsync(PostId);
            if (post != null)
            {
                _context.Post.Remove(post);
            }

            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadPosts");
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Post?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}