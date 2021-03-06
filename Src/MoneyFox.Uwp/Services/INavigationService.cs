﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MoneyFox.Uwp.Services
{
    public interface INavigationService
    {
        void Configure(string key, Type pageType);
        string GetNameOfRegisteredPage(Type page);
        bool GoBack();
        bool GoForward();
        bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null);

        bool CanGoBack { get; }
        bool CanGoForward { get; }
        Frame Frame { get; set; }
        event NavigatedEventHandler Navigated;
        event NavigationFailedEventHandler NavigationFailed;
    }
}
