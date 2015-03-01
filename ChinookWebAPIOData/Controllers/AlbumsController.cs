using ChinookWebAPIOData.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace ChinookWebAPIOData.Controllers
{
    public class AlbumsController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool AlbumExists(int key)
        {
            return db.Albums.Any(p => p.AlbumId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //[HttpPost]
        //public async Task<IHttpActionResult> Buy([FromODataUri] int key, ODataActionParameters parameters)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    var album = db.Albums.FirstOrDefault(a => a.AlbumId == key);

        //    if (album == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Ok(album);
        //    }
        //}

        [EnableQuery]
        public IQueryable<Album> Get()
        {
            return db.Albums;
        }
        [EnableQuery]
        public SingleResult<Album> Get([FromODataUri] int key)
        {
            IQueryable<Album> result = db.Albums.Where(p => p.AlbumId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Album album)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Albums.Add(album);
            await db.SaveChangesAsync();
            return Created(album);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Album> Album)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Albums.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Album.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Album update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.AlbumId)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var Album = await db.Albums.FindAsync(key);
            if (Album == null)
            {
                return NotFound();
            }
            db.Albums.Remove(Album);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Track> GetTracks([FromODataUri] int key)
        {
            return db.Albums.Where(m => m.AlbumId == key).SelectMany(m => m.Tracks);
        }

        [EnableQuery]
        public SingleResult<Artist> GetArtist([FromODataUri] int key)
        {
            var result = db.Albums.Where(m => m.AlbumId == key).Select(m => m.Artist);
            return SingleResult.Create(result);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key,
            string navigationProperty, [FromBody] Uri link)
        {
            var album = await db.Albums.SingleOrDefaultAsync(p => p.AlbumId == key);
            if (album == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "Artist":
                    var relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var artist = await db.Artists.SingleOrDefaultAsync(f => f.ArtistId == relatedKey);
                    if (artist == null)
                    {
                        return NotFound();
                    }

                    album.Artist = artist;
                    break;

                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key,
        string navigationProperty, [FromBody] Uri link)
        {
            var album = await db.Albums.SingleOrDefaultAsync(p => p.AlbumId == key);
            if (album == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Artist":
                    album.Artist = null;
                    break;

                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key,
        [FromODataUri] string relatedKey, string navigationProperty)
        {
            var album = await db.Albums.SingleOrDefaultAsync(p => p.AlbumId == key);
            if (album == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Tracks":
                    var trackId = Convert.ToInt32(relatedKey);
                    var track = await db.Tracks.SingleOrDefaultAsync(p => p.TrackId == trackId);

                    if (track == null)
                    {
                        return NotFound();
                    }
                    track.Album = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);

            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}