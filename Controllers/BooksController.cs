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
    public class BooksController : ODataController
    {
        private readonly OdataNet6TutorialContext odataNet6CoreDBContext;
        private readonly IConfiguration configuration;

        public BooksController(OdataNet6TutorialContext context)
        {
            this.odataNet6CoreDBContext = context;
        }

        [HttpGet]
        [EnableQuery(PageSize = 2)]
        public ActionResult<IQueryable<Books>> Get()
        {
            return this.Ok(this.odataNet6CoreDBContext.Books
                .AsNoTracking());
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<Books> Get([FromODataUri] Guid key)
        {
            var result = this.odataNet6CoreDBContext.Books.Where(c => c.Guid == key);
            return SingleResult.Create(result);
        }

        [HttpPost]
        [EnableQuery]
        public async Task<ActionResult<Authors>> Post([FromBody] Books item)
        {
            if (!this.ModelState.IsValid)
            {
                return this.StatusCode(403, this.ModelState);
            }
            this.odataNet6CoreDBContext.Books.Add(item);

            // Define the cancellation token.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            await this.odataNet6CoreDBContext.SaveChangesAsync(token);

            var results = this.CreatedAtAction(nameof(this.Get), new { id = item.Guid }, item);

            return this.Ok(results.Value);
        }

        [HttpPatch]
        [EnableQuery]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Books> books)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var entity = await this.odataNet6CoreDBContext.Books.FindAsync(key);
            if (entity == null)
            {
                return this.NotFound();
            }

            books.Patch(entity);
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
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            try
            {
                var deleteBooks = await this.odataNet6CoreDBContext.Books.FindAsync(key);
                if (deleteBooks == null)
                {
                    return this.StatusCode(404, "Book does not exist");
                }

                // Time Entry Delete
                this.odataNet6CoreDBContext.Books.Remove(deleteBooks);

                // Define the cancellation token.
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                await this.odataNet6CoreDBContext.SaveChangesAsync(token);
                return this.StatusCode(204, $"Book removed");
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error -> {ex.InnerException.Message.ToString()}");
            }
        }
    }
}
