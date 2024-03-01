using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using PetCenter9.Data;
using PetCenter9.Models;
using Microsoft.Extensions.Caching.Memory;

namespace PetCenter9.Controllers
{
    
    public class OwnersController : Controller
    {
        private readonly PetCenter9Context _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IMemoryCache _memoryCache;
        private const string OWNER_KEY = "Owners";

        public OwnersController(PetCenter9Context context, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _memoryCache = memoryCache;
        }

        
        [Authorize(Roles = "Admin,Sales")]
        
        public async Task<IActionResult> Index()
        {
            List<Owners> owners;
            if (!_memoryCache.TryGetValue(OWNER_KEY, out owners))
            {
                owners = await _context.Owners.ToListAsync();
                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetPriority(CacheItemPriority.High);
                _memoryCache.Set(OWNER_KEY, owners, cacheOptions);
            }
            return View(owners);
        }

        [Authorize(Roles = "Admin,Sales")]
        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.Pet) // Вклучувај ги милениците
                .FirstOrDefaultAsync(m => m.OwnersId == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }
        [Authorize(Roles = "Admin,Sales")]
        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Sales")]
        // POST: Owners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Owners owner, IFormFile ownerPictureFile)
        {
            if (ModelState.IsValid)
            {
                if (ownerPictureFile != null && ownerPictureFile.Length > 0)
                {
                    // Генерирање на уникатно име за сликата
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ownerPictureFile.FileName);

                    // Патеката каде ќе се зачува сликата
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                    // Зачувување на сликата на серверот
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ownerPictureFile.CopyToAsync(fileStream);
                    }

                    // Поставување на патеката до сликата во објектот за сопственикот
                    owner.OwnerPictureURL = "/images/" + fileName;
                }

                _context.Add(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }


        [Authorize(Roles = "Admin")]
        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Owners == null)
            {
                return NotFound();
            }

            var owners = await _context.Owners.FindAsync(id);
            if (owners == null)
            {
                return NotFound();
            }
            return View(owners);
        }
        [Authorize(Roles = "Admin")]
        // POST: Owners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnersId,OwnerPictureURL,FirstName,LastName,Age,Email")] Owners owners)
        {
            if (id != owners.OwnersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owners);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnersExists(owners.OwnersId))
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
            return View(owners);
        }
        [Authorize(Roles = "Admin")]
        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Owners == null)
            {
                return NotFound();
            }

            var owners = await _context.Owners
                .FirstOrDefaultAsync(m => m.OwnersId == id);
            if (owners == null)
            {
                return NotFound();
            }

            return View(owners);
        }
        [Authorize(Roles = "Admin")]
        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Owners == null)
            {
                return Problem("Entity set 'PetCenter9Context.Owners'  is null.");
            }
            var owners = await _context.Owners.FindAsync(id);
            if (owners != null)
            {
                _context.Owners.Remove(owners);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnersExists(int id)
        {
          return (_context.Owners?.Any(e => e.OwnersId == id)).GetValueOrDefault();
        }
    }
}
