using Microsoft.AspNetCore.Mvc;

namespace Diplom_project_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        public IActionResult Index()
        {
            return NoContent();
        }
    }
}
