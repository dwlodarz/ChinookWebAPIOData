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
    public class GenresController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool GenreExists(int key)
        {
            return db.Genres.Any(p => p.GenreId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Genre> Get()
        {
            return db.Genres;
        }
        [EnableQuery]
        public SingleResult<Genre> Get([FromODataUri] int key)
        {
            IQueryable<Genre> result = db.Genres.Where(p => p.GenreId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Genres.Add(Genre);
            await db.SaveChangesAsync();
            return Created(Genre);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Genre> Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Genres.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Genre.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Genre update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.GenreId)
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
                if (!GenreExists(key))
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
            var Genre = await db.Genres.FindAsync(key);
            if (Genre == null)
            {
                return NotFound();
            }
            db.Genres.Remove(Genre);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Track> GetTracks([FromODataUri] int key)
        {
            return db.Genres.Where(m => m.GenreId == key).SelectMany(m => m.Tracks);
        }
    }
}