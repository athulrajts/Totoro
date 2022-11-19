﻿using System.ComponentModel;

namespace Totoro.Core.Models;

public class Inpc : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}

public interface IAnimeModel
{
    public string Title { get; set; }
    public long Id { get; set; }
    public int? TotalEpisodes { get; set; }
    public Tracking Tracking { get; set; }
}

public class AnimeModel : Inpc, IAnimeModel
{
    public long Id { get; set; }
    public string Image { get; set; }
    public string Title { get; set; }
    public Tracking Tracking { get; set; }
    public int? TotalEpisodes { get; set; }
    public AiringStatus AiringStatus { get; set; }
    public float? MeanScore { get; set; }
    public int Popularity { get; set; }
    public List<string> AlternativeTitles { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return Title;
    }
}

public class SearchResultModel : IAnimeModel
{
    public string Title { get; set; }
    public long Id { get; set; }
    public Tracking Tracking { get; set; }
    public int? TotalEpisodes { get; set; }
    public override string ToString() => Title;
    public List<string> AlternativeTitles { get; set; }
}

public class ScheduledAnimeModel : AnimeModel
{
    public DayOfWeek? BroadcastDay { get; set; }
    public TimeRemaining TimeRemaining { get; set; }
}

public class FullAnimeModel : SeasonalAnimeModel
{
    public string[] Genres { get; set; }

    public AnimeModel[] Related { get; set; }
}

public class SeasonalAnimeModel : ScheduledAnimeModel
{
    public Season Season { get; set; }
}

public class TimeRemaining : Inpc
{
    public TimeSpan TimeSpan { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public TimeRemaining(TimeSpan timeSpan, DateTime lastUpdatedAt)
    {
        TimeSpan = timeSpan;
        LastUpdatedAt = lastUpdatedAt;
    }
}
