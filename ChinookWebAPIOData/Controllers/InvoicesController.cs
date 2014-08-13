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
    public class InvoicesController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool InvoiceExists(int key)
        {
            return db.Invoices.Any(p => p.InvoiceId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Invoice> Get()
        {
            return db.Invoices;
        }
        [EnableQuery]
        public SingleResult<Invoice> Get([FromODataUri] int key)
        {
            IQueryable<Invoice> result = db.Invoices.Where(p => p.InvoiceId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Invoice Invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Invoices.Add(Invoice);
            await db.SaveChangesAsync();
            return Created(Invoice);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Invoice> Invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Invoices.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Invoice.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Invoice update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.InvoiceId)
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
                if (!InvoiceExists(key))
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
            var Invoice = await db.Invoices.FindAsync(key);
            if (Invoice == null)
            {
                return NotFound();
            }
            db.Invoices.Remove(Invoice);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}