using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseLibrary.Entities;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OvertimeController(IGenericRepositoryInterface<Overtime> genericRepositoryInterface) 
        : GenericController<Overtime>(genericRepositoryInterface)
    {
        
    }
}