using System.ComponentModel.DataAnnotations;

namespace LegacyArchive.Application.DTOs;

public class UpdateCategoryRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
