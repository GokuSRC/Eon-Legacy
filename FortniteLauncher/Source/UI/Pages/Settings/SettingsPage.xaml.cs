using System;
using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FortniteLauncher.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly string LauncherVersion = GlobalSettings.Version;

        private readonly string About_Header = $"About {ProjectDefinitions.Name} Launcher";
        private readonly string GitHub_Juri = ProjectDefinitions.GitHub_Juri;
        private readonly string Github_Greenwood = ProjectDefinitions.GitHub_Greenwood;
        private readonly string Discord = ProjectDefinitions.Discord;
        private readonly string Tiktok = ProjectDefinitions.Tiktok;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object Sender, RoutedEventArgs Event)
        {
            SoundToggle.IsOn = GlobalSettings.Options.IsSoundEnabled;
            BubbleBuildsToggle.IsOn = GlobalSettings.Options.IsBubbleBuildsEnabled;
            ThemeSelector.SelectedItem = ThemeSelector.Items
              .Cast<ComboBoxItem>()
              .FirstOrDefault(i => i.Content.ToString() == GlobalSettings.Options.Theme);
        }

        private void ToggleSoundSwitch(object Sender, RoutedEventArgs Event)
        {
            var ToggleSwitch = (ToggleSwitch)Sender;
            if (ToggleSwitch.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                GlobalSettings.Options.IsSoundEnabled = true;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                GlobalSettings.Options.IsSoundEnabled = false;
            }

            UserSettings.SaveSettings();
        }
        private void ToggleBubbleBuilds(object Sender, RoutedEventArgs Event)
        {
            if (BubbleBuildsToggle.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                GlobalSettings.Options.IsBubbleBuildsEnabled = true;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                GlobalSettings.Options.IsBubbleBuildsEnabled = false;
            }

            UserSettings.SaveSettings();
        }

        private async void SignOutAccount(object Sender, RoutedEventArgs Event)
        {
            bool Result = await DialogService.YesOrNoDialog("This action will sign you out of your account. Your Account Information will be cleared from this device. Are you sure you want to proceed?", "Logging Out");

            if (Result)
            {
                GlobalSettings.Options.Email = string.Empty;
                GlobalSettings.Options.Password = string.Empty;
                UserSettings.SaveSettings();

                var Button = (Button)Sender;
                Button.IsEnabled = false;

                var ProgressRing = new ProgressRing
                {
                    IsIndeterminate = true,
                    Foreground = new SolidColorBrush(Colors.White)
                };

                Button.Content = ProgressRing;

                MainWindow.ShellFrame.Navigate(typeof(LoginPage));
                Button.IsEnabled = true;
            }
        }
        private void ThemeChanged(object Sender, SelectionChangedEventArgs Event)
        {
            if (ThemeSelector.SelectedItem is ComboBoxItem Selected)
            {
                var Theme = Selected.Content.ToString();
                GlobalSettings.Options.Theme = Theme;
                UserSettings.SaveSettings();
                ApplyTheme(Theme);
            }
        }

        public static void ApplyTheme(string Theme)
        {
            Brush Brush;
            var RequestedTheme = Theme == "Light" ? ElementTheme.Light : ElementTheme.Dark;

            if (Theme == "KittyParty")
            {
                Brush = new ImageBrush
                {
                    ImageSource = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri("https://media.discordapp.net/attachments/1509928333264031826/1510659568009744525/image.png?ex=6a1d9edb&is=6a1c4d5b&hm=4aebcaa1ee9ff6a9a1e59094e0db12c0434f3d3b95f0f6e784ee0221d1c7ea7d&format=webp&quality=lossless&width=912&height=959&")),
                    Stretch = Stretch.UniformToFill
                };
                RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                var Color = Theme switch
                {
                    "Light" => Windows.UI.Color.FromArgb(255, 255, 255, 255),
                    "Dark" => Windows.UI.Color.FromArgb(255, 13, 17, 23),
                    _ => Windows.UI.Color.FromArgb(255, 32, 35, 54)
                };
                Brush = new SolidColorBrush(Color);
            }

            if (GlobalSettings.Windows?.Content is FrameworkElement Root)
                Root.RequestedTheme = RequestedTheme;

            if (GlobalSettings.Windows is MainWindow MainWin)
                MainWin.SetWindowBackground(Brush);

            if (MainWindow.ShellFrame?.Content is MainShellPage Shell)
                Shell.SetBackground(Brush);
        }
    }
}