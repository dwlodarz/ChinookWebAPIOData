using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ChinookWebAPIOData.Models;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using ChinookWebAPIOData.Extensions;

namespace ChinookWebAPIOData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Album>("Albums");
            builder.EntitySet<Artist>("Artists");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Employee>("Employees");
            builder.EntitySet<Genre>("Genres");
            builder.EntitySet<Invoice>("Invoices");
            builder.EntitySet<InvoiceLine>("InvoiceLines");
            builder.EntitySet<MediaType>("MediaTypes");
            builder.EntitySet<Playlist>("Playlists");
            builder.EntitySet<Track>("Tracks");

            // Fix for the DateTime/DateTimeOffset issue
            var inv = builder.StructuralTypes.First(t => t.ClrType == typeof(Invoice));
            inv.AddProperty(typeof(Invoice).GetProperty("InvoiceDateOffset"));
            var invoice = builder.EntityType<Invoice>();

            invoice.Ignore(t => t.InvoiceDate);

            // Adding a Function
            var invoiceType = builder.EntityType<Invoice>();

            invoiceType
                .Function("CalculateSalesTax")
                .Returns<decimal>()
                .Parameter<string>("state");

            // Adding an Actions

            // Buy
            // URI: ~/odata/Albums(2)/ChinookWebAPIOData.Models.Buy
            ActionConfiguration checkOutAction = builder.EntityType<Album>().Action("Buy");
            checkOutAction.ReturnsFromEntitySet<Album>("Albums");

            // Adding custom Query Validators
            config.AddODataQueryFilter(new MyQueryableAttribute());

            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel());
        }
    }
}
