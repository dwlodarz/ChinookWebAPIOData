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
    public class AlbumController : ODataController
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
    }
}