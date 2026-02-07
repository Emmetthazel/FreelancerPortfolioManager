using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class ProjetService : IProjetService
    {
        private readonly ApplicationDbContext _context;

        public ProjetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Projet>> GetAllProjetsAsync(int freelancerId)
        {
            return await _context.Projets
                .Include(p => p.Client)
                .Include(p => p.Contrat)
                .Where(p => p.FreelancerId == freelancerId)
                .OrderByDescending(p => p.DateDebut)
                .ToListAsync();
        }

        public async Task<Projet> GetProjetByIdAsync(int id)
        {
            return await _context.Projets
                .Include(p => p.Client)
                .Include(p => p.Contrat)
                .Include(p => p.Factures)
                .Include(p => p.Missions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Projet> CreateProjetAsync(Projet projet)
        {
            _context.Projets.Add(projet);
            await _context.SaveChangesAsync();
            return projet;
        }

        public async Task<bool> UpdateProjetAsync(Projet projet)
        {
            try
            {
                _context.Projets.Update(projet);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProjetAsync(int id)
        {
            var projet = await _context.Projets.FindAsync(id);
            if (projet == null)
                return false;

            _context.Projets.Remove(projet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Projet>> GetProjetsByClientAsync(int clientId)
        {
            return await _context.Projets
                .Include(p => p.Client)
                .Where(p => p.ClientId == clientId)
                .OrderByDescending(p => p.DateDebut)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projet>> GetProjetsByStatutAsync(int freelancerId, StatutProjet statut)
        {
            return await _context.Projets
                .Include(p => p.Client)
                .Where(p => p.FreelancerId == freelancerId && p.Statut == statut)
                .OrderByDescending(p => p.DateDebut)
                .ToListAsync();
        }

        public async Task<bool> CloturerProjetAsync(int id)
        {
            var projet = await _context.Projets.FindAsync(id);
            if (projet == null)
                return false;

            projet.Statut = StatutProjet.TERMINE;
            projet.TauxAvancement = 100;
            projet.DateFin = System.DateTime.Now;

            _context.Projets.Update(projet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}