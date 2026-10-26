namespace BlaChallenge.Api.Controllers;

using System.Security.Claims;
using BlaChallenge.Application.Tasks.Commands.CreateTask;
using BlaChallenge.Application.Tasks.Commands.DeleteTask;
using BlaChallenge.Application.Tasks.Commands.UpdateTask;
using BlaChallenge.Application.Tasks.Contracts;
using BlaChallenge.Application.Tasks.Exceptions;
using BlaChallenge.Application.Tasks.Queries.GetTaskById;
using BlaChallenge.Application.Tasks.Queries.GetTasks;
using BlaChallenge.Application.Users.Exceptions;
using BlaChallenge.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/tasks")]
public sealed class TasksController(
    CreateTaskUseCase createTask,
    GetTasksUseCase getTasks,
    GetTaskByIdUseCase getTaskById,
    UpdateTaskUseCase updateTask,
    DeleteTaskUseCase deleteTask) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(
        TaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var task = await createTask.ExecuteAsync(
                new CreateTaskCommand(
                    ownerId,
                    request.Title,
                    request.Description,
                    request.Status,
                    request.DueDateUtc,
                    GetRole(),
                    request.AssignedToId),
                cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (InvalidUserInputException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        return Ok(await getTasks.ExecuteAsync(new GetTasksQuery(ownerId, GetRole()), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        var task = await getTaskById.ExecuteAsync(new GetTaskByIdQuery(id, ownerId, GetRole()), cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(
        Guid id,
        TaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var task = await updateTask.ExecuteAsync(
                new UpdateTaskCommand(
                    id,
                    ownerId,
                    request.Title,
                    request.Description,
                    request.Status,
                    request.DueDateUtc,
                    GetRole(),
                    request.AssignedToId),
                cancellationToken);
            return Ok(task);
        }
        catch (TaskNotFoundException)
        {
            return NotFound();
        }
        catch (TaskForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
        catch (InvalidUserInputException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            await deleteTask.ExecuteAsync(new DeleteTaskCommand(id, ownerId, GetRole()), cancellationToken);
            return NoContent();
        }
        catch (TaskNotFoundException)
        {
            return NotFound();
        }
        catch (TaskForbiddenException exception)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = exception.Message });
        }
    }

    private bool TryGetUserId(out Guid userId)
        => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

    private UserRole GetRole()
        => Enum.TryParse<UserRole>(User.FindFirstValue(ClaimTypes.Role), out var role) ? role : UserRole.User;

    public sealed record TaskRequest(
        string Title,
        string? Description,
        TaskStatus Status,
        DateTime DueDateUtc,
        Guid? AssignedToId = null);
}
