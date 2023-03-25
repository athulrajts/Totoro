﻿using System.ComponentModel;
using MalApi;
using MalApi.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Totoro.Core.Services;
using Totoro.Core.Services.AniList;
using Totoro.Core.Services.Debrid;
using Totoro.Core.Services.MediaEvents;
using Totoro.Core.Services.MyAnimeList;
using Totoro.Core.Services.ShanaProject;
using Totoro.Core.Services.StreamResolvers;
using Totoro.Core.Torrents;
using Totoro.Core.ViewModels;

namespace Totoro.Core
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddTotoro(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordRichPresense, DiscordRichPresense>();
            services.AddSingleton<IPlaybackStateStorage, PlaybackStateStorage>();
            services.AddSingleton<IVolatileStateStorage, VolatileStateStorage>();
            services.AddSingleton<ITimestampsService, TimestampsService>();
            services.AddSingleton<ILocalMediaService, LocalMediaService>();
            services.AddSingleton<IAiredEpisodeNotifier, AiredEpisodeNotifier>();
            services.AddSingleton<IUpdateService, WindowsUpdateService>();
            services.AddSingleton<ITrackingServiceContext, TrackingServiceContext>();
            services.AddSingleton<IAnimeServiceContext, AnimeServiceContext>();
            services.AddSingleton<ISettings, SettingsModel>();
            services.AddSingleton<IPluginManager, PluginManager>();
            //services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IKnownFolders, KnownFolders>();
            services.AddSingleton<IInitializer, Initalizer>();
            services.AddSingleton<ITorrentEngine, TorrentEngine>();

            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IAnimeIdService, AnimeIdService>();
            services.AddTransient<IShanaProjectService, ShanaProjectService>();
            services.AddTransient<TotoroCommands>();
            services.AddTransient<ISystemClock, SystemClock>();
            services.AddTransient<ISchedulerProvider, SchedulerProvider>();
            services.AddTransient<IStreamPageMapper, StreamPageMapper>();
            services.AddTransient<IAnilistService, AnilistService>();
            services.AddTransient<IVideoStreamResolverFactory, VideoStreamResolverFactory>();

            services.AddTransient<IMediaEventListener, MediaSessionStateStorage>();
            services.AddTransient<IMediaEventListener, TrackingUpdater>();
            services.AddTransient<IMediaEventListener, DiscordRichPresenseUpdater>();
            services.AddTransient<IMediaEventListener, Aniskip>();

            services.AddMemoryCache();
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection AddAniList(this IServiceCollection services)
        {
            services.AddTransient<ITrackingService, AniListTrackingService>();
            services.AddTransient<IAnimeService>(x => x.GetRequiredService<IAnilistService>());

            return services;
        }

        public static IServiceCollection AddMyAnimeList(this IServiceCollection services)
        {
            services.AddTransient<ITrackingService, MyAnimeListTrackingService>();
            services.AddTransient<IAnimeService, MyAnimeListService>();
            services.AddSingleton<IMalClient, MalClient>();

            return services;
        }

        public static IServiceCollection AddTorrenting(this IServiceCollection services)
        {
            services.AddTransient<ITorrentCatalog, NyaaCatalog>();
            services.AddTransient<ITorrentCatalog, AnimeToshoCatalog>();
            services.AddSingleton<ITorrentCatalogFactory, TorrentCatalogFactory>();
            
            services.AddTransient<IDebridService, PremiumizeService>();
            services.AddSingleton<IDebridServiceContext, DebridServiceContext>();

            return services;
        }
    }
}
