using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HealthApp.Models;
using System;
using System.Linq;

namespace HealthApp.Razor.Data
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Vérifie si la table contient déjà des données
            if (context.Events.Any())
            {
                return; // La base est déjà remplie, on ne fait rien
            }

            // Ajouter des événements de test
            context.Events.AddRange(
                new Event
                {
                    PatientName = "Jean Dupont",
                    Date = DateTime.Today.AddDays(2),
                    DoctorId = 1,
                    IsConfirmed = true
                },
                new Event
                {
                    PatientName = "Alice Martin",
                    Date = DateTime.Today.AddDays(5),
                    DoctorId = 1,
                    IsConfirmed = false
                },
                new Event
                {
                    PatientName = "Paul Leroy",
                    Date = DateTime.Today.AddDays(7),
                    DoctorId = 2,
                    IsConfirmed = true
                }
            );

            // Sauvegarder les changements dans la base
            context.SaveChanges();
        }
    }
}