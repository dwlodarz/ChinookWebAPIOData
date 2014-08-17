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

        [EnableQuery]
        public IQueryable<Customer> GetCustomers([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeId == key).SelectMany(m => m.Customers);
        }

        [EnableQuery]
        public IQueryable<Employee> GetEmployee1([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeId == key).SelectMany(m => m.Employee1);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key,
            string navigationProperty, [FromBody] Uri link)
        {
            var employee = await db.Employees.SingleOrDefaultAsync(p => p.EmployeeId == key);
            if (employee == null)
            {
                return NotFound();
            }
            switch (navigationProperty)
            {
                case "Employee2":
                    var relatedKey = Helpers.GetKeyFromUri<int>(Request, link);
                    var employee2 = await db.Employees.SingleOrDefaultAsync(f => f.EmployeeId == relatedKey);
                    if (employee2 == null)
                    {
                        return NotFound();
                    }

                    employee.Employee2 = employee2;
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
            var employee = await db.Employees.SingleOrDefaultAsync(p => p.EmployeeId == key);
            if (employee == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Employee2":
                    employee.Employee2 = null;
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
            var employee = await db.Employees.SingleOrDefaultAsync(p => p.EmployeeId == key);
            if (employee == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Customers":
                    var customerId = Convert.ToInt32(relatedKey);
                    var customer = await db.Customers.SingleOrDefaultAsync(p => p.CustomerId == customerId);

                    if (customer == null)
                    {
                        return NotFound();
                    }
                    customer.Employee = null;
                    break;
                case "Employee1":
                    var employeeId = Convert.ToInt32(relatedKey);
                    var employee1 = await db.Employees.SingleOrDefaultAsync(p => p.EmployeeId == employeeId);

                    if (employee1 == null)
                    {
                        return NotFound();
                    }
                    employee1.Employee1 = null;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);

            }
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}