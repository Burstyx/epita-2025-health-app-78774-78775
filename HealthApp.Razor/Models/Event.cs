using System;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int DoctorId { get; set; } // Lien avec le médecin

        public bool IsConfirmed {get; set;}

    }
}
