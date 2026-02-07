using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public enum StatutFacture
    {
        [Display(Name = "Brouillon")]
        BROUILLON,
        [Display(Name = "Envoyée")]
        ENVOYEE,
        [Display(Name = "Payée")]
        PAYEE,
        [Display(Name = "En retard")]
        EN_RETARD,
        [Display(Name = "Annulée")]
        ANNULEE
    }

    public class Facture
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le numéro est requis")]
        [StringLength(50)]
        public string Numero { get; set; }

        [Display(Name = "Date d'émission")]
        [DataType(DataType.Date)]
        public DateTime DateEmission { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La date d'échéance est requise")]
        [Display(Name = "Date d'échéance")]
        [DataType(DataType.Date)]
        public DateTime DateEcheance { get; set; }

        [Required(ErrorMessage = "Le montant HT est requis")]
        [Range(0, 10000000, ErrorMessage = "Le montant doit être positif")]
        [Display(Name = "Montant HT")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantHT { get; set; }

        [Display(Name = "Montant TVA")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTVA { get; set; }

        [Display(Name = "Montant TTC")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantTTC { get; set; }

        [Required]
        public StatutFacture Statut { get; set; } = StatutFacture.BROUILLON;

        [Range(0, 100, ErrorMessage = "Le taux de TVA doit être entre 0 et 100")]
        [Display(Name = "Taux TVA (%)")]
        public decimal TauxTVA { get; set; } = 20;

        // Clés étrangères
        [Required]
        public int ProjetId { get; set; }

        [Required]
        public int FreelancerId { get; set; }

        // Relations (nullable / initialisées pour éviter les erreurs de binding)
        [ForeignKey("ProjetId")]
        public Projet? Projet { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer? Freelancer { get; set; }

        public ICollection<Paiement> Paiements { get; set; } = new List<Paiement>();
    }
}