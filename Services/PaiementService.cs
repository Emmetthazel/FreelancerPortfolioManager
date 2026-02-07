using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class PaiementService : IPaiementService
    {
        private readonly ApplicationDbContext _context;

        public PaiementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Paiement>> GetPaiementsByFactureAsync(int factureId)
        {
            return await _context.Paiements
                .Where(p => p.FactureId == factureId)
                .OrderByDescending(p => p.DatePaiement)
                .ToListAsync();
        }

        public async Task<Paiement> GetPaiementByIdAsync(int id)
        {
            return await _context.Paiements
                .Include(p => p.Facture)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paiement> CreatePaiementAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);

            // Mettre à jour le statut de la facture
            var facture = await _context.Factures.FindAsync(paiement.FactureId);
            if (facture != null)
            {
                var totalPaye = await _context.Paiements
                    .Where(p => p.FactureId == paiement.FactureId)
                    .SumAsync(p => p.Montant) + paiement.Montant;

                if (totalPaye >= facture.MontantTTC)
                {
                    facture.Statut = StatutFacture.PAYEE;
                }
            }

            await _context.SaveChangesAsync();
            return paiement;
        }

        public async Task<bool> DeletePaiementAsync(int id)
        {
            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement == null) return false;

            _context.Paiements.Remove(paiement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
