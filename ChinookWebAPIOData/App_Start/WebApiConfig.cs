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

            var inv = builder.StructuralTypes.First(t => t.ClrType == typeof(Invoice));
            inv.AddProperty(typeof(Invoice).GetProperty("InvoiceDateOffset"));
            var invoice = builder.EntityType<Invoice>();

            invoice.Ignore(t => t.InvoiceDate);

            var invoiceType = builder.EntityType<Invoice>();

            // Function bound to a single entity
            // Accept a string as parameter and return a double
            // This function calculate the sales tax base on the 
            // state

            invoiceType
                .Function("CalculateSalesTax")
                .Returns<decimal>()
                .Parameter<string>("state");

            ActionConfiguration createArtistAction = builder.Action("CreateArtist");
            createArtistAction.Parameter<string>("Name");
            createArtistAction.ReturnsFromEntitySet<Artist>("Artists");

            config.AddODataQueryFilter(new MyQueryableAttribute());

            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel());
        }
    }
}
