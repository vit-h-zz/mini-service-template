using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VH.MiniService.Common.Service.Controllers;
using Messaging.TemplateService;
using TemplateService.Application.TodoItems.GetFinishedWork;
using TemplateService.Data.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.TodoItems.Create;
using TemplateService.Application.TodoItems.Delete;
using TemplateService.Application.TodoItems.Get;
using TemplateService.Application.TodoItems.Update;

namespace TemplateService.Features.Todos
{
    public class TodosController : ApiController
    {
        [HttpGet]
        public Task<ActionResult<List<TodoItem>>> GetTodoItems(CancellationToken ct)
        {
            return Send(new GetTodoItemsQuery(), ct).ToActionResult(this);
        }

        [HttpPost]
        public Task<ActionResult<TodoItem>> Create(CreateTodoItemCommand command, CancellationToken ct)
        {
            return Send(command, ct).ToActionResult(this, HttpStatusCode.Created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateTodoItemCommand command, CancellationToken ct)
        {
            return id == command.Id
                ? await Send(command, ct).ToActionResult(this)
                : BadRequest();
        }

        [HttpDelete("{id:int}")]
        public Task<ActionResult> Delete([FromRoute] DeleteTodoItemCommand request, CancellationToken ct)
        {
            return Send(request, ct).ToActionResult(this);
        }

        [HttpGet("finished")]
        public Task<ActionResult<List<WorkItem>>> GetFinishedWork(CancellationToken ct)
        {
            return Send(new GetFinishedWorkQuery(), ct).ToActionResult(this);
        }
    }
}
