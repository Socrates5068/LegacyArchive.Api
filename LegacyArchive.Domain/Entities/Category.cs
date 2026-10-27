using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyArchive.Domain.Entities;
public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Artwork> Artworks { get; set; }
        = new List<Artwork>();
}
