using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using OData_webapi_netcore6.Models;
using OData_webapi_netcore6.Services;

namespace OData_webapi_netcore6.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]
    public class AuthorsController : ODataController
    {
        private readonly OdataNet6TutorialContext odataNet6CoreDBContext;

        public AuthorsController(OdataNet6TutorialContext context)
        {
            this.odataNet6CoreDBContext = context;
        }

        /// <summary>
        /// Lists all Authors
        /// </summary>
        /// <returns>Returned all Authors</returns>
        [HttpGet]
        [EnableQuery(PageSize = 2)]
        [Authorize("read:authors")]
        public ActionResult<IQueryable<Authors>> Get()
        {
            return this.Ok(this.odataNet6CoreDBContext.Authors
                .AsNoTracking());
        }

        /// <summary>
        /// Retrieves a specific Author
        /// </summary>
        /// <param name="key">ContactGuid</param>
        /// <returns>A specific Author is returned</returns>
        [HttpGet]
        [EnableQuery]
        [Authorize("read:authors")]
        public SingleResult<Authors> Get([FromODataUri] Guid key)
        {
            var result = this.odataNet6CoreDBContext.Authors.Where(c => c.Guid == key);
            return SingleResult.Create(result);
        }

        [HttpPost]
        [EnableQuery]
        [Authorize("write:authors")]
        public async Task<ActionResult<Authors>> Post([FromBody] Authors item)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(403, this.ModelState);
            }
            this.odataNet6CoreDBContext.Authors.Add(item);

            // Define the cancellation token.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            await this.odataNet6CoreDBContext.SaveChangesAsync(token);

            var results = this.CreatedAtAction(nameof(this.Get), new { id = item.Guid }, item);

            return this.Ok(results.Value);
        }

        [HttpPatch]
        [EnableQuery]
        [Authorize("write:authors")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Authors> authors, ODataQueryOptions<Authors> options)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            // eTag Validation (ConcurrencyCheck)
            if ((options.IfMatch != null &&
                !(options.IfMatch.ApplyTo(this.odataNet6CoreDBContext.Authors.Where(t => t.Guid == key))
                as IQueryable<Authors>).Any()) ||
                options.IfMatch == null)
            {
                return this.StatusCode(412); //PRE CONDITION FAIL
            }

            var entity = await this.odataNet6CoreDBContext.Authors.FindAsync(key);
            if (entity == null)
            {
                return this.NotFound();
            }

            authors.Patch(entity);
            try
            {
                // We need to retrigger the Model State once more so we can rerun the validation rules
                this.TryValidateModel(entity);
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                // Define the cancellation token.
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                await this.odataNet6CoreDBContext.SaveChangesAsync(token);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return this.Updated(entity);
        }

        [HttpDelete]
        [EnableQuery]
        [Authorize("write:authors")]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            try
            {
                var deleteAuthors = await this.odataNet6CoreDBContext.Authors.FindAsync(key);
                if (deleteAuthors == null)
                {
                    return this.StatusCode(404, "Author does not exist");
                }

                // Time Entry Delete
                this.odataNet6CoreDBContext.Authors.Remove(deleteAuthors);

                // Define the cancellation token.
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                await this.odataNet6CoreDBContext.SaveChangesAsync(token);
                return this.StatusCode(204, $"Author removed");
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error -> {ex.InnerException.Message.ToString()}");
            }
        }
    }
}
