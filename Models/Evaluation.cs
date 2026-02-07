using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancerPortfolioManager.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La note est requise")]
        [Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5")]
        public int Note { get; set; }

        [Required(ErrorMessage = "Le commentaire est requis")]
        [StringLength(1000)]
        [MinLength(10, ErrorMessage = "Le commentaire doit contenir au moins 10 caractères")]
        public string Commentaire { get; set; }

        [Display(Name = "Date d'évaluation")]
        public DateTime DateEvaluation { get; set; } = DateTime.Now;

        // Clés étrangères
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int FreelancerId { get; set; }

        // Navigation properties — make nullable so binder does not require full object
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer? Freelancer { get; set; }
    }
}