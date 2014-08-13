namespace ChinookWebAPIOData.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Artist")]
    public partial class Artist
    {
        public Artist()
        {
            Albums = new HashSet<Album>();
        }

        public int ArtistId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
}
