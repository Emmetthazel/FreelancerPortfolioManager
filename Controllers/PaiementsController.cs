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
    public class PaiementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaiementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Paiements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Paiements.Include(p => p.Facture);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Paiements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var paiement = await _context.Paiements
                .Include(p => p.Facture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paiement == null) return NotFound();

            return View(paiement);
        }

        // GET: Paiements/Create
        public IActionResult Create()
        {
            ViewData["FactureId"] = new SelectList(_context.Factures.AsNoTracking(), "Id", "Numero");
            return View();
        }

        // POST: Paiements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Montant,DatePaiement,MoyenPaiement,Reference,FactureId")] Paiement paiement)
        {
            if (!_context.Factures.Any(f => f.Id == paiement.FactureId))
            {
                ModelState.AddModelError("FactureId", "Facture introuvable.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(paiement);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Paiement créé avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["FactureId"] = new SelectList(_context.Factures.AsNoTracking(), "Id", "Numero", paiement.FactureId);
            return View(paiement);
        }

        // GET: Paiements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement == null) return NotFound();

            ViewData["FactureId"] = new SelectList(_context.Factures.AsNoTracking(), "Id", "Numero", paiement.FactureId);
            return View(paiement);
        }

        // POST: Paiements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Montant,DatePaiement,MoyenPaiement,Reference,FactureId")] Paiement paiement)
        {
            if (id != paiement.Id) return NotFound();

            if (!_context.Factures.Any(f => f.Id == paiement.FactureId))
            {
                ModelState.AddModelError("FactureId", "Facture introuvable.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paiement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaiementExists(paiement.Id)) return NotFound();
                    throw;
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["FactureId"] = new SelectList(_context.Factures.AsNoTracking(), "Id", "Numero", paiement.FactureId);
            return View(paiement);
        }

        // GET: Paiements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var paiement = await _context.Paiements
                .Include(p => p.Facture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paiement == null) return NotFound();

            return View(paiement);
        }

        // POST: Paiements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement == null) return NotFound();

            try
            {
                _context.Paiements.Remove(paiement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError(string.Empty, "Impossible de supprimer le paiement : " + dbEx.GetBaseException().Message);
                ViewData["FactureId"] = new SelectList(_context.Factures.AsNoTracking(), "Id", "Numero", paiement.FactureId);
                return View("Delete", paiement);
            }
        }

        private bool PaiementExists(int id) => _context.Paiements.Any(e => e.Id == id);
    }
}
