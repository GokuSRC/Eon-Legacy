using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace FortniteLauncher.Pages
{
    public sealed partial class MainShellPage : Page
    {
        public static NavigationView STATIC_MainNavigation;

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

        public void UpdateIcons(string Theme)
        {
            string Suffix = Theme == "Light" ? "_B" : string.Empty;

            SetIcon(PlayPageItem, $"ms-appx:///Content/Texture/Icons/IC_Play{Suffix}.png");
            SetIcon(DownloadsItem, $"ms-appx:///Content/Texture/Icons/IC_Download{Suffix}.png");
            SetIcon(ItemShopItem, $"ms-appx:///Content/Texture/Icons/IC_Shop{Suffix}.png");
            SetIcon(LeaderboardItem, $"ms-appx:///Content/Texture/Icons/IC_Leaderboard{Suffix}.png");
            SetIcon(ServerStatusItem, $"ms-appx:///Content/Texture/Icons/IC_ServerStatus{Suffix}.png");
            SetIcon(SettingsItem, $"ms-appx:///Content/Texture/Icons/IC_Settings{Suffix}.png");
        }

        private void SetIcon(NavigationViewItem Item, string IconUri)
        {
            int MenuIndex = MainNavigation.MenuItems.IndexOf(Item);
            int FooterIndex = MainNavigation.FooterMenuItems.IndexOf(Item);

            var ImageIcon = new ImageIcon
            {
                Width = 24,
                Height = 24,
                Margin = new Microsoft.UI.Xaml.Thickness(-4),
                Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(IconUri))
            };

            if (MenuIndex >= 0)
            {
                MainNavigation.MenuItems.RemoveAt(MenuIndex);
                Item.Icon = ImageIcon;
                MainNavigation.MenuItems.Insert(MenuIndex, Item);
            }
            else if (FooterIndex >= 0)
            {
                MainNavigation.FooterMenuItems.RemoveAt(FooterIndex);
                Item.Icon = ImageIcon;
                MainNavigation.FooterMenuItems.Insert(FooterIndex, Item);
            }
        }
        public Frame GetRootFrame() => RootFrame;
    }
}