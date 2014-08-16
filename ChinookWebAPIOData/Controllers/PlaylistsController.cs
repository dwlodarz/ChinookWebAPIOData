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
    public class PlaylistsController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool PlaylistExists(int key)
        {
            return db.Playlists.Any(p => p.PlaylistId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Playlist> Get()
        {
            return db.Playlists;
        }
        [EnableQuery]
        public SingleResult<Playlist> Get([FromODataUri] int key)
        {
            IQueryable<Playlist> result = db.Playlists.Where(p => p.PlaylistId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Playlist Playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Playlists.Add(Playlist);
            await db.SaveChangesAsync();
            return Created(Playlist);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Playlist> Playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Playlists.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Playlist.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Playlist update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.PlaylistId)
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
                if (!PlaylistExists(key))
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
            var Playlist = await db.Playlists.FindAsync(key);
            if (Playlist == null)
            {
                return NotFound();
            }
            db.Playlists.Remove(Playlist);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Track> GetTracks([FromODataUri] int key)
        {
            return db.Playlists.Where(m => m.PlaylistId == key).SelectMany(m => m.Tracks);
        }
    }
}