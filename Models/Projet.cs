using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public enum StatutProjet
    {
        [Display(Name = "En attente")]
        EN_ATTENTE,
        [Display(Name = "En cours")]
        EN_COURS,
        [Display(Name = "Terminé")]
        TERMINE,
        [Display(Name = "Annulé")]
        ANNULE
    }

    public class Projet
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200)]
        public string Titre { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "La date de début est requise")]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [Range(0, 1000000, ErrorMessage = "Le budget doit être entre 0 et 1000000")]
        public decimal Budget { get; set; }

        [Required]
        public StatutProjet Statut { get; set; } = StatutProjet.EN_ATTENTE;

        [Range(0, 100, ErrorMessage = "Le taux d'avancement doit être entre 0 et 100")]
        [Display(Name = "Taux d'avancement (%)")]
        public int TauxAvancement { get; set; } = 0;

        // Clés étrangères
        [Required]
        public int FreelancerId { get; set; }

        [Required]
        public int ClientId { get; set; }

        // Relations: make nullable so binder doesn't require full object
        [ForeignKey("FreelancerId")]
        public Freelancer? Freelancer { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public Contrat? Contrat { get; set; }
        public ICollection<Facture> Factures { get; set; } = new List<Facture>();
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();
    }
}