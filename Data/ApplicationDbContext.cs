using FreelancerPortfolioManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolioManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets pour chaque modèle
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<Contrat> Contrats { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Rapport> Rapports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations et contraintes

            // Freelancer - Clients (One-to-Many)
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Freelancer)
                .WithMany(f => f.Clients)
                .HasForeignKey(c => c.FreelancerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Freelancer - Projets (One-to-Many)
            modelBuilder.Entity<Projet>()
                .HasOne(p => p.Freelancer)
                .WithMany(f => f.Projets)
                .HasForeignKey(p => p.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Client - Projets (One-to-Many)
            modelBuilder.Entity<Projet>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Projets)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Projet - Contrat (One-to-One)
            modelBuilder.Entity<Contrat>()
                .HasOne(c => c.Projet)
                .WithOne(p => p.Contrat)
                .HasForeignKey<Contrat>(c => c.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Projet - Factures (One-to-Many)
            modelBuilder.Entity<Facture>()
                .HasOne(f => f.Projet)
                .WithMany(p => p.Factures)
                .HasForeignKey(f => f.ProjetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Freelancer - Factures (One-to-Many)
            modelBuilder.Entity<Facture>()
                .HasOne(f => f.Freelancer)
                .WithMany(fr => fr.Factures)
                .HasForeignKey(f => f.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Facture - Paiements (One-to-Many)
            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Facture)
                .WithMany(f => f.Paiements)
                .HasForeignKey(p => p.FactureId)
                .OnDelete(DeleteBehavior.Cascade);

            // Client - Evaluations (One-to-Many)
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Client)
                .WithMany(c => c.Evaluations)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Evaluation - Freelancer (One-to-Many) — avoid multiple cascade paths by not cascading here
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Freelancer)
                .WithMany(f => f.Evaluations)
                .HasForeignKey(e => e.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Projet - Missions (One-to-Many)
            modelBuilder.Entity<Mission>()
                .HasOne(m => m.Projet)
                .WithMany(p => p.Missions)
                .HasForeignKey(m => m.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour améliorer les performances
            modelBuilder.Entity<Freelancer>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email);

            modelBuilder.Entity<Facture>()
                .HasIndex(f => f.Numero)
                .IsUnique();

            modelBuilder.Entity<Contrat>()
                .HasIndex(c => c.Reference)
                .IsUnique();

            // Configuration des types décimaux
            modelBuilder.Entity<Freelancer>()
                .Property(f => f.TarifHoraire)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Projet>()
                .Property(p => p.Budget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Contrat>()
                .Property(c => c.Montant)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Facture>()
                .Property(f => f.TauxTVA)
                .HasPrecision(10, 2);
        }
    }
}