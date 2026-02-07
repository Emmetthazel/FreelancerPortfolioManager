using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class RapportService : IRapportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;

        public RapportService(ApplicationDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<IEnumerable<Rapport>> GetAllRapportsAsync(int freelancerId)
        {
            return await _context.Rapports
                .Where(r => r.FreelancerId == freelancerId)
                .OrderByDescending(r => r.DateGeneration)
                .ToListAsync();
        }

        public async Task<Rapport> GetRapportByIdAsync(int id)
        {
            return await _context.Rapports
                .Include(r => r.Freelancer)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rapport> GenererRapportAsync(int freelancerId, string typeRapport, DateTime dateDebut, DateTime dateFin)
        {
            // Récupérer les données du freelancer
            var freelancer = await _context.Freelancers.FindAsync(freelancerId);
            if (freelancer == null)
                throw new Exception("Freelancer introuvable");

            // Créer le rapport
            var rapport = new Rapport
            {
                FreelancerId = freelancerId,
                TypeRapport = typeRapport,
                DateDebut = dateDebut,
                DateFin = dateFin,
                Titre = $"Rapport {typeRapport} - {dateDebut:dd/MM/yyyy} au {dateFin:dd/MM/yyyy}",
                DateGeneration = DateTime.Now
            };

            // Collecter les données pour le rapport
            var donnees = await CollecterDonneesRapportAsync(freelancerId, dateDebut, dateFin);
            donnees.NomFreelancer = $"{freelancer.Prenom} {freelancer.Nom}";
            donnees.EmailFreelancer = freelancer.Email;

            // Générer le contenu textuel
            rapport.Contenu = GenererContenuTexte(donnees);

            // Enregistrer le rapport en base
            _context.Rapports.Add(rapport);
            await _context.SaveChangesAsync();

            // Générer le PDF
            var pdfBytes = await _pdfService.GenererRapportPdfAsync(rapport, donnees);
            var nomFichier = $"rapport_{rapport.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            rapport.CheminPDF = await _pdfService.SauvegarderPdfAsync(pdfBytes, nomFichier);

            // Mettre à jour le rapport avec le chemin du PDF
            _context.Rapports.Update(rapport);
            await _context.SaveChangesAsync();

            return rapport;
        }

        private async Task<RapportData> CollecterDonneesRapportAsync(int freelancerId, DateTime dateDebut, DateTime dateFin)
        {
            // Récupérer les projets
            var projets = await _context.Projets
                .Include(p => p.Client)
                .Where(p => p.FreelancerId == freelancerId
                    && p.DateDebut >= dateDebut
                    && p.DateDebut <= dateFin)
                .ToListAsync();

            // Récupérer les clients uniques
            var clientIds = projets.Select(p => p.ClientId).Distinct().ToList();
            var clients = await _context.Clients
                .Where(c => clientIds.Contains(c.Id))
                .ToListAsync();

            // Récupérer les factures
            var factures = await _context.Factures
                .Where(f => f.FreelancerId == freelancerId
                    && f.DateEmission >= dateDebut
                    && f.DateEmission <= dateFin)
                .ToListAsync();

            // Calculer les statistiques
            var revenuTotal = factures.Where(f => f.Statut == StatutFacture.PAYEE).Sum(f => f.MontantTTC);
            var revenuMoyen = projets.Any() ? revenuTotal / projets.Count : 0m;
            var facturesPayees = factures.Count(f => f.Statut == StatutFacture.PAYEE);
            var projetsTermines = projets.Count(p => p.Statut == StatutProjet.TERMINE);
            var tauxReussite = projets.Any() ? (decimal)((float)projetsTermines / projets.Count * 100) : 0m;

            // Top 5 clients par revenu
            var topClients = clients
                .Select(c => new ClientStatistique
                {
                    NomEntreprise = c.NomEntreprise,
                    NombreProjets = projets.Count(p => p.ClientId == c.Id),
                    RevenuGenere = factures
                        .Where(f => projets.Any(p => p.Id == f.ProjetId && p.ClientId == c.Id)
                            && f.Statut == StatutFacture.PAYEE)
                        .Sum(f => f.MontantTTC),
                    Note = c.NoteGlobale
                })
                .OrderByDescending(c => c.RevenuGenere)
                .Take(5)
                .ToList();

            // Liste des projets pour le tableau
            var projetsStats = projets.Select(p => new ProjetStatistique
            {
                Titre = p.Titre,
                Client = p.Client?.NomEntreprise ?? "N/A",
                DateDebut = p.DateDebut,
                DateFin = p.DateFin,
                Budget = p.Budget,
                Statut = p.Statut.ToString()
            }).ToList();

            return new RapportData
            {
                NombreProjets = projets.Count,
                NombreClients = clients.Count,
                RevenuTotal = revenuTotal,
                RevenuMoyen = revenuMoyen,
                NombreFactures = factures.Count,
                FacturesPayees = facturesPayees,
                TauxReussite = tauxReussite,
                Projets = projetsStats,
                TopClients = topClients
            };
        }

        private string GenererContenuTexte(RapportData donnees)
        {
            return $@"
=== VUE D'ENSEMBLE ===
- Nombre de projets : {donnees.NombreProjets}
- Nombre de clients : {donnees.NombreClients}
- Revenu total : {donnees.RevenuTotal:C}
- Revenu moyen : {donnees.RevenuMoyen:C}
- Taux de réussite : {donnees.TauxReussite:F1}%

=== STATISTIQUES FINANCIÈRES ===
- Nombre de factures : {donnees.NombreFactures}
- Factures payées : {donnees.FacturesPayees}

=== TOP CLIENTS ===
{string.Join("\n", donnees.TopClients.Select(c => $"- {c.NomEntreprise} : {c.NombreProjets} projets, {c.RevenuGenere:C}, Note: {c.Note:F1}/5"))}
";
        }

        public async Task<byte[]> GenererPDFRapportAsync(int rapportId)
        {
            var rapport = await GetRapportByIdAsync(rapportId);
            if (rapport == null)
                throw new Exception("Rapport introuvable");

            // Régénérer les données
            var donnees = await CollecterDonneesRapportAsync(rapport.FreelancerId, rapport.DateDebut, rapport.DateFin);
            donnees.NomFreelancer = $"{rapport.Freelancer.Prenom} {rapport.Freelancer.Nom}";
            donnees.EmailFreelancer = rapport.Freelancer.Email;

            return await _pdfService.GenererRapportPdfAsync(rapport, donnees);
        }

        public async Task<bool> DeleteRapportAsync(int id)
        {
            var rapport = await _context.Rapports.FindAsync(id);
            if (rapport == null) return false;

            // Supprimer le fichier PDF s'il existe
            if (!string.IsNullOrEmpty(rapport.CheminPDF) && File.Exists(rapport.CheminPDF))
            {
                File.Delete(rapport.CheminPDF);
            }

            _context.Rapports.Remove(rapport);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}