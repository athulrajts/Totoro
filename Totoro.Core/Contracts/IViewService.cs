﻿using AnimDL.Api;

namespace Totoro.Core.Contracts;

public interface IViewService
{
    Task<Unit> UpdateAnimeStatus(IAnimeModel anime);
    Task<SearchResult> ChoooseSearchResult(List<SearchResult> searchResults, ProviderType providerType);
    Task AuthenticateMal();
    Task PlayVideo(string title, string url);
    Task<T> SelectModel<T>(IEnumerable<object> models) where T : class;
}
