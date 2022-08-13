﻿using AnimDL.WinUI.ViewModels;
using ReactiveUI;

namespace AnimDL.WinUI.Views;

public class DiscoverPageBase : ReactivePage<DiscoverViewModel> { }
public sealed partial class DiscoverPage : DiscoverPageBase
{
    public DiscoverPage()
    {
        InitializeComponent();
    }
}
