using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IRapportService
    {
        Task<IEnumerable<Rapport>> GetAllRapportsAsync(int freelancerId);
        Task<Rapport> GetRapportByIdAsync(int id);
        Task<Rapport> GenererRapportAsync(int freelancerId, string typeRapport, System.DateTime dateDebut, System.DateTime dateFin);
        Task<byte[]> GenererPDFRapportAsync(int rapportId);
        Task<bool> DeleteRapportAsync(int id);
    }
}
