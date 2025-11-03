using Microsoft.AspNetCore.Mvc;
using StadiumSensor.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StadiumSensor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventSummaryController : ControllerBase
    {
        private readonly ISensorEventRepo _repository;
        private readonly ILogger<EventSummaryController> _logger;

        public EventSummaryController(
            ISensorEventRepo repository,
            ILogger<EventSummaryController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorEventSummaryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEventSummary([FromQuery] string? gate = null,
            [FromQuery] string? type = null,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.GetSummaryAsync(gate, type, startTime, endTime, cancellationToken);
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events summary");
                return Problem(title: "Error retrieving events summary", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
