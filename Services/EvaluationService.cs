using FreelancerPortfolioManager.Data;
using FreelancerPortfolioManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly ApplicationDbContext _context;

        public EvaluationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Evaluation>> GetEvaluationsByClientAsync(int clientId)
        {
            return await _context.Evaluations
                .Include(e => e.Client)
                .Where(e => e.ClientId == clientId)
                .OrderByDescending(e => e.DateEvaluation)
                .ToListAsync();
        }

        public async Task<Evaluation> GetEvaluationByIdAsync(int id)
        {
            return await _context.Evaluations
                .Include(e => e.Client)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Evaluation> CreateEvaluationAsync(Evaluation evaluation)
        {
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            // Mettre à jour la note globale du client
            await UpdateNoteGlobaleClientAsync(evaluation.ClientId);

            return evaluation;
        }

        public async Task<bool> UpdateEvaluationAsync(Evaluation evaluation)
        {
            try
            {
                _context.Evaluations.Update(evaluation);
                await _context.SaveChangesAsync();
                await UpdateNoteGlobaleClientAsync(evaluation.ClientId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEvaluationAsync(int id)
        {
            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation == null) return false;

            int clientId = evaluation.ClientId;
            _context.Evaluations.Remove(evaluation);
            await _context.SaveChangesAsync();

            await UpdateNoteGlobaleClientAsync(clientId);
            return true;
        }

        private async Task UpdateNoteGlobaleClientAsync(int clientId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.ClientId == clientId)
                .ToListAsync();

            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.NoteGlobale = evaluations.Any() ? (float)evaluations.Average(e => e.Note) : 0;
                await _context.SaveChangesAsync();
            }
        }
    }
}
