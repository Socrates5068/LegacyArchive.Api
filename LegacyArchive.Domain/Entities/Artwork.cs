using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyArchive.Domain.Entities;

public class Artwork
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public DateTime CreatedAt { get; set; }
}
