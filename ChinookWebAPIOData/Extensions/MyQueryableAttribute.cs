using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Query;

namespace ChinookWebAPIOData.Extensions
{
    public class MyQueryableAttribute : EnableQueryAttribute
    {
        public override void ValidateQuery(HttpRequestMessage request,
            ODataQueryOptions queryOptions)
        {
            if (queryOptions.OrderBy != null)
            {
                queryOptions.OrderBy.Validator = new MyOrderByValidator();
            }
            base.ValidateQuery(request, queryOptions);
        }
    }
}
