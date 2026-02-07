using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public class Rapport
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200)]
        public string Titre { get; set; }

        [Display(Name = "Date de génération")]
        public DateTime DateGeneration { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Le type de rapport est requis")]
        [StringLength(50)]
        [Display(Name = "Type de rapport")]
        public string TypeRapport { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [Display(Name = "Contenu")]
        public string? Contenu { get; set; }

        [Display(Name = "Chemin du fichier PDF")]
        public string? CheminPDF { get; set; }

        // Clé étrangère
        [Required]
        public int FreelancerId { get; set; }

        // Relation — make nullable so binder doesn't require full object on POST
        [ForeignKey("FreelancerId")]
        public Freelancer? Freelancer { get; set; }
    }
}