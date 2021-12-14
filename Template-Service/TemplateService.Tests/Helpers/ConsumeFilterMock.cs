using GreenPipes;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace TemplateService.Tests.Helpers
{
    public class ConsumeFilterMock<TMessage> : IFilter<ConsumeContext<TMessage>> where TMessage : class
    {
        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (Exception) //only for real bugs
            {
                // if (context.ResponseAddress != null) //figure out
                //await context.RespondAsync<ErrorMessage>(new object());

                // var error = _errorHandler.HandleException(e);
                // context.Response.StatusCode = error.Code;
                // await context.Response.WriteAsync( JsonSerializer.Serialize(error), CancellationToken.None);

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(ConsumeFilterMock<TMessage>));
        }
    }
}
