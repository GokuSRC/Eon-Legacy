using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace FortniteLauncher.Pages
{
    public sealed partial class MainShellPage : Page
    {
        public static NavigationView STATIC_MainNavigation;

        // Properties needed for x:Bind data bindings in your XAML layout
        public string Progress { get; set; } = "0%";
        public string DisplayUsername { get; set; } = "Player";
        public string Description { get; set; } = "Welcome back!";

        public void SetBackground(Brush Brush)
        {
            MainNavigation.Background = Brush;
        }

        public MainShellPage()
        {
            this.InitializeComponent();
            NavigationService.InitializeNavigationService(MainNavigation, MainBreadcrumb, RootFrame);
            MainNavigation.SelectedItem = PlayPageItem;
        }

        private void MainNavigation_SelectionChanged(NavigationView Sender, NavigationViewSelectionChangedEventArgs Args)
        {
            if ((Args.SelectedItem as NavigationViewItem) == PlayPageItem) { NavigationService.Navigate(typeof(PlayPage), true); NavigationService.ChangeBreadcrumbVisibility(false); }
            if ((Args.SelectedItem as NavigationViewItem) == DownloadsItem) { NavigationService.Navigate(typeof(DownloadsPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == ItemShopItem) { NavigationService.Navigate(typeof(ItemShopPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == LeaderboardItem) { NavigationService.Navigate(typeof(LeaderboardPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == ServerStatusItem) { NavigationService.Navigate(typeof(ServerStatusPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == SettingsItem) { NavigationService.Navigate(typeof(SettingsPage), true); }
            ElementSoundPlayer.Play(ElementSoundKind.Invoke);
        }

        private void MainBreadcrumb_ItemClicked(BreadcrumbBar Sender, BreadcrumbBarItemClickedEventArgs Args)
        {
            if (Args.Index < NavigationService.BreadCrumbs.Count - 1)
            {
                var Crumb = (NavigationService.Breadcrumb)Args.Item;
                Crumb.NavigateToFromBreadcrumb(Args.Index);
            }
        }

        private void MainNavigation_Loaded(object Sender, RoutedEventArgs Event)
        {
            STATIC_MainNavigation = MainNavigation;
            SettingsPage.ApplyTheme(GlobalSettings.Options.Theme ?? "Default");
        }

        // Action Handlers for your XAML interactive cards and buttons
        private void Donations(object sender, RoutedEventArgs e)
        {
            // Add your donation link handling code logic here
        }

        private void Tiktok(object sender, RoutedEventArgs e)
        {
            // Add your TikTok link handling code logic here
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            // Add your game launching process initialization logic here
        }
    }
}