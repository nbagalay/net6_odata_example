using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Query;

namespace OData_webapi_netcore6.Services
{
    public class DefaultSkipTokenHandler : SkipTokenHandler
    {
        public override IQueryable<T> ApplyTo<T>(IQueryable<T> query, SkipTokenQueryOption skipTokenQueryOption, ODataQuerySettings querySettings, ODataQueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public override IQueryable ApplyTo(IQueryable query, SkipTokenQueryOption skipTokenQueryOption, ODataQuerySettings querySettings, ODataQueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public override Uri GenerateNextPageLink(Uri baseUri, int pageSize, object instance, ODataSerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
