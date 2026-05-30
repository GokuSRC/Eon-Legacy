using System.Linq;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using FortniteLauncher.Pages;
using WinUIEx;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace FortniteLauncher
{
    public sealed partial class MainWindow : WindowEx
    {
        public string LauncherName { get; } = $"{ProjectDefinitions.Name} ";
        public static Frame ShellFrame { get; private set; }

        private static readonly List<VirtualKey> KonamiSequence = new()
        {
            VirtualKey.Up, VirtualKey.Up,
            VirtualKey.Down, VirtualKey.Down,
            VirtualKey.Left, VirtualKey.Right,
            VirtualKey.Left, VirtualKey.Right,
            VirtualKey.B, VirtualKey.A
        };

        private readonly List<VirtualKey> _inputBuffer = new();

        public MainWindow()
        {
            InitializeComponent();
            ConfigureWindow();
            ConfigureBackdrop();
            InitializeNavigation();
            this.Activated += (s, e) => RootGrid.Focus(FocusState.Programmatic);
            RootGrid.PreviewKeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Key pressed: {e.Key}");

            _inputBuffer.Add(e.Key);

            if (_inputBuffer.Count > KonamiSequence.Count)
                _inputBuffer.RemoveAt(0);

            System.Diagnostics.Debug.WriteLine($"Buffer: {string.Join(", ", _inputBuffer)}");

            if (_inputBuffer.Count == KonamiSequence.Count &&
                _inputBuffer.SequenceEqual(KonamiSequence))
            {
                _inputBuffer.Clear();
                OnKonamiActivated();
            }
        }

        private void OnKonamiActivated()
        {
            DialogService.ShowSimpleDialog("you found the secret :)", "Konami Code");
        }

        private void ConfigureWindow()
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            this.SetWindowSize(1200, 725);
            this.CenterOnScreen();
            this.SetIcon("Content\\Texture\\Branding\\EonPlus.ico");
            Title = LauncherName;
        }

        private void ConfigureBackdrop()
        {
            SystemBackdrop = new MicaBackdrop
            {
                Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
            };
        }

        private void InitializeNavigation()
        {
            RootGrid.Focus(FocusState.Programmatic);
            ShellFrame = MainWindowFrame;
            MainWindowFrame.Navigate(typeof(LoginPage));
        }
    }
}