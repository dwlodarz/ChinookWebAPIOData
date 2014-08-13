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
    public class TracksController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool TrackExists(int key)
        {
            return db.Tracks.Any(p => p.TrackId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Track> Get()
        {
            return db.Tracks;
        }
        [EnableQuery]
        public SingleResult<Track> Get([FromODataUri] int key)
        {
            IQueryable<Track> result = db.Tracks.Where(p => p.TrackId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Track Track)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Tracks.Add(Track);
            await db.SaveChangesAsync();
            return Created(Track);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Track> Track)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Tracks.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Track.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Track update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.TrackId)
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
                if (!TrackExists(key))
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
            var Track = await db.Tracks.FindAsync(key);
            if (Track == null)
            {
                return NotFound();
            }
            db.Tracks.Remove(Track);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}