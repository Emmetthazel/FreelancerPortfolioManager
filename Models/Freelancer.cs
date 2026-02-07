using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelancerPortfolioManager.Models
{
    public class Freelancer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(150)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [StringLength(20)]
        public string Telephone { get; set; }

        [StringLength(200)]
        public string Adresse { get; set; }

        public string Competences { get; set; }

        [Range(0, 10000, ErrorMessage = "Le tarif doit être entre 0 et 10000")]
        public decimal TarifHoraire { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.Now;

        public string Photo { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        public DateTime? DerniereConnexion { get; set; }

        // Relations
        public ICollection<Projet> Projets { get; set; } = new List<Projet>();
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<Facture> Factures { get; set; } = new List<Facture>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}