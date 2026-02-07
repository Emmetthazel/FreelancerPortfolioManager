using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class FactureService : IFactureService
    {
        private readonly ApplicationDbContext _context;

        public FactureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesAsync(int freelancerId)
        {
            return await _context.Factures
                .Include(f => f.Projet)
                .ThenInclude(p => p.Client)
                .Where(f => f.FreelancerId == freelancerId)
                .OrderByDescending(f => f.DateEmission)
                .ToListAsync();
        }

        public async Task<Facture> GetFactureByIdAsync(int id)
        {
            return await _context.Factures
                .Include(f => f.Projet)
                .ThenInclude(p => p.Client)
                .Include(f => f.Paiements)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Facture> CreateFactureAsync(Facture facture)
        {
            facture.Numero = await GenererNumeroFactureAsync();
            facture.MontantTVA = facture.MontantHT * (facture.TauxTVA / 100);
            facture.MontantTTC = facture.MontantHT + facture.MontantTVA;

            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();
            return facture;
        }

        public async Task<bool> UpdateFactureAsync(Facture facture)
        {
            try
            {
                facture.MontantTVA = facture.MontantHT * (facture.TauxTVA / 100);
                facture.MontantTTC = facture.MontantHT + facture.MontantTVA;

                _context.Factures.Update(facture);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFactureAsync(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null) return false;

            _context.Factures.Remove(facture);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenererNumeroFactureAsync()
        {
            var year = DateTime.Now.Year;
            var lastFacture = await _context.Factures
                .Where(f => f.DateEmission.Year == year)
                .OrderByDescending(f => f.Numero)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastFacture != null && !string.IsNullOrEmpty(lastFacture.Numero))
            {
                var parts = lastFacture.Numero.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"FACT-{year}-{nextNumber:D4}";
        }

        public async Task<byte[]> GenererPDFFactureAsync(int factureId)
        {
            // TODO: Implémenter la génération PDF avec iTextSharp ou similaire
            await Task.CompletedTask;
            return new byte[0];
        }

        public async Task<bool> EnvoyerFactureAsync(int factureId)
        {
            // TODO: Implémenter l'envoi d'email
            var facture = await GetFactureByIdAsync(factureId);
            if (facture == null) return false;

            facture.Statut = StatutFacture.ENVOYEE;
            await UpdateFactureAsync(facture);
            return true;
        }
    }
}
