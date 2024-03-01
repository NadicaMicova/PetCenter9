using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCenter9.Data;
using PetCenter9.Models;

namespace PetCenter9.Controllers
{

    public class PetsController : Controller
    {
        private readonly PetCenter9Context _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PetsController(PetCenter9Context context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }


        [Authorize(Roles = "Admin,Sales")]
        // GET: Pets
        public async Task<IActionResult> Index()
        {
            var petCenter9Context = _context.Pets.Include(p => p.Owners);
            return View(await petCenter9Context.ToListAsync());
        }
        [Authorize(Roles = "Admin,Sales")]
        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owners)
                .Include(p => p.Vaccine)  // Include the vaccines navigation property
                .FirstOrDefaultAsync(m => m.PetsId == id);

            if (pet == null)
            {
                return NotFound();
            }

            // Pass the pet and its associated vaccines to the view
            return View(pet);
        }







        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Pets == null)
        //    {
        //        return NotFound();
        //    }

        //    var pets = await _context.Pets
        //        .Include(p => p.Owners)
        //        .FirstOrDefaultAsync(m => m.PetsId == id);
        //    if (pets == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pets);
        //}
        [Authorize(Roles = "Admin,Sales")]
        // GET: Pets/Create
        public IActionResult Create()
        {
            ViewData["OwnersId"] = new SelectList(_context.Owners, "OwnersId", "Email");
            return View();
        }
        [Authorize(Roles = "Admin,Sales")]
        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pets pet, IFormFile petPictureFile)
        {
            if (ModelState.IsValid)
            {
                if (petPictureFile != null && petPictureFile.Length > 0)
                {
                    // Генерирање на уникатно име за сликата
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(petPictureFile.FileName);

                    // Патеката каде ќе се зачува сликата
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "pet-images", fileName);

                    // Зачувување на сликата на серверот
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await petPictureFile.CopyToAsync(fileStream);
                    }

                    // Поставување на патеката до сликата во објектот за сопственикот
                    pet.PetPictureURL = "/pet-images/" + fileName;
                }

                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        [Authorize(Roles = "Admin")]
        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            var availableVaccines = await _context.Vaccines.ToListAsync();

            ViewData["Vaccines"] = new MultiSelectList(availableVaccines, "VaccinesId", "Name");

            ViewData["OwnersId"] = new SelectList(_context.Owners, "OwnersId", "Email", pet.OwnersId);

            return View(pet);






            //if (id == null || _context.Pets == null)
            //{
            //    return NotFound();
            //}

            //var pets = await _context.Pets.FindAsync(id);
            //if (pets == null)
            //{
            //    return NotFound();
            //}
            //ViewData["OwnersId"] = new SelectList(_context.Owners, "OwnersId", "Email", pets.OwnersId);
            //return View(pets);
        }
        [Authorize(Roles = "Admin")]
        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("PetsId,PetPictureURL,Name,Age,OwnersId")] Pets pets, int[] Vaccine)
        {
            if (id != pets.PetsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the pet entity
                    _context.Update(pets);


                    // Add the selected vaccines to the pet
                    if (Vaccine != null)
                    {
                        foreach (var vaccineId in Vaccine)
                        {
                            var vaccine = await _context.Vaccines.FindAsync(vaccineId);
                            if (vaccine != null)
                            {
                                pets.Vaccine.Add(vaccine);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetsExists(pets.PetsId))
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

            // If ModelState is not valid, populate ViewData and return the view
            var availableVaccines = await _context.Vaccines.ToListAsync();
            ViewData["Vaccines"] = new MultiSelectList(availableVaccines, "VaccinesId", "Name");
            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnersId", "Email", pets.OwnersId);
            return View(pets);
        }









        //public async Task<IActionResult> Edit(int id, [Bind("PetsId,PetPictureURL,Name,Age,OwnersId")] Pets pets)
        //{
        //    if (id != pets.PetsId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(pets);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PetsExists(pets.PetsId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["OwnersId"] = new SelectList(_context.Owners, "OwnersId", "Email", pets.OwnersId);
        //    return View(pets);
        //}
        [Authorize(Roles = "Admin")]
        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pets == null)
            {
                return NotFound();
            }

            var pets = await _context.Pets
                .Include(p => p.Owners)
                .FirstOrDefaultAsync(m => m.PetsId == id);
            if (pets == null)
            {
                return NotFound();
            }

            return View(pets);
        }
        [Authorize(Roles = "Admin")]
        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pets == null)
            {
                return Problem("Entity set 'PetCenter9Context.Pets'  is null.");
            }
            var pets = await _context.Pets.FindAsync(id);
            if (pets != null)
            {
                _context.Pets.Remove(pets);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetsExists(int id)
        {
          return (_context.Pets?.Any(e => e.PetsId == id)).GetValueOrDefault();
        }
    }
}
