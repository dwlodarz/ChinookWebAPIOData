namespace ChinookWebAPIOData.Migrations
{
    using ChinookWebAPIOData.Models;
    using Ploeh.AutoFixture;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ChinookWebAPIOData.Models.ChinookModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ChinookWebAPIOData.Models.ChinookModel context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //context.People.AddOrUpdate(
            //  p => p.FullName,
            //  new Person { FullName = "Andrew Peters" },
            //  new Person { FullName = "Brice Lambson" },
            //  new Person { FullName = "Rowan Miller" }
            //);
            //

            List<Employee> employees = new List<Employee>
            {
                new Employee{FirstName = "fname1", LastName = "lName1", EmployeeId = 1},
                new Employee{FirstName = "fname2", LastName = "lName2", EmployeeId = 2},
                new Employee{FirstName = "fname3", LastName = "lName3", EmployeeId = 3},
                new Employee{FirstName = "fname4", LastName = "lName4", EmployeeId = 4},
                new Employee{FirstName = "fname5", LastName = "lName5", EmployeeId = 5},
                new Employee{FirstName = "fname6", LastName = "lName6", EmployeeId = 6}
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();
        }
    }
}
