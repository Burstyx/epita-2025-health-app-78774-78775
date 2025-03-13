using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HealthApp.Razor.Controllers
{
    [Route("Calendrier")]
    public class CalendrierController : Controller
    {
        private static List<object> events = new List<object>
        {
            new { id = 1, title = "Consultation", start = "2024-03-15", color = "#17a2b8" },
            new { id = 2, title = "Opération", start = "2024-03-20", end = "2024-03-22", color = "#dc3545" }
        };

        [HttpGet("GetEvents")]
        public JsonResult GetEvents()
        {
            return Json(events);
        }

        [HttpPost("AddEvent")]
public IActionResult AddEvent([FromBody] EventModel newEvent)
{
    if (newEvent == null || string.IsNullOrEmpty(newEvent.Title) || string.IsNullOrEmpty(newEvent.Start))
    {
        Console.WriteLine("❌ Erreur : Données invalides !");
        return BadRequest(new { status = "error", message = "Données invalides" });
    }

    var newEventObject = new Dictionary<string, object>
    {
        { "id", events.Count + 1 },
        { "title", newEvent.Title },
        { "start", newEvent.Start },
        { "end", string.IsNullOrEmpty(newEvent.End) ? newEvent.Start : newEvent.End },
        { "color", "#28a745" }
    };

    events.Add(newEventObject);

    Console.WriteLine($"✅ Événement ajouté : {System.Text.Json.JsonSerializer.Serialize(newEventObject)}");

    return Ok(new { status = "success", eventData = newEventObject });
}

[HttpPost("DeleteEvent")]
public IActionResult DeleteEvent([FromBody] DeleteEventModel data)
{
    Console.WriteLine($"🔍 Tentative de suppression de l'événement ID: {data?.Id}");

    if (data == null || data.Id == 0)
    {
        return BadRequest(new { status = "error", message = "ID invalide" });
    }

    // Vérifier si la liste contient des événements sous forme de Dictionary
    var eventToRemove = events.FirstOrDefault(e => 
        (e is Dictionary<string, object> dict) && Convert.ToInt32(dict["id"]) == data.Id
    );

    if (eventToRemove != null)
    {
        events.Remove(eventToRemove);
        Console.WriteLine($"✅ Événement supprimé : ID {data.Id}");
        return Ok(new { status = "success" });
    }

    Console.WriteLine("❌ Événement non trouvé !");
    return NotFound(new { status = "error", message = "Événement introuvable" });
}

// Modèle de requête pour la suppression
public class DeleteEventModel
{
    public int Id { get; set; }
}




        public class EventModel
        {
            public string Title { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
        }
    }
}
