using ChinookWebAPIOData.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ChinookWebAPIOData.Controllers
{
    public class MediaTypesController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool MediaTypeExists(int key)
        {
            return db.MediaTypes.Any(p => p.MediaTypeId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<MediaType> Get()
        {
            return db.MediaTypes;
        }
        [EnableQuery]
        public SingleResult<MediaType> Get([FromODataUri] int key)
        {
            IQueryable<MediaType> result = db.MediaTypes.Where(p => p.MediaTypeId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(MediaType MediaType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.MediaTypes.Add(MediaType);
            await db.SaveChangesAsync();
            return Created(MediaType);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<MediaType> MediaType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.MediaTypes.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            MediaType.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaTypeExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, MediaType update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.MediaTypeId)
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
                if (!MediaTypeExists(key))
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
            var MediaType = await db.MediaTypes.FindAsync(key);
            if (MediaType == null)
            {
                return NotFound();
            }
            db.MediaTypes.Remove(MediaType);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Track> GetTracks([FromODataUri] int key)
        {
            return db.MediaTypes.Where(m => m.MediaTypeId == key).SelectMany(m => m.Tracks);
        }

        public async Task<IHttpActionResult> DeleteRef([FromODataUri] int key,
        [FromODataUri] string relatedKey, string navigationProperty)
        {
            var mediaType = await db.MediaTypes.SingleOrDefaultAsync(p => p.MediaTypeId == key);
            if (mediaType == null)
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
                    track.MediaType = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);

            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}