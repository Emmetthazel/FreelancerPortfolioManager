
using FreelancerPortfolioManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync(int freelancerId);
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> CreateClientAsync(Client client);
        Task<bool> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);
        Task UpdateNoteGlobaleAsync(int clientId);
    }
}