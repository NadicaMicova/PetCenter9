using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCenter9.Data;
using PetCenter9.Models;

namespace PetCenter9.Controllers
{

    public class VaccinesController : Controller
    {
        private readonly PetCenter9Context _context;

        public VaccinesController(PetCenter9Context context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Sales")]
        // GET: Vaccines
        public async Task<IActionResult> Index()
        {
            var vaccines = await _context.Vaccines.Include(v => v.Pet).ToListAsync();
            ViewBag.Pet = await _context.Pets.ToListAsync();
            return View(vaccines);
        }
        [Authorize(Roles = "Admin,Sales")]
        // GET: Vaccines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vaccines == null)
            {
                return NotFound();
            }

            var vaccines = await _context.Vaccines
                .Include(p => p.Pet)
                .FirstOrDefaultAsync(m => m.VaccinesId == id);
            if (vaccines == null)
            {
                return NotFound();
            }

            return View(vaccines);
        }
        [Authorize(Roles = "Admin,Sales")]
        // GET: Vaccines/Create
        public IActionResult Create()
        {
            ViewBag.Pet = _context.Pets.ToList();
            return View();
        }
        [Authorize(Roles = "Admin,Sales")]
        // POST: Vaccines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VaccinesId,Name,Pets")] Vaccines vaccines)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vaccines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vaccines);
        }
        [Authorize(Roles = "Admin")]
        // GET: Vaccines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vaccines == null)
            {
                return NotFound();
            }

            var vaccines = await _context.Vaccines.FindAsync(id);
            if (vaccines == null)
            {
                return NotFound();
            }
            var availablePets = await _context.Pets.ToListAsync();
            ViewData["Pet"] = new MultiSelectList(availablePets, "PetsId", "Name");
            return View(vaccines);
        }
        [Authorize(Roles = "Admin")]
        // POST: Vaccines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VaccinesId,Name,Pet")] Vaccines vaccine, int[] Pet)


        {
            if (id != vaccine.VaccinesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vaccine);

                    // Add the selected pets to the vaccine
                    if (Pet != null)
                    {
                        foreach (var petId in Pet)
                        {
                            var pet = await _context.Pets.FindAsync(petId);
                            if (pet != null)
                            {
                                vaccine.Pet.Add(pet);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VaccinesExists(vaccine.VaccinesId))
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

            var availablePets = await _context.Pets.ToListAsync();
            ViewData["Pets"] = new MultiSelectList(availablePets, "PetsId", "Name");
            return View(vaccine);
        }

        [Authorize(Roles = "Admin")]
        // GET: Vaccines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vaccines == null)
            {
                return NotFound();
            }

            var vaccines = await _context.Vaccines
                .FirstOrDefaultAsync(m => m.VaccinesId == id);
            if (vaccines == null)
            {
                return NotFound();
            }

            return View(vaccines);
        }
        [Authorize(Roles = "Admin")]
        // POST: Vaccines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vaccines == null)
            {
                return Problem("Entity set 'PetCenter9Context.Vaccines'  is null.");
            }
            var vaccines = await _context.Vaccines.FindAsync(id);
            if (vaccines != null)
            {
                _context.Vaccines.Remove(vaccines);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VaccinesExists(int id)
        {
          return (_context.Vaccines?.Any(e => e.VaccinesId == id)).GetValueOrDefault();
        }
    }
}
