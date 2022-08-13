﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using AnimDL.WinUI.Contracts;
using AnimDL.Api;
using System.Threading.Tasks;

namespace AnimDL.WinUI.ViewModels;

public class SettingsViewModel : NavigatableViewModel, ISettings
{
    [Reactive] public ElementTheme ElementTheme { get; set; }
    [Reactive] public bool PreferSubs { get; set; }
    [Reactive] public ProviderType DefaultProviderType { get; set; }
    [Reactive] public bool IsAuthenticated { get; set; } = true;
    [Reactive] public bool UseDiscordRichPresense { get; set; }
    public List<ElementTheme> Themes { get; set; } = Enum.GetValues<ElementTheme>().Cast<ElementTheme>().ToList();
    public List<ProviderType> ProviderTypes { get; set; } = Enum.GetValues<ProviderType>().Cast<ProviderType>().ToList();
    
    public ICommand AuthenticateCommand { get; }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, 
                             ILocalSettingsService localSettingsService,
                             IViewService viewService,
                             IDiscordRichPresense dRpc)
    {
        ElementTheme = themeSelectorService.Theme;
        PreferSubs = localSettingsService.ReadSetting(nameof(PreferSubs), true);
        DefaultProviderType = localSettingsService.ReadSetting(nameof(DefaultProviderType), ProviderType.AnimixPlay);
        UseDiscordRichPresense = localSettingsService.ReadSetting(nameof(UseDiscordRichPresense), false);
        
        AuthenticateCommand = ReactiveCommand.Create(async () => 
        {
            await viewService.AuthenticateMal();
        });

        if(UseDiscordRichPresense && !dRpc.IsInitialized)
        {
            dRpc.Initialize();
        }

        this.ObservableForProperty(x => x.ElementTheme, x => x)
            .Subscribe(themeSelectorService.SetTheme);
        this.ObservableForProperty(x => x.PreferSubs, x => x)
            .Subscribe(x => localSettingsService.SaveSetting(nameof(PreferSubs), PreferSubs));
        this.ObservableForProperty(x => x.DefaultProviderType, x => x)
            .Subscribe(x => localSettingsService.SaveSetting(nameof(DefaultProviderType), DefaultProviderType));
        this.ObservableForProperty(x => x.UseDiscordRichPresense, x => x)
            .Subscribe(x => 
            {
                localSettingsService.SaveSetting(nameof(UseDiscordRichPresense), UseDiscordRichPresense);
                if(x && !dRpc.IsInitialized)
                {
                    dRpc.Initialize();
                }
            });
    }

    public override Task OnNavigatedTo(IReadOnlyDictionary<string, object> parameters)
    {
        if(parameters.ContainsKey("IsAuthenticated"))
        {
            IsAuthenticated = false;
        }

        return base.OnNavigatedTo(parameters);
    }

    public static string ElementThemeToString(ElementTheme theme) => theme.ToString();
    
}
