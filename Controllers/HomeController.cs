using System.Diagnostics;
using FreelancerPortfolioManager.Models;
using FreelancerPortfolioManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolioManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                FreelancersCount = await _context.Freelancers.CountAsync(),
                ClientsCount = await _context.Clients.CountAsync(),
                ProjetsCount = await _context.Projets.CountAsync(),
                FacturesCount = await _context.Factures.CountAsync(),
                ContratsCount = await _context.Contrats.CountAsync(),
                PaiementsCount = await _context.Paiements.CountAsync(),
                MissionsCount = await _context.Missions.CountAsync(),
                EvaluationsCount = await _context.Evaluations.CountAsync(),
                RapportsCount = await _context.Rapports.CountAsync(),
                ProjetsEnCours = await _context.Projets.CountAsync(p => p.Statut == StatutProjet.EN_COURS),
                FacturesPayees = await _context.Factures.CountAsync(f => f.Statut == StatutFacture.PAYEE),
                RevenuTotal = await _context.Factures
                    .Where(f => f.Statut == StatutFacture.PAYEE)
                    .SumAsync(f => f.MontantTTC)
            };

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
