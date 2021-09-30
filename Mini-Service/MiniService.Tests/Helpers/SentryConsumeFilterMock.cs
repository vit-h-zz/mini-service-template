using GreenPipes;
using MassTransit;
using Sentry;
using System;
using System.Threading.Tasks;

namespace MiniService.Tests.Helpers
{
    public class SentryConsumeFilterMock<TMessage> : IFilter<ConsumeContext<TMessage>> where TMessage : class
    {
        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (Exception e) //only for real bugs
            {
                CaptureException(null!, e);

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
            context.CreateFilterScope("SentryConsumeFilter");
        }

        private void CaptureException(IHub hub, Exception e)
        {
        }
    }
}
