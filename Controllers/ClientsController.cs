using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clients.Include(c => c.Freelancer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            var freelancers = _context.Freelancers.AsNoTracking().ToList();
            if (!freelancers.Any())
            {
                // empty SelectList to avoid null reference in the view
                ViewData["FreelancerId"] = new SelectList(Enumerable.Empty<Freelancer>(), "Id", "Email");
                ModelState.AddModelError(string.Empty, "Aucun freelancer trouvé. Créez d'abord un freelancer avant d'ajouter un client.");
            }
            else
            {
                ViewData["FreelancerId"] = new SelectList(freelancers, "Id", "Email");
            }

            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomEntreprise,NomContact,Email,Telephone,Adresse,FreelancerId")] Client client)
        {
            // Defensive check: ensure selected freelancer exists (prevents FK errors)
            if (client.FreelancerId <= 0 || !_context.Freelancers.Any(f => f.Id == client.FreelancerId))
            {
                ModelState.AddModelError("FreelancerId", "Veuillez sélectionner un freelancer valide.");
            }

            if (!ModelState.IsValid)
            {
                // Collect ModelState errors for quick debugging and display in the view
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["ModelErrors"] = string.Join(" | ", errors);

                // Re-populate the select list and return view
                ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", client?.FreelancerId);
                return View(client);
            }

            client.DateAjout = DateTime.Now; // Génération automatique
            _context.Add(client);
            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Client créé avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                ViewData["FreelancerId"] = new SelectList(_context.Freelancers.AsNoTracking(), "Id", "Email", client?.FreelancerId);
                return View(client);
            }
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "Id", "Email", client.FreelancerId);
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomEntreprise,NomContact,Email,Telephone,Adresse,DateAjout,NoteGlobale,FreelancerId")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            // Validate selected freelancer exists on edit as well
            if (client.FreelancerId <= 0 || !_context.Freelancers.Any(f => f.Id == client.FreelancerId))
            {
                ModelState.AddModelError("FreelancerId", "Freelancer sélectionné introuvable. Veuillez en sélectionner un valide.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "Id", "Email", client.FreelancerId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Freelancer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}