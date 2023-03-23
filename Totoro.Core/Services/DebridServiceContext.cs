﻿using System.Reactive.Subjects;
using Totoro.Core.Services.Debrid;

namespace Totoro.Core.Services;

internal class DebridServiceContext : IDebridServiceContext
{
    private readonly Dictionary<DebridServiceType, IDebridService> _services;
    private readonly ISettings _settings;
    private readonly Subject<string> _onNewTransfer = new();


    public DebridServiceContext(IEnumerable<IDebridService> debridServices,
                                ISettings settings)
    {
        _services = debridServices.ToDictionary(x => x.Type, x => x);
        _settings = settings;
    }

    public bool IsAuthenticated => _services[_settings.DebridServiceType].IsAuthenticated;

    public IObservable<string> TransferCreated => _onNewTransfer;

    public Task<bool> Check(string magneticLink) => _services[_settings.DebridServiceType].Check(magneticLink);

    public async Task<string> CreateTransfer(string magneticLink)
    {
        var id = await _services[_settings.DebridServiceType].CreateTransfer(magneticLink);
        _onNewTransfer.OnNext(id);
        return id;
    }

    public Task<IEnumerable<DirectDownloadLink>> GetDirectDownloadLinks(string magneticLink) => _services[_settings.DebridServiceType].GetDirectDownloadLinks(magneticLink);

    public Task<IEnumerable<Transfer>> GetTransfers() => _services[_settings.DebridServiceType].GetTransfers();
}