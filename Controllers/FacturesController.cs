using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.Extensions.Logging;

namespace FreelancerPortfolioManager.Controllers
{
    public class FacturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FacturesController> _logger;

        public FacturesController(ApplicationDbContext context, ILogger<FacturesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Factures
        public async Task<IActionResult> Index()
        {
            // Recalculer le MontantTTC pour les factures qui ont MontantTTC = 0
            var facturesAvecTTCZero = await _context.Factures
                .Where(f => f.MontantTTC == 0 && f.MontantHT > 0)
                .ToListAsync();

            if (facturesAvecTTCZero.Any())
            {
                foreach (var facture in facturesAvecTTCZero)
                {
                    facture.MontantTVA = facture.MontantHT * (facture.TauxTVA / 100);
                    facture.MontantTTC = facture.MontantHT + facture.MontantTVA;
                }
                await _context.SaveChangesAsync();
            }

            var applicationDbContext = _context.Factures.Include(f => f.Freelancer).Include(f => f.Projet);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Factures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Factures
                .Include(f => f.Freelancer)
                .Include(f => f.Projet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            return View(facture);
        }

        // GET: Factures/Create
        public IActionResult Create()
        {
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email");
            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre");
            return View();
        }

        // POST: Factures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,DateEmission,DateEcheance,MontantHT,Statut,TauxTVA,ProjetId,FreelancerId")] Facture facture)
        {
            // validate foreign keys to avoid binder/FK errors
            if (!_context.Freelancers.Any(f => f.Id == facture.FreelancerId))
                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");
            if (!_context.Projets.Any(p => p.Id == facture.ProjetId))
                ModelState.AddModelError("ProjetId", "Projet introuvable.");

            // Calculer automatiquement MontantTVA et MontantTTC
            facture.MontantTVA = facture.MontantHT * (facture.TauxTVA / 100);
            facture.MontantTTC = facture.MontantHT + facture.MontantTVA;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(facture);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Facture créée avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", facture.FreelancerId);
            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", facture.ProjetId);
            return View(facture);
        }

        // GET: Factures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var facture = await _context.Factures.FindAsync(id);
            if (facture == null) return NotFound();

            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", facture.FreelancerId);
            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", facture.ProjetId);
            return View(facture);
        }

        // POST: Factures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,DateEmission,DateEcheance,MontantHT,Statut,TauxTVA,ProjetId,FreelancerId")] Facture facture)
        {
            if (id != facture.Id) return NotFound();

            if (!_context.Freelancers.Any(f => f.Id == facture.FreelancerId))
                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");
            if (!_context.Projets.Any(p => p.Id == facture.ProjetId))
                ModelState.AddModelError("ProjetId", "Projet introuvable.");

            // Charger l'entité existante pour préserver les autres champs
            var existingFacture = await _context.Factures.FindAsync(id);
            if (existingFacture == null) return NotFound();

            // Calculer automatiquement MontantTVA et MontantTTC
            facture.MontantTVA = facture.MontantHT * (facture.TauxTVA / 100);
            facture.MontantTTC = facture.MontantHT + facture.MontantTVA;

            if (ModelState.IsValid)
            {
                try
                {
                    // Mettre à jour uniquement les champs modifiables
                    existingFacture.Numero = facture.Numero;
                    existingFacture.DateEmission = facture.DateEmission;
                    existingFacture.DateEcheance = facture.DateEcheance;
                    existingFacture.MontantHT = facture.MontantHT;
                    existingFacture.TauxTVA = facture.TauxTVA;
                    existingFacture.MontantTVA = facture.MontantTVA;
                    existingFacture.MontantTTC = facture.MontantTTC;
                    existingFacture.Statut = facture.Statut;
                    existingFacture.ProjetId = facture.ProjetId;
                    existingFacture.FreelancerId = facture.FreelancerId;

                    _context.Update(existingFacture);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Facture modifiée avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactureExists(facture.Id)) return NotFound();
                    throw;
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", facture.FreelancerId);
            ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", facture.ProjetId);
            return View(existingFacture);
        }

        // GET: Factures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("Delete called with id={Id}", id);
            if (id == null)
            {
                _logger.LogWarning("Delete: id was null");
                return NotFound();
            }

            var facture = await _context.Factures
                .Include(f => f.Freelancer)
                .Include(f => f.Projet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facture == null)
            {
                _logger.LogWarning("Delete: facture with id={Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Delete: facture found id={Id}", id);
            return View(facture);
        }

        // POST: Factures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("DeleteConfirmed called with id={Id}", id);
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                _logger.LogWarning("DeleteConfirmed: facture id={Id} not found", id);
                return NotFound();
            }

            try
            {
                _context.Factures.Remove(facture);
                await _context.SaveChangesAsync();
                _logger.LogInformation("DeleteConfirmed: facture id={Id} deleted", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error deleting facture id={Id}", id);
                ModelState.AddModelError(string.Empty, "Impossible de supprimer la facture : " + dbEx.GetBaseException().Message);

                // Re-populate selects and return confirmation view with error
                ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", facture.FreelancerId);
                ViewData["ProjetId"] = new SelectList(_context.Projets.AsNoTracking(), "Id", "Titre", facture.ProjetId);
                return View("Delete", facture);
            }
        }

        // helper
        private bool FactureExists(int id) => _context.Factures.Any(e => e.Id == id);
    }
}
