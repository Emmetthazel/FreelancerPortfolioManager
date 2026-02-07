using FreelancerPortfolioManager.Models;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenererRapportPdfAsync(Rapport rapport, RapportData donnees);
        Task<byte[]> GenererFacturePdfAsync(Facture facture);
        Task<string> SauvegarderPdfAsync(byte[] pdfBytes, string nomFichier);
    }

    // Classe pour les données du rapport
    public class RapportData
    {
        public string NomFreelancer { get; set; }
        public string EmailFreelancer { get; set; }
        public int NombreProjets { get; set; }
        public int NombreClients { get; set; }
        public decimal RevenuTotal { get; set; }
        public decimal RevenuMoyen { get; set; }
        public int NombreFactures { get; set; }
        public int FacturesPayees { get; set; }
        public decimal TauxReussite { get; set; }
        public List<ProjetStatistique> Projets { get; set; }
        public List<ClientStatistique> TopClients { get; set; }
    }

    public class ProjetStatistique
    {
        public string Titre { get; set; }
        public string Client { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public decimal Budget { get; set; }
        public string Statut { get; set; }
    }

    public class ClientStatistique
    {
        public string NomEntreprise { get; set; }
        public int NombreProjets { get; set; }
        public decimal RevenuGenere { get; set; }
        public float Note { get; set; }
    }
}