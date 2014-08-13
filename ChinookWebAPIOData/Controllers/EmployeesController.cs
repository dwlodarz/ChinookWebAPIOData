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
    public class EmployeesController : ODataController
    {
        ChinookModel db = new ChinookModel();
        private bool EmployeeExists(int key)
        {
            return db.Employees.Any(p => p.EmployeeId == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Employee> Get()
        {
            return db.Employees;
        }
        [EnableQuery]
        public SingleResult<Employee> Get([FromODataUri] int key)
        {
            IQueryable<Employee> result = db.Employees.Where(p => p.EmployeeId == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Employee Employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Employees.Add(Employee);
            await db.SaveChangesAsync();
            return Created(Employee);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Employee> Employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.Employees.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            Employee.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(key))
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
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Employee update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.EmployeeId)
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
                if (!EmployeeExists(key))
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
            var Employee = await db.Employees.FindAsync(key);
            if (Employee == null)
            {
                return NotFound();
            }
            db.Employees.Remove(Employee);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}