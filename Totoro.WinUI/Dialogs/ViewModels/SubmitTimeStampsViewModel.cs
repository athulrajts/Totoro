﻿
using Totoro.WinUI.Media;

namespace Totoro.WinUI.Dialogs.ViewModels;

public class SubmitTimeStampsViewModel : DialogViewModel
{
    private IDisposable _subscription;
    private readonly ITimestampsService _timestampsService;

    public SubmitTimeStampsViewModel(ITimestampsService timestampsService)
    {
        _timestampsService = timestampsService;

        PlayRange = ReactiveCommand.Create(() => Play());
        SetStartPosition = ReactiveCommand.Create(() => StartPosition = MediaPlayer.GetMediaPlayer().Position.TotalSeconds);
        SetEndPosition = ReactiveCommand.Create(() => EndPosition = MediaPlayer.GetMediaPlayer().Position.TotalSeconds);
        SkipNearEnd = ReactiveCommand.Create(() => MediaPlayer.Seek(TimeSpan.FromSeconds(EndPosition - 5)));

        MediaPlayer
            .PositionChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => CurrentPlayerPosition = x.TotalSeconds);

        this.WhenAnyValue(x => x.StartPosition)
            .DistinctUntilChanged()
            .Subscribe(x => MediaPlayer.Seek(TimeSpan.FromSeconds(x)));
    }

    [Reactive] public double StartPosition { get; set; }
    [Reactive] public double EndPosition { get; set; }
    [Reactive] public string SelectedTimeStampType { get; set; } = "OP";
    [Reactive] public double CurrentPlayerPosition { get; set; }

    public string MediaUrl { get; set; }
    public long MalId { get; set; }
    public int Episode { get; set; }
    public double Duration { get; set; }
    public string[] TimeStampTypes = new[] { "OP", "ED" };

    public WinUIMediaPlayerWrapper MediaPlayer { get; } = new WinUIMediaPlayerWrapper();

    public ICommand PlayRange { get; }
    public ICommand SetStartPosition { get; }
    public ICommand SetEndPosition { get; }
    public ICommand SkipNearEnd { get; }

    private void Play()
    {
        _subscription?.Dispose();
        MediaPlayer.Play(StartPosition);

        _subscription = this.WhenAnyValue(x => x.CurrentPlayerPosition)
            .Where(time => time >= EndPosition)
            .Subscribe(_ =>
            {
                _subscription?.Dispose();
                MediaPlayer.Pause();
            });
    }

    public async Task Submit()
    {
        await _timestampsService.SubmitTimeStamp(MalId, Episode, SelectedTimeStampType.ToLower(),  new Interval { StartTime = StartPosition, EndTime = EndPosition }, Duration);
    }

}
