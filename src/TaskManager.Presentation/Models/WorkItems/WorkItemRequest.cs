using System.ComponentModel.DataAnnotations;

namespace TaskManager.Presentation.Models.WorkItems;

public class WorkItemRequest
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;

    public string WorkItemStatus { get; set; } = "Pending";

    [Required(ErrorMessage = "El responsable es obligatorio.")]
    [StringLength(500, ErrorMessage = "El responsable no puede superar los 500 caracteres.")]
    public string Asignee { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha límite es obligatoria.")]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);
}
