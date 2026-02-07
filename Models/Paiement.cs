using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le montant est requis")]
        [Range(0, 10000000, ErrorMessage = "Le montant doit être positif")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        [Required(ErrorMessage = "La date de paiement est requise")]
        [Display(Name = "Date de paiement")]
        [DataType(DataType.Date)]
        public DateTime DatePaiement { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Le moyen de paiement est requis")]
        [StringLength(50)]
        [Display(Name = "Moyen de paiement")]
        public string MoyenPaiement { get; set; }

        [StringLength(100)]
        [Display(Name = "Référence de transaction")]
        public string Reference { get; set; }

        // Clé étrangère
        [Required]
        public int FactureId { get; set; }

        // Relation — make nullable so binder doesn't require the full object on POST
        [ForeignKey("FactureId")]
        public Facture? Facture { get; set; }
    }
}