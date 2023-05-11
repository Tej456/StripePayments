using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
       private readonly AppDbContext _appDbContext;
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book bookToCreate)
        {
            bookToCreate.Id = Guid.NewGuid().ToString();

            await _appDbContext.AddAsync(bookToCreate);

            await _appDbContext.SaveChangesAsync();

            return Ok(bookToCreate);
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> Get()
        {
            return await _appDbContext.Products.ToListAsync();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Book updatedBook)
        {
            _appDbContext.Update(updatedBook);

            await _appDbContext.SaveChangesAsync();

            return Ok(updatedBook);
        }

        [HttpDelete]
        [Route("{productToDeleteId}")]
        public async Task<IActionResult> Update(string bookToDeleteId)
        {
            var bookToDeleted = await _appDbContext.Products.FindAsync(bookToDeleteId);

            _appDbContext.Remove(bookToDeleted);

            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}