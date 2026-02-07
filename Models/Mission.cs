using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "La date de début est requise")]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [Required(ErrorMessage = "Le nombre d'heures est requis")]
        [Range(1, 1000, ErrorMessage = "Le nombre d'heures doit être entre 1 et 1000")]
        [Display(Name = "Nombre d'heures")]
        public int NombreHeures { get; set; }

        [Required(ErrorMessage = "Le tarif horaire est requis")]
        [Range(0, 10000, ErrorMessage = "Le tarif horaire doit être positif")]
        [Display(Name = "Tarif horaire")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TarifHoraire { get; set; }

        // Propriété calculée
        [Display(Name = "Montant total")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTotal => NombreHeures * TarifHoraire;

        // Clé étrangère
        [Required]
        public int ProjetId { get; set; }

        // Relation — make nullable so model binder doesn't require the full object
        [ForeignKey("ProjetId")]
        public Projet? Projet { get; set; }
    }
}