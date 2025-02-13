using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseLibrary.Entities;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TownController(IGenericRepositoryInterface<Town> genericRepositoryInterface) 
        : GenericController<Town>(genericRepositoryInterface)
    {
        
    }
}