using ChinookWebAPIOData.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ChinookWebAPIOData.Controllers
{
    public class ArtistsController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool ArtistExists(int key)
        {
            return db.Artists.Any(p => p.ArtistId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Artist> Get()
        {
            return db.Artists;
        }
        [EnableQuery]
        public SingleResult<Artist> Get([FromODataUri] int key)
        {
            IQueryable<Artist> result = db.Artists.Where(p => p.ArtistId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Artist Artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Artists.Add(Artist);
            await db.SaveChangesAsync();
            return Created(Artist);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Artist> Artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Artists.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Artist.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Artist update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.ArtistId)
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
                if (!ArtistExists(key))
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
            var Artist = await db.Artists.FindAsync(key);
            if (Artist == null)
            {
                return NotFound();
            }
            db.Artists.Remove(Artist);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Album> GetAlbums([FromODataUri] int key)
        {
            return db.Artists.Where(m => m.ArtistId == key).SelectMany(m => m.Albums);
        }
    }
}