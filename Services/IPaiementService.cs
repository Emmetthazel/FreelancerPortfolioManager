using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IPaiementService
    {
        Task<IEnumerable<Paiement>> GetPaiementsByFactureAsync(int factureId);
        Task<Paiement> GetPaiementByIdAsync(int id);
        Task<Paiement> CreatePaiementAsync(Paiement paiement);
        Task<bool> DeletePaiementAsync(int id);
    }
}
