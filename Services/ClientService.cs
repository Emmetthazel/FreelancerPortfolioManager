using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync(int freelancerId)
        {
            return await _context.Clients
                .Where(c => c.FreelancerId == freelancerId)
                .OrderBy(c => c.NomEntreprise)
                .ToListAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Projets)
                .Include(c => c.Evaluations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            try
            {
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateNoteGlobaleAsync(int clientId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.ClientId == clientId)
                .ToListAsync();

            if (evaluations.Any())
            {
                var client = await _context.Clients.FindAsync(clientId);
                if (client != null)
                {
                    client.NoteGlobale = (float)evaluations.Average(e => e.Note);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}