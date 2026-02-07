using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'entreprise est requis")]
        [StringLength(200)]
        [Display(Name = "Nom de l'entreprise")]
        public string NomEntreprise { get; set; }

        [Required(ErrorMessage = "Le nom du contact est requis")]
        [StringLength(100)]
        [Display(Name = "Nom du contact")]
        public string NomContact { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(150)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [StringLength(20)]
        public string Telephone { get; set; }

        [StringLength(200)]
        public string Adresse { get; set; }

        [Display(Name = "Date d'ajout")]
        public DateTime DateAjout { get; set; } = DateTime.Now;

        [Range(0, 5, ErrorMessage = "La note doit être entre 0 et 5")]
        [Display(Name = "Note globale")]
        public float NoteGlobale { get; set; } = 0;

        // Clé étrangère
        [Required]
        public int FreelancerId { get; set; }

        // Navigation (nullable so binder doesn't require full object)
        [ForeignKey("FreelancerId")]
        public Freelancer? Freelancer { get; set; }

        // Initialize collections to avoid "field is required" validation
        public ICollection<Projet> Projets { get; set; } = new List<Projet>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}