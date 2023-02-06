﻿using Microsoft.UI.Xaml;
using Splat;
using Totoro.WinUI.Activation;

namespace Totoro.WinUI.Services;

public class ActivationService : IActivationService, IEnableLogger
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IPlaybackStateStorage _playbackStateStorage;
    private readonly IPluginManager _pluginManager;
    private readonly IUpdateService _updateService;
    private readonly string _prevWebviewFolder;
    private readonly string _tempPath = System.IO.Path.GetTempPath();

    public bool IsAuthenticated { get; set; } = true;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
                             IEnumerable<IActivationHandler> activationHandlers,
                             IThemeSelectorService themeSelectorService,
                             IPlaybackStateStorage playbackStateStorage,
                             ITrackingServiceContext trackingService,
                             IPluginManager pluginManager,
                             IUpdateService updateService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _playbackStateStorage = playbackStateStorage;
        _pluginManager = pluginManager;
        _updateService = updateService;
        IsAuthenticated = trackingService.IsAuthenticated;
        _prevWebviewFolder = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER");
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", _tempPath);
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();


        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        App.MainWindow.Closed += MainWindow_Closed;

        // Execute tasks after activation.
        await StartupAsync();
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        this.Log().Info("Closing MainWindow");
        if (!string.IsNullOrEmpty(_prevWebviewFolder) && _prevWebviewFolder != _tempPath)
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", _prevWebviewFolder);
        }
        _playbackStateStorage.StoreState();
        _updateService.ShutDown();
        App.MainWindow.Closed -= MainWindow_Closed;
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        _themeSelectorService.Initialize();
        await _pluginManager.Initialize();
    }

    private Task StartupAsync()
    {
        _themeSelectorService.SetRequestedTheme();
        return Task.CompletedTask;
    }
}
