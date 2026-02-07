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
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FreelancersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Freelancers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Freelancers.ToListAsync());
        }

        // GET: Freelancers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freelancer == null)
            {
                return NotFound();
            }

            return View(freelancer);
        }

        // GET: Freelancers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Freelancers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Prenom,Email,Telephone,Adresse,Competences,TarifHoraire,Photo,PasswordHash,DerniereConnexion")] Freelancer freelancer)
        {
            if (ModelState.IsValid)
            {
                // On définit la date d'inscription automatiquement
                freelancer.DateInscription = DateTime.Now;

                _context.Add(freelancer);
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Freelancer créé avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    // Surface the DB error to the user for debugging (you can log instead)
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }

            // Si le modèle n'est pas valide ou s'il y a une erreur DB, on retourne la vue avec les messages d'erreur
            return View(freelancer);
        }

        // ... reste du contrôleur inchangé ...
        // GET: Freelancers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancer = await _context.Freelancers.FindAsync(id);
            if (freelancer == null)
            {
                return NotFound();
            }
            return View(freelancer);
        }

        // POST: Freelancers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Email,Telephone,Adresse,Competences,TarifHoraire,DateInscription,Photo")] Freelancer freelancer)
        {
            if (id != freelancer.Id)
            {
                return NotFound();
            }

            // Charger l'entité existante pour préserver PasswordHash et DerniereConnexion
            var existingFreelancer = await _context.Freelancers.FindAsync(id);
            if (existingFreelancer == null)
            {
                return NotFound();
            }

            // Supprimer les erreurs de validation pour les champs cachés
            ModelState.Remove("PasswordHash");
            ModelState.Remove("DerniereConnexion");

            if (ModelState.IsValid)
            {
                try
                {
                    // Mettre à jour uniquement les champs modifiables
                    existingFreelancer.Nom = freelancer.Nom;
                    existingFreelancer.Prenom = freelancer.Prenom;
                    existingFreelancer.Email = freelancer.Email;
                    existingFreelancer.Telephone = freelancer.Telephone;
                    existingFreelancer.Adresse = freelancer.Adresse;
                    existingFreelancer.Competences = freelancer.Competences;
                    existingFreelancer.TarifHoraire = freelancer.TarifHoraire;
                    existingFreelancer.DateInscription = freelancer.DateInscription;
                    existingFreelancer.Photo = freelancer.Photo;
                    // PasswordHash et DerniereConnexion sont préservés

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Freelancer modifié avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerExists(freelancer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    ModelState.AddModelError(string.Empty, "Erreur base de données : " + dbEx.GetBaseException().Message);
                }
            }
            
            // Si erreur de validation, retourner la vue avec le modèle existant
            return View(existingFreelancer);
        }

        // GET: Freelancers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (freelancer == null)
            {
                return NotFound();
            }

            return View(freelancer);
        }

        // POST: Freelancers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var freelancer = await _context.Freelancers.FindAsync(id);
            if (freelancer != null)
            {
                _context.Freelancers.Remove(freelancer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerExists(int id)
        {
            return _context.Freelancers.Any(e => e.Id == id);
        }
    }
}