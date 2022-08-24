using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace LostArkBot.Bot.QuartzJobs;

public class CustomJobFactory : IJobFactory
{
    private readonly IServiceProvider serviceProvider;

    private readonly ConcurrentDictionary<IJob, IServiceScope> scopes = new();

    public CustomJobFactory(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        IServiceScope scope = this.serviceProvider.CreateScope();
        IJob job;

        try
        {
            job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }
        catch
        {
            // Failed to create the job -> ensure scope gets disposed
            scope.Dispose();

            throw;
        }

        // Add scope to dictionary so we can dispose it once the job finishes
        if (!this.scopes.TryAdd(job, scope))
        {
            // Failed to track DI scope -> ensure scope gets disposed
            scope.Dispose();

            throw new Exception("Failed to track DI scope");
        }

        return job;
    }

    public void ReturnJob(IJob job)
    {
        if (this.scopes.TryRemove(job, out IServiceScope scope))
        {
            // The Dispose() method ends the scope lifetime.
            // Once Dispose is called, any scoped services that have been resolved from ServiceProvider will be disposed.
            scope.Dispose();
        }
    }
}