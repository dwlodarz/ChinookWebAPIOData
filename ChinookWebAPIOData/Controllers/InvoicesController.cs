using ChinookWebAPIOData.Models;
using System;
using System.Collections.Concurrent;
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
    public class InvoicesController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private static ConcurrentDictionary<int, Invoice> _data;

        static InvoicesController()
        {
            _data = new ConcurrentDictionary<int, Invoice>();
        }

        public InvoicesController()
        {
        }

        private bool InvoiceExists(int key)
        {
            return db.Invoices.Any(p => p.InvoiceId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        public IHttpActionResult CalculateSalesTax([FromODataUri] int key, string state)
        {
            decimal taxRate = GetRate(state);

            var inv = db.Invoices.FirstOrDefault(p => p.InvoiceId == key);
            if (inv != null)
            {
                decimal tax = inv.Total * taxRate / 100;
                return Ok(tax);
            }
            else
            {
                return NotFound();
            }
        }

        private static decimal GetRate(string state)
        {
            decimal taxRate = 0;
            switch (state)
            {
                case "AZ": taxRate = 5.6M; break;
                case "CA": taxRate = 7.5M; break;
                case "CT": taxRate = 6.35M; break;
                case "GA": taxRate = 4M; break;
                case "IN": taxRate = 7M; break;
                case "KS": taxRate = 6.15M; break;
                case "KY": taxRate = 6M; break;
                case "MA": taxRate = 6.25M; break;
                case "MI": taxRate = 6.5M; break;
                case "NV": taxRate = 6.85M; break;
                case "NJ": taxRate = 7M; break;
                case "NY": taxRate = 4; break;
                case "NC": taxRate = 4.75M; break;
                case "ND": taxRate = 5; break;
                case "PA": taxRate = 6; break;
                case "TN": taxRate = 7; break;
                case "TX": taxRate = 6.25M; break;
                case "VA": taxRate = 4.3M; break;
                case "WA": taxRate = 6.5M; break;
                case "WV": taxRate = 6.0M; break;
                case "WI": taxRate = 5.0M; break;

                default:
                    taxRate = 0;
                    break;
            }

            return taxRate;
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

        [EnableQuery]
        public IQueryable<InvoiceLine> GetInvoiceLines([FromODataUri] int key)
        {
            return db.Invoices.Where(m => m.InvoiceId == key).SelectMany(m => m.InvoiceLines);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key,
            string navigationProperty, [FromBody] Uri link)
        {
            var invoice = await db.Invoices.SingleOrDefaultAsync(p => p.InvoiceId == key);
            if (invoice == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "Customer":
                    var relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var customer = await db.Customers.SingleOrDefaultAsync(f => f.CustomerId == relatedKey);
                    if (customer == null)
                    {
                        return NotFound();
                    }

                    invoice.Customer = customer;
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
            var invoice = await db.Invoices.SingleOrDefaultAsync(p => p.InvoiceId == key);
            if (invoice == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "InvoiceLines":
                    var invoiceLineId = Convert.ToInt32(relatedKey);
                    var invoiceLine = await db.InvoiceLines.SingleOrDefaultAsync(p => p.InvoiceLineId == invoiceLineId);

                    if (invoiceLine == null)
                    {
                        return NotFound();
                    }
                    invoiceLine.Invoice = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);

            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}