using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData.Query;
using System.Web.OData.Query.Validators;

namespace ChinookWebAPIOData.Extensions
{
    public class MyOrderByValidator : OrderByQueryValidator
    {
        // Disallow the 'desc' parameter for $orderby option.
        public override void Validate(OrderByQueryOption orderByOption,
                                        ODataValidationSettings validationSettings)
        {
            if (orderByOption.OrderByNodes.Any(
                    node => node.Direction == OrderByDirection.Descending))
            {
                throw new ODataException("The 'desc' option is not supported.");
            }
            base.Validate(orderByOption, validationSettings);
        }
    }
}
