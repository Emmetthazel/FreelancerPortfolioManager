using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;

namespace FreelancerPortfolioManager.Controllers
{
    public class ContratsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContratsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contrats
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Contrats.Include(c => c.Projet);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Contrats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contrat = await _context.Contrats
                .Include(c => c.Projet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrat == null) return NotFound();

            return View(contrat);
        }

        // GET: Contrats/Create
        public IActionResult Create()
        {
            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre");
            return View();
        }

        // POST: Contrats/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reference,DateSignature,Montant,Conditions,Statut,ProjetId")] Contrat contrat)
        {
            // validate projet exists to avoid FK/validation errors
            if (!_context.Projets.Any(p => p.Id == contrat.ProjetId))
            {
                ModelState.AddModelError("ProjetId", "Projet introuvable.");
            }

            // Initialiser DateCreation automatiquement
            contrat.DateCreation = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(contrat);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contrat créé avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", contrat.ProjetId);
            return View(contrat);
        }

        // GET: Contrats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contrat = await _context.Contrats.FindAsync(id);
            if (contrat == null) return NotFound();

            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", contrat.ProjetId);
            return View(contrat);
        }

        // POST: Contrats/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reference,DateCreation,DateSignature,Montant,Conditions,Statut,ProjetId")] Contrat contrat)
        {
            if (id != contrat.Id) return NotFound();

            if (!_context.Projets.Any(p => p.Id == contrat.ProjetId))
            {
                ModelState.AddModelError("ProjetId", "Projet introuvable.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contrat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContratExists(contrat.Id)) return NotFound();
                    throw;
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", contrat.ProjetId);
            return View(contrat);
        }

        // GET: Contrats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contrat = await _context.Contrats
                .Include(c => c.Projet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrat == null) return NotFound();

            return View(contrat);
        }

        // POST: Contrats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrat = await _context.Contrats.FindAsync(id);
            if (contrat != null) _context.Contrats.Remove(contrat);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContratExists(int id)
        {
            return _context.Contrats.Any(e => e.Id == id);
        }
    }
}
