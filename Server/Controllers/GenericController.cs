using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepositoryInterface) : 
        ControllerBase where T : class
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await genericRepositoryInterface.GetAll());

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("invalid request sent");
            return Ok(await genericRepositoryInterface.DeleteById(id));
        }

        [HttpGet("single/{id}")] // Changed from HttpDelete to HttpGet for getting a single item
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("invalid request sent");
            return Ok(await genericRepositoryInterface.GetById(id));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await genericRepositoryInterface.Insert(model)); // Added missing semicolon
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null) return BadRequest("Bad request made");
            return Ok(await genericRepositoryInterface.Update(model)); // Added missing semicolon
        }
    }
}