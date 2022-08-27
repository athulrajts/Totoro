﻿using Microsoft.UI.Xaml.Controls;

namespace AnimDL.WinUI.Contracts;

public interface IContentDialogService
{
    Task<ContentDialogResult> ShowDialog<TViewModel>(Action<ContentDialog> configure) where TViewModel : class;
    Task<ContentDialogResult> ShowDialog<TViewModel>(TViewModel viewModel, Action<ContentDialog> configure) where TViewModel : class;
}