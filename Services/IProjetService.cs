using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IProjetService
    {
        Task<IEnumerable<Projet>> GetAllProjetsAsync(int freelancerId);
        Task<Projet> GetProjetByIdAsync(int id);
        Task<Projet> CreateProjetAsync(Projet projet);
        Task<bool> UpdateProjetAsync(Projet projet);
        Task<bool> DeleteProjetAsync(int id);
        Task<IEnumerable<Projet>> GetProjetsByClientAsync(int clientId);
        Task<IEnumerable<Projet>> GetProjetsByStatutAsync(int freelancerId, StatutProjet statut);
        Task<bool> CloturerProjetAsync(int id);
    }
}