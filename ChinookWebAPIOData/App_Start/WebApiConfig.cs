using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ChinookWebAPIOData.Models;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace ChinookWebAPIOData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //// Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

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
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel());
        }
    }
}
