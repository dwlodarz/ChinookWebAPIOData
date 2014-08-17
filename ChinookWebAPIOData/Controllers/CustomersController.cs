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
    public class CustomersController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool CustomerExists(int key)
        {
            return db.Customers.Any(p => p.CustomerId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return db.Customers;
        }
        [EnableQuery]
        public SingleResult<Customer> Get([FromODataUri] int key)
        {
            IQueryable<Customer> result = db.Customers.Where(p => p.CustomerId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Customer Customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Customers.Add(Customer);
            await db.SaveChangesAsync();
            return Created(Customer);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Customer> Customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Customers.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Customer.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Customer update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.CustomerId)
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
                if (!CustomerExists(key))
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
            var Customer = await db.Customers.FindAsync(key);
            if (Customer == null)
            {
                return NotFound();
            }
            db.Customers.Remove(Customer);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public IQueryable<Invoice> GetTracks([FromODataUri] int key)
        {
            return db.Customers.Where(m => m.CustomerId == key).SelectMany(m => m.Invoices);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key,
            string navigationProperty, [FromBody] Uri link)
        {
            var customer = await db.Customers.SingleOrDefaultAsync(p => p.CustomerId == key);
            if (customer == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "Employee":
                    var relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var employee = await db.Employees.SingleOrDefaultAsync(f => f.EmployeeId == relatedKey);
                    if (employee == null)
                    {
                        return NotFound();
                    }

                    customer.Employee = employee;
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
            var customer = await db.Customers.SingleOrDefaultAsync(p => p.CustomerId == key);
            if (customer == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Employee":
                    customer.Employee = null;
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
            var customer = await db.Customers.SingleOrDefaultAsync(p => p.CustomerId == key);
            if (customer == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Invoices":
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