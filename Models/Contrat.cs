using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public enum StatutContrat
    {
        [Display(Name = "Brouillon")]
        BROUILLON,
        [Display(Name = "En attente de signature")]
        EN_ATTENTE_SIGNATURE,
        [Display(Name = "Signé")]
        SIGNE,
        [Display(Name = "Résilié")]
        RESILIE
    }

    public class Contrat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La référence est requise")]
        [StringLength(50)]
        public string Reference { get; set; }

        [Display(Name = "Date de création")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        [Display(Name = "Date de signature")]
        [DataType(DataType.Date)]
        public DateTime? DateSignature { get; set; }

        [Required(ErrorMessage = "Le montant est requis")]
        [Range(0, 10000000, ErrorMessage = "Le montant doit être positif")]
        public decimal Montant { get; set; }

        [Required(ErrorMessage = "Les conditions sont requises")]
        [StringLength(5000)]
        public string Conditions { get; set; }

        [Required]
        public StatutContrat Statut { get; set; } = StatutContrat.BROUILLON;

        // Clé étrangère
        [Required]
        public int ProjetId { get; set; }

        // Relation — nullable so binder doesn't require full object on POST
        [ForeignKey("ProjetId")]
        public Projet? Projet { get; set; }
    }
}