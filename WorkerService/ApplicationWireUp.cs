﻿using EventStoreAdapter;
using HttpClientAdapter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ports;
using ProtoActorAdapter;

namespace WorkerService
{
    public static class ApplicationWireUp
    {
        public static void RegisterApplicationComponentsUsing(this IServiceCollection service, IConfiguration configuration) 
            => service
                .RegisterLogger()
                .RegisterEventStoreAdapterUsing(configuration)
                .RegisterStreamPrefixUsing(configuration)
                .RegisterProtoActorAdapterUsing(configuration)
                .RegisterHttpClientAdapterUsing(configuration)
                .AddHostedService<Worker>();

        private static IServiceCollection RegisterLogger(this IServiceCollection service)
            => service.AddSingleton(provider => Logger.New());

        private static IServiceCollection RegisterEventStoreAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(provider =>
                EventStoreReader.BuildUsing(
                    configuration.EventStoreConnectionString(),
                    provider.GetService<IInternalLogger>()));

        private static IServiceCollection RegisterStreamPrefixUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(configuration.StreamPrefix());

        private static IServiceCollection RegisterProtoActorAdapterUsing(this IServiceCollection service,
            IConfiguration configuration)
            => service.AddSingleton(provider => ProtoActorDomainEventApplierBuilder.New()
                .Using(configuration.SnapshotConfiguration())
                .DecorateWith(provider.GetService<IInternalLogger>())
                .Build());
        
        private static IServiceCollection RegisterHttpClientAdapterUsing(this IServiceCollection service, IConfiguration configuration)
            => service.AddSingleton(provider => HttpApplyDomainEventStrategyBuilder.New()
                .WithDestinationUri(configuration.DestinationServiceUri())
                .WithMaxConnectionsPerServer(configuration.MaxConnectionsPerServer())
                .DecorateWith(provider.GetService<IInternalLogger>())
                .Build());
    }
}