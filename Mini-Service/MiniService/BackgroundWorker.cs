using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common.Errors;
using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniService.Application.TodoItems.Create;

namespace MiniService
{
    /// Example of Hosted service
    public class BackgroundWorker : BackgroundService //, ISomeService
    {
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly IOptionsMonitor<BackgroundWorkerOptions> _optionsMonitor;
        private readonly IServiceProvider _serviceProvider;

        private readonly Channel<CreateTodoItemCommand> _queue;
        private readonly CancellationTokenSource _serviceStoppingTokenSource;
        private bool _stopRequested;

        public BackgroundWorker(
            IServiceProvider serviceProvider,
            ILogger<BackgroundWorker> logger,
            IOptionsMonitor<BackgroundWorkerOptions> optionsMonitor)
        {
            _logger = logger;
            _optionsMonitor = optionsMonitor;
            _serviceProvider = serviceProvider;
            _queue = Channel.CreateUnbounded<CreateTodoItemCommand>();
            _serviceStoppingTokenSource = new CancellationTokenSource();
        }

        public async Task<Result> AddWorkAsync(CreateTodoItemCommand cmd, CancellationToken ct)
        {
            if (_stopRequested)
            {
                const string Message = "Can't be completed due to service is shutting down.";
                return Result.Fail(new ConflictError(Message, Message));
            }

            await _queue.Writer.WriteAsync(cmd, ct);

            return Result.Ok();
        }

        protected override async Task ExecuteAsync(CancellationToken appStoppingToken)
        {
            await foreach (var cmd in _queue.Reader.ReadAllAsync(CancellationToken.None))
            {
                try
                {
                    _logger.LogInformation("Creating started {@Command}", cmd);

                    using var scope = _serviceProvider.CreateScope();
                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    await sender.Send(cmd, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Creating error {@Command}", cmd);
                }
            }

            _serviceStoppingTokenSource.Cancel();
        }

        public override async Task StopAsync(CancellationToken appStoppingToken)
        {
            _stopRequested = true;

            _logger.LogInformation($"{nameof(BackgroundWorker)} stopping requested.");

            var queueProceededToken = _serviceStoppingTokenSource.Token;
            _queue.Writer.Complete();

            await Task
                .Delay(_optionsMonitor.CurrentValue.MaxStoppingDelayMs, queueProceededToken)
                .HideCanceledEx();

            _logger.LogInformation($"{nameof(BackgroundWorker)} stopping, queue is empty.");

            await base.StopAsync(appStoppingToken);
        }
    }

    public class BackgroundWorkerOptions
    {
        public int MaxStoppingDelayMs { get; set; } = 60_000;
    }

    public static class TaskExtension
    {
        public static async Task HideCanceledEx(this Task task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
        }
    }
}
