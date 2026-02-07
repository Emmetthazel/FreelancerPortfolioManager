using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;

namespace FreelancerPortfolioManager.Controllers
{
    public class ProjetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projets.Include(p => p.Client).Include(p => p.Freelancer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projet = await _context.Projets
                .Include(p => p.Client)
                .Include(p => p.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projet == null)
            {
                return NotFound();
            }

            return View(projet);
        }

        // GET: Projets/Create
        public IActionResult Create()
        {
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email");
            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise");
            return View();
        }

        // POST: Projets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,DateDebut,DateFin,Budget,Statut,TauxAvancement,FreelancerId,ClientId")] Projet projet)
        {
            // validate foreign keys
            if (!_context.Freelancers.Any(f => f.Id == projet.FreelancerId))
                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");
            if (!_context.Clients.Any(c => c.Id == projet.ClientId))
                ModelState.AddModelError("ClientId", "Client introuvable.");

            if (ModelState.IsValid)
            {
                _context.Add(projet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Projet créé avec succès !";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", projet.FreelancerId);
            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", projet.ClientId);
            return View(projet);
        }

        // GET: Projets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projet = await _context.Projets.FindAsync(id);
            if (projet == null)
            {
                return NotFound();
            }

            // Use meaningful display fields and AsNoTracking
            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", projet.ClientId);
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", projet.FreelancerId);
            return View(projet);
        }

        // POST: Projets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DateDebut,DateFin,Budget,Statut,TauxAvancement,FreelancerId,ClientId")] Projet projet)
        {
            if (id != projet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjetExists(projet.Id))
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

            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", projet.ClientId);
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", projet.FreelancerId);
            return View(projet);
        }

        // GET: Projets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projet = await _context.Projets
                .Include(p => p.Client)
                .Include(p => p.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projet == null)
            {
                return NotFound();
            }

            return View(projet);
        }

        // POST: Projets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projet = await _context.Projets.FindAsync(id);
            if (projet != null)
            {
                _context.Projets.Remove(projet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjetExists(int id)
        {
            return _context.Projets.Any(e => e.Id == id);
        }
    }
}
