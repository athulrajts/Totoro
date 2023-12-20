﻿using MonoTorrent;
using RDNET;

namespace Totoro.Core.Services.Debrid;


internal class RealDebridService : IDebridService
{
    private readonly RdNetClient _client = new();
    private bool _useMock = true;

    public RealDebridService()
    {
        _client.UseApiAuthentication("TWGXW4YJCPUVH2VAANZB7WCML5S54O6J7J2CJUBKMKY4K43IFYIA");
    }

    public DebridServiceType Type => DebridServiceType.RealDebrid;

    public bool IsAuthenticated { get; private set; }

    public async Task<bool> Check(string magnetLink)
    {
        var magnet = MagnetLink.Parse(magnetLink);
        try
        {
            _ = await _client.Torrents.GetAvailableFiles(magnet.InfoHash.ToHex());
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async IAsyncEnumerable<bool> Check(IEnumerable<string> magnetLinks)
    {
        foreach (var item in magnetLinks)
        {
            yield return true;
        }
    }

    public Task<string> CreateTransfer(string magnetLink)
    {
        return Task.FromResult(string.Empty);
    }

    public async Task<IEnumerable<DirectDownloadLink>> GetDirectDownloadLinks(string magnetLink)
    {
        if(_useMock)
        {
            await Task.Delay(0);
            IEnumerable<DirectDownloadLink> mock = new List<DirectDownloadLink>()
            {
                new DirectDownloadLink{ Path = "[Erai-raws] Hyakkano - 11 [1080p][Multiple Subtitle][D55CE876].mkv", Link = @"http://mum1.download.real-debrid.com/d/NKBC4XRHRYNTA41/%5BErai-raws%5D%20Hyakkano%20-%2011%20%5B1080p%5D%5BMultiple%20Subtitle%5D%5BD55CE876%5D.mkv"}
            };
            return mock;
        }
        else
        {
            var magnet = MagnetLink.Parse(magnetLink);
            var hash = magnet.InfoHash.ToHex().ToLower();

            var result = await _client.Torrents.GetAvailableFiles(hash);

            if (!result[hash].TryGetValue("rd", out _))
            {
                return Enumerable.Empty<DirectDownloadLink>();
            }

            var magnetResult = await _client.Torrents.AddMagnetAsync(magnetLink);
            var info = await _client.Torrents.GetInfoAsync(magnetResult.Id);
            await _client.Torrents.SelectFilesAsync(info.Id, new[] { "1" });
            info = await _client.Torrents.GetInfoAsync(magnetResult.Id);

            var links = (await GetLinks(info).ToListAsync()).Select(x => new DirectDownloadLink()
            {
                Link = x.Download,
                Path = x.Filename,
            });

            await _client.Torrents.DeleteAsync(info.Id);

            return links;
        }
    }

    private async IAsyncEnumerable<UnrestrictLink> GetLinks(RDNET.Torrent torrent)
    {
        foreach (var link in torrent.Links)
        {
            var unrestrictResult = await _client.Unrestrict.LinkAsync(link);
            yield return unrestrictResult;
        }
    }

    public Task<IEnumerable<Transfer>> GetTransfers()
    {
        return Task.FromResult(Enumerable.Empty<Transfer>());
    }
}
