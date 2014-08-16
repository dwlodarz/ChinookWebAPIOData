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
    public class InvoiceLinesController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool InvoiceLineExists(int key)
        {
            return db.InvoiceLines.Any(p => p.InvoiceLineId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<InvoiceLine> Get()
        {
            return db.InvoiceLines;
        }
        [EnableQuery]
        public SingleResult<InvoiceLine> Get([FromODataUri] int key)
        {
            IQueryable<InvoiceLine> result = db.InvoiceLines.Where(p => p.InvoiceLineId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(InvoiceLine InvoiceLine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.InvoiceLines.Add(InvoiceLine);
            await db.SaveChangesAsync();
            return Created(InvoiceLine);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<InvoiceLine> InvoiceLine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.InvoiceLines.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            InvoiceLine.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceLineExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, InvoiceLine update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.InvoiceLineId)
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
                if (!InvoiceLineExists(key))
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
            var InvoiceLine = await db.InvoiceLines.FindAsync(key);
            if (InvoiceLine == null)
            {
                return NotFound();
            }
            db.InvoiceLines.Remove(InvoiceLine);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public SingleResult<Invoice> GetInvoice([FromODataUri] int key)
        {
            var result = db.InvoiceLines.Where(m => m.InvoiceLineId == key).Select(m => m.Invoice);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public SingleResult<Track> GetTrack([FromODataUri] int key)
        {
            var result = db.InvoiceLines.Where(m => m.InvoiceLineId == key).Select(m => m.Track);
            return SingleResult.Create(result);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key,
            string navigationProperty, [FromBody] Uri link)
        {
            var invoiceLine = await db.InvoiceLines.SingleOrDefaultAsync(p => p.InvoiceLineId == key);
            if (invoiceLine == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "Invoice":
                    var relatedInvoiceKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var invoice = await db.Invoices.SingleOrDefaultAsync(f => f.InvoiceId == relatedInvoiceKey);
                    if (invoice == null)
                    {
                        return NotFound();
                    }

                    invoiceLine.Invoice = invoice;
                    break;
                case "Track":
                    var relatedTrackKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var track = await db.Tracks.SingleOrDefaultAsync(f => f.TrackId == relatedTrackKey);
                    if (track == null)
                    {
                        return NotFound();
                    }

                    invoiceLine.Track = track;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}