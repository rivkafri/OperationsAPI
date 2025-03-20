// Controllers/OperationsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OperationsAPI.Data;
using OperationsAPI.DTO;
using OperationsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly OperationsDbContext _context;

    public OperationsController(OperationsDbContext context)
    {
        _context = context;
    }

    // GET: api/Operations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Operation>>> GetOperations()
    {
        return await _context.Operations.ToListAsync();
    }

    // Calculate
    [HttpPost("calculate")]
    public async Task<ActionResult<string>> Calculate([FromBody] CalculationRequest request)
    {
        var operation = await _context.Operations.FindAsync(request.OperationId);
        if (operation == null)
        {
            return NotFound("Operation not found");
        }

        string result;
        if (operation.SupportsNumbers && double.TryParse(request.ValueA, out double numA) && double.TryParse(request.ValueB, out double numB))
        {
            result = operation.Name switch
            {
                "Add" => (numA + numB).ToString(),
                "Subtract" => (numA - numB).ToString(),
                "Multiply" => (numA * numB).ToString(),
                "Divide" => numB != 0 ? (numA / numB).ToString() : "Error: Division by zero",
                _ => "Unsupported operation"
            };
        }
        else if (operation.SupportsStrings)
        {
            result = operation.Name switch
            {
                "Concat" => request.ValueA + request.ValueB,
                _ => "Unsupported operation"
            };
        }
        else
        {
            result = "Invalid input";
        }

        var history = new OperationHistory
        {
            OperationId = request.OperationId,
            ValueA = request.ValueA,
            ValueB = request.ValueB,
            Result = result
        };

        _context.OperationHistory.Add(history);
        await _context.SaveChangesAsync();

        return Ok(result);
    }

    // Add a new operation
    [HttpPost("add")]
    public async Task<ActionResult<Operation>> AddOperation([FromBody] OperationDto operationDto)
    {
        var operation = new Operation
        {
            Name = operationDto.Name,
            SupportsNumbers = operationDto.SupportsNumbers,
            SupportsStrings = operationDto.SupportsStrings
        };

        _context.Operations.Add(operation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOperations), new { id = operation.OperationId }, operation); // ✅ מחזירים ישירות את ה-Operation
    }

    // Update an existing operation
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateOperation(int id, [FromBody] OperationDto operationDto)
    {
        if (operationDto == null)
        {
            return BadRequest("Invalid request: No data received.");
        }
        Console.WriteLine($"Updating ID: {id} | Name: {operationDto.Name} | SupportsNumbers: {operationDto.SupportsNumbers} | SupportsStrings: {operationDto.SupportsStrings}");

        var operation = await _context.Operations.FindAsync(id);

        if (operation == null)
        {
            return NotFound($"Operation with ID {id} not found.");
        }
        operation.Name = operationDto.Name;
        operation.SupportsNumbers = operationDto.SupportsNumbers;
        operation.SupportsStrings = operationDto.SupportsStrings;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating operation: {ex.Message}");
            return StatusCode(500, "Internal Server Error.");
        }
    }

    // Delete an existing operation
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOperation(int id)
    {
        var operation = await _context.Operations.FindAsync(id);
        if (operation == null)
        {
            return NotFound();
        }

        _context.Operations.Remove(operation);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // Bonus: Get operation history
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<OperationHistoryDto>>> GetOperationHistory()
    {
        var history = await _context.OperationHistory
            .Select(h => new OperationHistoryDto
            {
                HistoryId = h.HistoryId,
                OperationId = h.OperationId,
                ValueA = h.ValueA,
                ValueB = h.ValueB,
                Result = h.Result,
                CreatedAt = h.CreatedAt
            })
            .ToListAsync();

        return Ok(history);
    }

    // Bonus: Get additional statistics after Calculate
    [HttpGet("statistics")]
    public async Task<ActionResult<OperationStatisticsDto>> GetOperationStatistics()
    {
        var lastThreeOperations = await _context.OperationHistory
            .OrderByDescending(h => h.CreatedAt)
            .Take(3)
            .ToListAsync();

        var operationsCount = await _context.OperationHistory
            .GroupBy(h => h.OperationId)
            .Select(g => new OperationCountDto
            {
                OperationId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var statistics = new OperationStatisticsDto
        {
            LastThreeOperations = lastThreeOperations,
            OperationsCount = operationsCount
        };

        return Ok(statistics);
    }

    private bool OperationExists(int id)
    {
        return _context.Operations.Any(e => e.OperationId == id);
    }
}

public class CalculationRequest
{
    public int OperationId { get; set; }
    public string ValueA { get; set; }
    public string ValueB { get; set; }
}
public class OperationCountDto
{
    public int OperationId { get; set; }
    public int Count { get; set; }
}

public class OperationStatisticsDto
{
    public List<OperationHistory> LastThreeOperations { get; set; }
    public List<OperationCountDto> OperationsCount { get; set; }
}

