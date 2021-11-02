using System.Threading.Tasks;
using TemplateService.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace TemplateService.Features.Todos
{
    public class TodosService : Grpc.Todos.TodosBase
    {
        private readonly ILogger<TodosService> _logger;

        public TodosService(ILogger<TodosService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task SayHelloStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream,
            ServerCallContext context)
        {
            return Task.CompletedTask;
        }
    }
}
