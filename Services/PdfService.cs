using FreelancerPortfolioManager.Models;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancerPortfolioManager.Services
{
    public class PdfService : IPdfService
    {
        private readonly string _rapportsPath;

        public PdfService(IConfiguration configuration)
        {
            _rapportsPath = configuration["AppSettings:RapportsPath"] ?? "wwwroot/rapports";

            // Créer le dossier s'il n'existe pas
            if (!Directory.Exists(_rapportsPath))
            {
                Directory.CreateDirectory(_rapportsPath);
            }

            // Configuration QuestPDF (licence communautaire gratuite)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenererRapportPdfAsync(Rapport rapport, RapportData donnees)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        // En-tête
                        page.Header().Element(c => ComposerEntete(c, rapport, donnees));

                        // Contenu
                        page.Content().Element(c => ComposerContenu(c, rapport, donnees));

                        // Pied de page
                        page.Footer().Element(c => ComposerPiedDePage(c));
                    });
                });

                return document.GeneratePdf();
            });
        }

        private void ComposerEntete(IContainer container, Rapport rapport, RapportData donnees)
        {
            container.Column(column =>
            {
                column.Item().Background(Colors.Blue.Medium).Padding(20).Column(headerColumn =>
                {
                    headerColumn.Item().Text("RAPPORT D'ACTIVITÉ")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.White);

                    headerColumn.Item().PaddingTop(10).Text(rapport.Titre)
                        .FontSize(14)
                        .FontColor(Colors.White);
                });

                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Freelancer : ").Bold();
                            text.Span(donnees.NomFreelancer);
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Email : ").Bold();
                            text.Span(donnees.EmailFreelancer);
                        });
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text(text =>
                        {
                            text.Span("Période : ").Bold();
                            text.Span($"{rapport.DateDebut:dd/MM/yyyy} - {rapport.DateFin:dd/MM/yyyy}");
                        });

                        col.Item().AlignRight().Text(text =>
                        {
                            text.Span("Généré le : ").Bold();
                            text.Span($"{rapport.DateGeneration:dd/MM/yyyy HH:mm}");
                        });
                    });
                });

                column.Item().PaddingTop(10).Height(2).Background(Colors.Blue.Medium);
            });
        }

        private void ComposerContenu(IContainer container, Rapport rapport, RapportData donnees)
        {
            container.Column(column =>
            {
                // Section Vue d'ensemble
                column.Item().PaddingTop(20).Element(c => SectionVueEnsemble(c, donnees));

                // Section Projets
                column.Item().PaddingTop(20).Element(c => SectionProjets(c, donnees));

                // Section Top Clients
                column.Item().PaddingTop(20).Element(c => SectionTopClients(c, donnees));

                // Section Statistiques financières
                column.Item().PaddingTop(20).Element(c => SectionStatistiquesFinancieres(c, donnees));
            });
        }

        private void SectionVueEnsemble(IContainer container, RapportData donnees)
        {
            container.Column(column =>
            {
                column.Item().Text("VUE D'ENSEMBLE")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Medium);

                column.Item().PaddingTop(10).Height(1).Background(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().BorderColor(Colors.Blue.Lighten3).Border(1).Padding(10)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(donnees.NombreProjets.ToString())
                                .FontSize(32)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            col.Item().AlignCenter().Text("Projets")
                                .FontSize(12)
                                .FontColor(Colors.Grey.Medium);
                        });

                    row.Spacing(10);

                    row.RelativeItem().BorderColor(Colors.Green.Lighten3).Border(1).Padding(10)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(donnees.NombreClients.ToString())
                                .FontSize(32)
                                .Bold()
                                .FontColor(Colors.Green.Medium);

                            col.Item().AlignCenter().Text("Clients")
                                .FontSize(12)
                                .FontColor(Colors.Grey.Medium);
                        });

                    row.Spacing(10);

                    row.RelativeItem().BorderColor(Colors.Orange.Lighten3).Border(1).Padding(10)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text($"{donnees.RevenuTotal:C}")
                                .FontSize(32)
                                .Bold()
                                .FontColor(Colors.Orange.Medium);

                            col.Item().AlignCenter().Text("Revenu Total")
                                .FontSize(12)
                                .FontColor(Colors.Grey.Medium);
                        });
                });
            });
        }

        private void SectionProjets(IContainer container, RapportData donnees)
        {
            container.Column(column =>
            {
                column.Item().Text("PROJETS DE LA PÉRIODE")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Medium);

                column.Item().PaddingTop(10).Height(1).Background(Colors.Grey.Lighten2);

                if (donnees.Projets != null && donnees.Projets.Any())
                {
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        // En-tête du tableau
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Projet").Bold();

                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Client").Bold();

                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Date début").Bold();

                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Budget").Bold();

                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Statut").Bold();
                        });

                        // Lignes du tableau
                        foreach (var projet in donnees.Projets)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(projet.Titre);

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(projet.Client);

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(projet.DateDebut.ToString("dd/MM/yyyy"));

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text($"{projet.Budget:C}");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(projet.Statut);
                        }
                    });
                }
                else
                {
                    column.Item().PaddingTop(10)
                        .Text("Aucun projet pour cette période")
                        .Italic()
                        .FontColor(Colors.Grey.Medium);
                }
            });
        }

        private void SectionTopClients(IContainer container, RapportData donnees)
        {
            container.Column(column =>
            {
                column.Item().Text("TOP 5 CLIENTS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Medium);

                column.Item().PaddingTop(10).Height(1).Background(Colors.Grey.Lighten2);

                if (donnees.TopClients != null && donnees.TopClients.Any())
                {
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Client").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Projets").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Revenu").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                .Text("Note").Bold();
                        });

                        foreach (var client in donnees.TopClients)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(client.NomEntreprise);

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(client.NombreProjets.ToString());

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text($"{client.RevenuGenere:C}");

                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text($"{client.Note:F1}/5");
                        }
                    });
                }
            });
        }

        private void SectionStatistiquesFinancieres(IContainer container, RapportData donnees)
        {
            container.Column(column =>
            {
                column.Item().Text("STATISTIQUES FINANCIÈRES")
                    .FontSize(16)
                    .Bold()
                    .FontColor(Colors.Blue.Medium);

                column.Item().PaddingTop(10).Height(1).Background(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Revenu total : ").Bold();
                            text.Span($"{donnees.RevenuTotal:C}").FontColor(Colors.Green.Medium);
                        });

                        col.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Revenu moyen par projet : ").Bold();
                            text.Span($"{donnees.RevenuMoyen:C}");
                        });

                        col.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Nombre de factures : ").Bold();
                            text.Span(donnees.NombreFactures.ToString());
                        });
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Factures payées : ").Bold();
                            text.Span($"{donnees.FacturesPayees}/{donnees.NombreFactures}");
                        });

                        col.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Taux de réussite : ").Bold();
                            text.Span($"{donnees.TauxReussite:F1}%").FontColor(Colors.Green.Medium);
                        });
                    });
                });
            });
        }

        private void ComposerPiedDePage(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().Height(1).Background(Colors.Grey.Lighten2);

                // On définit le style PAR DÉFAUT pour tout ce qui est dans ce bloc texte
                column.Item().PaddingTop(5).Text(text =>
                {
                    // On applique le style ici
                    text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Medium));

                    text.Span("Généré par Freelancer Portfolio Manager - ");
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                }); // <-- On termine par un point-virgule ici, rien après la parenthèse
            });
        }

        public async Task<byte[]> GenererFacturePdfAsync(Facture facture)
        {
            // TODO: Implémenter la génération de facture PDF
            await Task.CompletedTask;
            return new byte[0];
        }

        public async Task<string> SauvegarderPdfAsync(byte[] pdfBytes, string nomFichier)
        {
            var cheminComplet = Path.Combine(_rapportsPath, nomFichier);
            await File.WriteAllBytesAsync(cheminComplet, pdfBytes);
            return cheminComplet;
        }
    }
}