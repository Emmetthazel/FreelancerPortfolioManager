using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IEvaluationService
    {
        Task<IEnumerable<Evaluation>> GetEvaluationsByClientAsync(int clientId);
        Task<Evaluation> GetEvaluationByIdAsync(int id);
        Task<Evaluation> CreateEvaluationAsync(Evaluation evaluation);
        Task<bool> UpdateEvaluationAsync(Evaluation evaluation);
        Task<bool> DeleteEvaluationAsync(int id);
    }
}
