using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateCategoryRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
