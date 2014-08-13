namespace ChinookWebAPIOData.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Genre")]
    public partial class Genre
    {
        public Genre()
        {
            Tracks = new HashSet<Track>();
        }

        public int GenreId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
