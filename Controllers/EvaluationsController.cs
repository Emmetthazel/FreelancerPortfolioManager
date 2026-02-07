using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using FreelancerPortfolioManager.Services;

namespace FreelancerPortfolioManager.Controllers
{
    public class EvaluationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEvaluationService _evaluationService;

        public EvaluationsController(ApplicationDbContext context, IEvaluationService evaluationService)
        {
            _context = context;
            _evaluationService = evaluationService;
        }

        // GET: Evaluations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Evaluations.Include(e => e.Client).Include(e => e.Freelancer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Evaluations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var evaluation = await _context.Evaluations
                .Include(e => e.Client)
                .Include(e => e.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evaluation == null) return NotFound();

            return View(evaluation);
        }

        // GET: Evaluations/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise");
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email");
            return View();
        }

        // POST: Evaluations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Note,Commentaire,DateEvaluation,ClientId,FreelancerId")] Evaluation evaluation)
        {
            // validate FKs to avoid FK/ModelState issues
            if (!_context.Clients.Any(c => c.Id == evaluation.ClientId))
                ModelState.AddModelError("ClientId", "Client introuvable.");
            if (!_context.Freelancers.Any(f => f.Id == evaluation.FreelancerId))
                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _evaluationService.CreateEvaluationAsync(evaluation);
                    TempData["Success"] = "Évaluation créée avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", evaluation.ClientId);
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", evaluation.FreelancerId);
            return View(evaluation);
        }

        // GET: Evaluations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", evaluation.ClientId);
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", evaluation.FreelancerId);
            return View(evaluation);
        }

        // POST: Evaluations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Note,Commentaire,DateEvaluation,ClientId,FreelancerId")] Evaluation evaluation)
        {
            if (id != evaluation.Id) return NotFound();

            if (!_context.Clients.Any(c => c.Id == evaluation.ClientId))
                ModelState.AddModelError("ClientId", "Client introuvable.");
            if (!_context.Freelancers.Any(f => f.Id == evaluation.FreelancerId))
                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvaluation = await _context.Evaluations.FindAsync(id);
                    if (existingEvaluation == null) return NotFound();

                    // Get the old ClientId before update to update its note if it changed
                    int oldClientId = existingEvaluation.ClientId;

                    existingEvaluation.Note = evaluation.Note;
                    existingEvaluation.Commentaire = evaluation.Commentaire;
                    existingEvaluation.DateEvaluation = evaluation.DateEvaluation;
                    existingEvaluation.ClientId = evaluation.ClientId;
                    existingEvaluation.FreelancerId = evaluation.FreelancerId;

                    await _evaluationService.UpdateEvaluationAsync(existingEvaluation);

                    // If client changed, update the old client's note too
                    if (oldClientId != evaluation.ClientId)
                    {
                        await UpdateClientNoteAsync(oldClientId);
                    }

                    TempData["Success"] = "Évaluation modifiée avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Evaluations.Any(e => e.Id == evaluation.Id)) return NotFound();
                    throw;
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            ViewData["ClientId"] = new SelectList(_context.Clients.AsNoTracking(), "Id", "NomEntreprise", evaluation.ClientId);
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", evaluation.FreelancerId);
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var evaluation = await _context.Evaluations
                .Include(e => e.Client)
                .Include(e => e.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evaluation == null) return NotFound();

            return View(evaluation);
        }

        // POST: Evaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation != null)
            {
                await _evaluationService.DeleteEvaluationAsync(id);
                TempData["Success"] = "Évaluation supprimée avec succès !";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task UpdateClientNoteAsync(int clientId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.ClientId == clientId)
                .ToListAsync();

            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.NoteGlobale = evaluations.Any() ? (float)evaluations.Average(e => e.Note) : 0;
                await _context.SaveChangesAsync();
            }
        }
    }
}
