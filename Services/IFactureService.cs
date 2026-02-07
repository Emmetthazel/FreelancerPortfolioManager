using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IFactureService
    {
        Task<IEnumerable<Facture>> GetAllFacturesAsync(int freelancerId);
        Task<Facture> GetFactureByIdAsync(int id);
        Task<Facture> CreateFactureAsync(Facture facture);
        Task<bool> UpdateFactureAsync(Facture facture);
        Task<bool> DeleteFactureAsync(int id);
        Task<string> GenererNumeroFactureAsync();
        Task<byte[]> GenererPDFFactureAsync(int factureId);
        Task<bool> EnvoyerFactureAsync(int factureId);
    }
}
