//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using FreelancerPortfolioManager.Data;
//using FreelancerPortfolioManager.Models;

//namespace FreelancerPortfolioManager.Controllers
//{
//    public class RapportsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public RapportsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Rapports
//        public async Task<IActionResult> Index()
//        {
//            var applicationDbContext = _context.Rapports.Include(r => r.Freelancer);
//            return View(await applicationDbContext.ToListAsync());
//        }

//        // GET: Rapports/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null) return NotFound();

//            var rapport = await _context.Rapports
//                .Include(r => r.Freelancer)
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (rapport == null) return NotFound();

//            return View(rapport);
//        }

//        // GET: Rapports/Create
//        public IActionResult Create()
//        {
//            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email");
//            return View();
//        }

//        // POST: Rapports/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Titre,DateGeneration,TypeRapport,DateDebut,DateFin,Contenu,CheminPDF,FreelancerId")] Rapport rapport)
//        {
//            if (!_context.Freelancers.Any(f => f.Id == rapport.FreelancerId))
//                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Add(rapport);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction(nameof(Index));
//                }
//                catch (DbUpdateException dbEx)
//                {
//                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
//                }
//            }

//            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", rapport.FreelancerId);
//            return View(rapport);
//        }

//        // GET: Rapports/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null) return NotFound();

//            var rapport = await _context.Rapports.FindAsync(id);
//            if (rapport == null) return NotFound();

//            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", rapport.FreelancerId);
//            return View(rapport);
//        }

//        // POST: Rapports/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,DateGeneration,TypeRapport,DateDebut,DateFin,Contenu,CheminPDF,FreelancerId")] Rapport rapport)
//        {
//            if (id != rapport.Id) return NotFound();

//            if (!_context.Freelancers.Any(f => f.Id == rapport.FreelancerId))
//                ModelState.AddModelError("FreelancerId", "Freelancer introuvable.");

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(rapport);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction(nameof(Index));
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!_context.Rapports.Any(e => e.Id == rapport.Id)) return NotFound();
//                    throw;
//                }
//                catch (DbUpdateException dbEx)
//                {
//                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
//                }
//            }

//            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", rapport.FreelancerId);
//            return View(rapport);
//        }

//        // GET: Rapports/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();

//            var rapport = await _context.Rapports
//                .Include(r => r.Freelancer)
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (rapport == null) return NotFound();

//            return View(rapport);
//        }

//        // POST: Rapports/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var rapport = await _context.Rapports.FindAsync(id);
//            if (rapport == null) return NotFound();

//            try
//            {
//                _context.Rapports.Remove(rapport);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            catch (DbUpdateException dbEx)
//            {
//                ModelState.AddModelError(string.Empty, "Impossible de supprimer le rapport : " + dbEx.GetBaseException().Message);
//                ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", rapport.FreelancerId);
//                return View("Delete", rapport);
//            }
//        }
//    }
//}


using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using FreelancerPortfolioManager.Services;

namespace FreelancerPortfolioManager.Controllers
{
    [Authorize]
    public class RapportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRapportService _rapportService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RapportsController(ApplicationDbContext context, IRapportService rapportService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _rapportService = rapportService;
            _userManager = userManager;
        }

        // GET: Rapports
        public async Task<IActionResult> Index()
        {
            int freelancerId = await GetCurrentFreelancerIdAsync();
            
            IEnumerable<Rapport> rapports;
            
            if (freelancerId == 0)
            {
                // Aucun freelancer trouvé pour cet utilisateur - afficher tous les rapports
                // pour permettre à l'utilisateur de voir ce qu'il a créé
                rapports = await _context.Rapports
                    .Include(r => r.Freelancer)
                    .OrderByDescending(r => r.DateGeneration)
                    .ToListAsync();
            }
            else
            {
                rapports = await _rapportService.GetAllRapportsAsync(freelancerId);
            }
            
            return View(rapports);
        }

        // GET: Rapports/Create
        public IActionResult Create()
        {
            ViewBag.TypesRapport = new SelectList(new[]
            {
                new { Value = "Mensuel", Text = "Rapport Mensuel" },
                new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                new { Value = "Annuel", Text = "Rapport Annuel" },
                new { Value = "Personnalise", Text = "Période Personnalisée" }
            }, "Value", "Text");
            
            // Charger la liste des freelancers pour la sélection
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email");
            
            return View();
        }

        // POST: Rapports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,TypeRapport,DateDebut,DateFin,FreelancerId")] Rapport rapport)
        {
            // Valider que le freelancer existe
            if (rapport.FreelancerId <= 0 || !_context.Freelancers.Any(f => f.Id == rapport.FreelancerId))
            {
                ModelState.AddModelError("FreelancerId", "Veuillez sélectionner un freelancer valide.");
            }

            // Initialiser les champs optionnels (requis par la base de données mais pas par l'utilisateur)
            rapport.Contenu = string.Empty;
            rapport.CheminPDF = string.Empty;
            
            // Supprimer les erreurs de validation pour ces champs car ils sont initialisés automatiquement
            ModelState.Remove(nameof(rapport.Contenu));
            ModelState.Remove(nameof(rapport.CheminPDF));

            if (ModelState.IsValid)
            {
                try
                {
                    rapport.DateGeneration = DateTime.Now;

                    _context.Add(rapport);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rapport créé avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, $"Erreur base de données : {dbEx.GetBaseException().Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erreur lors de la création : {ex.Message}");
                }
            }

            ViewBag.TypesRapport = new SelectList(new[]
            {
                new { Value = "Mensuel", Text = "Rapport Mensuel" },
                new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                new { Value = "Annuel", Text = "Rapport Annuel" },
                new { Value = "Personnalise", Text = "Période Personnalisée" }
            }, "Value", "Text", rapport.TypeRapport);
            
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", rapport.FreelancerId);
            return View(rapport);
        }

        // GET: Rapports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rapport = await _rapportService.GetRapportByIdAsync(id.Value);
            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            ViewBag.TypesRapport = new SelectList(new[]
            {
                new { Value = "Mensuel", Text = "Rapport Mensuel" },
                new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                new { Value = "Annuel", Text = "Rapport Annuel" },
                new { Value = "Personnalise", Text = "Période Personnalisée" }
            }, "Value", "Text", rapport.TypeRapport);
            return View(rapport);
        }

        // POST: Rapports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,DateGeneration,TypeRapport,DateDebut,DateFin,FreelancerId")] Rapport rapport)
        {
            if (id != rapport.Id) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    // Charger le rapport existant pour préserver le Contenu
                    var existingRapport = await _context.Rapports.FindAsync(id);
                    if (existingRapport == null) return NotFound();

                    // Mettre à jour uniquement les champs modifiables
                    existingRapport.Titre = rapport.Titre;
                    existingRapport.TypeRapport = rapport.TypeRapport;
                    existingRapport.DateDebut = rapport.DateDebut;
                    existingRapport.DateFin = rapport.DateFin;
                    // Contenu et CheminPDF sont préservés automatiquement

                    _context.Update(existingRapport);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rapport modifié avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Rapports.AnyAsync(e => e.Id == rapport.Id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erreur lors de la modification : {ex.Message}");
                }
            }

            ViewBag.TypesRapport = new SelectList(new[]
            {
                new { Value = "Mensuel", Text = "Rapport Mensuel" },
                new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                new { Value = "Annuel", Text = "Rapport Annuel" },
                new { Value = "Personnalise", Text = "Période Personnalisée" }
            }, "Value", "Text", rapport.TypeRapport);
            return View(rapport);
        }

        // GET: Rapports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rapport = await _rapportService.GetRapportByIdAsync(id.Value);

            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            return View(rapport);
        }

        // GET: Rapports/Generate
        public IActionResult Generate()
        {
            ViewBag.TypesRapport = new SelectList(new[]
            {
                new { Value = "Mensuel", Text = "Rapport Mensuel" },
                new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                new { Value = "Annuel", Text = "Rapport Annuel" },
                new { Value = "Personnalise", Text = "Période Personnalisée" }
            }, "Value", "Text");

            return View();
        }

        // POST: Rapports/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(string typeRapport, DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
            {
                ModelState.AddModelError("", "La date de début doit être antérieure à la date de fin.");
                return View();
            }

            if (dateDebut > DateTime.Now)
            {
                ModelState.AddModelError("", "La date de début ne peut pas être dans le futur.");
                return View();
            }

            try
            {
                int freelancerId = await GetCurrentFreelancerIdAsync();
                if (freelancerId == 0)
                {
                    ModelState.AddModelError("", "Aucun freelancer trouvé pour votre compte. Veuillez créer un freelancer avec le même email que votre compte utilisateur, ou créez un rapport manuellement en sélectionnant un freelancer.");
                    ViewBag.TypesRapport = new SelectList(new[]
                    {
                        new { Value = "Mensuel", Text = "Rapport Mensuel" },
                        new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                        new { Value = "Annuel", Text = "Rapport Annuel" },
                        new { Value = "Personnalise", Text = "Période Personnalisée" }
                    }, "Value", "Text");
                    return View();
                }
                
                var rapport = await _rapportService.GenererRapportAsync(freelancerId, typeRapport, dateDebut, dateFin);

                TempData["Success"] = "Rapport généré avec succès !";
                return RedirectToAction(nameof(Details), new { id = rapport.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de la génération du rapport : {ex.Message}");

                ViewBag.TypesRapport = new SelectList(new[]
                {
                    new { Value = "Mensuel", Text = "Rapport Mensuel" },
                    new { Value = "Trimestriel", Text = "Rapport Trimestriel" },
                    new { Value = "Annuel", Text = "Rapport Annuel" },
                    new { Value = "Personnalise", Text = "Période Personnalisée" }
                }, "Value", "Text");

                return View();
            }
        }

        // GET: Rapports/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var rapport = await _rapportService.GetRapportByIdAsync(id);

            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            try
            {
                // Générer le PDF à la volée
                var pdfBytes = await _rapportService.GenererPDFRapportAsync(id);

                var nomFichier = $"Rapport_{rapport.TypeRapport}_{rapport.DateGeneration:yyyyMMdd}.pdf";

                return File(pdfBytes, "application/pdf", nomFichier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors du téléchargement : {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Rapports/Preview/5
        public async Task<IActionResult> Preview(int id)
        {
            var rapport = await _rapportService.GetRapportByIdAsync(id);

            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            try
            {
                // Générer le PDF pour prévisualisation
                var pdfBytes = await _rapportService.GenererPDFRapportAsync(id);

                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la prévisualisation : {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Rapports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rapport = await _rapportService.GetRapportByIdAsync(id.Value);

            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            return View(rapport);
        }

        // POST: Rapports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rapport = await _rapportService.GetRapportByIdAsync(id);

            if (rapport == null) return NotFound();

            // Vérifier que le rapport appartient au freelancer connecté
            int currentFreelancerId = await GetCurrentFreelancerIdAsync();
            if (currentFreelancerId != 0 && rapport.FreelancerId != currentFreelancerId)
                return Forbid();

            try
            {
                var success = await _rapportService.DeleteRapportAsync(id);

                if (success)
                {
                    TempData["Success"] = "Rapport supprimé avec succès.";
                }
                else
                {
                    TempData["Error"] = "Impossible de supprimer le rapport.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la suppression : {ex.Message}";
                return View(rapport);
            }
        }
        // Récupère l'ID du freelancer basé sur l'email de l'utilisateur connecté
        private async Task<int> GetCurrentFreelancerIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return 0;

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(f => f.Email == user.Email);
            
            return freelancer?.Id ?? 0;
        }

        // Version synchrone pour compatibilité (utilise la version async en interne)
        private int GetCurrentFreelancerId()
        {
            // Pour les appels synchrones, on essaie de trouver par email depuis les claims
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(emailClaim))
                return 0;

            var freelancer = _context.Freelancers
                .AsNoTracking()
                .FirstOrDefault(f => f.Email == emailClaim);
            
            return freelancer?.Id ?? 0;
        }
    }
}