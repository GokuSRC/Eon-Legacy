using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System.Text.RegularExpressions;

class DialogService
{
    private const int IconSize = 40;
    private const int IconRadius = 20;
    private const int TitleFontSize = 20;
    private const int ContentFontSize = 14;
    private const int ContentLineHeight = 22;
    private const int ContentMaxWidth = 500;
    private const int ContentMaxHeight = 550;
    private const int DialogCornerRadius = 16;
    private const int ButtonCornerRadius = 8;

    public enum DialogIcon { Error, Info }

    public static async Task ShowSimpleDialog(string Content, string Title, DialogIcon Icon = DialogIcon.Error)
    {
        if (!ValidateWindow())
            return;

        await Processes.ForceCloseFortnite(true);

        string ProcessedContent = ProcessCustomErrors(Content, Title);
        ContentDialog Dialog = CreateDialog(Title, ProcessedContent, false, Icon);
        await Dialog.ShowAsync();
    }

    public static async Task<bool> YesOrNoDialog(string Content, string Title, DialogIcon Icon = DialogIcon.Error)
    {
        if (!ValidateWindow())
            return false;

        await Processes.ForceCloseFortnite(true);

        ContentDialog Dialog = CreateDialog(Title, Content, true, Icon);
        ContentDialogResult Result = await Dialog.ShowAsync();

        return Result == ContentDialogResult.Primary;
    }

    private static bool ValidateWindow()
    {
        return GlobalSettings.Windows?.Content?.XamlRoot != null;
    }

    private static ContentDialog CreateDialog(string Title, string Content, bool IsYesNo, DialogIcon Icon)
    {
        return new ContentDialog
        {
            XamlRoot = GlobalSettings.Windows.Content.XamlRoot,
            Title = CreateTitlePanel(Title, Icon),
            Content = CreateContentBlock(Content),
            PrimaryButtonText = IsYesNo ? "Yes" : null,
            CloseButtonText = IsYesNo ? "No" : "OK",
            DefaultButton = ContentDialogButton.Primary,
            Background = GlobalSettings.Options.Theme switch
            {
                "Light" => CreateSolidBrush(255, 255, 255),
                "Dark" => CreateSolidBrush(13, 17, 23),
                _ => CreateSolidBrush(30, 30, 40)
            },
            BorderBrush = GlobalSettings.Options.Theme switch
            {
                "Light" => CreateSolidBrush(200, 200, 200),
                "Dark" => CreateSolidBrush(30, 30, 30),
                _ => CreateSolidBrush(60, 60, 70)
            },
            CornerRadius = new CornerRadius(DialogCornerRadius),
            PrimaryButtonStyle = GlobalSettings.Options.Theme == "Light"
                ? CreateButtonStyle(200, 200, 200, 0, 0, 0, 150, 150, 150)
                : CreateButtonStyle(45, 45, 55, 200, 200, 200, 60, 60, 70),
            CloseButtonStyle = GlobalSettings.Options.Theme == "Light"
                ? CreateButtonStyle(220, 220, 220, 0, 0, 0, 180, 180, 180)
                : CreateButtonStyle(35, 35, 45, 160, 160, 170, 50, 50, 60)
        };
    }

    private static StackPanel CreateTitlePanel(string Title, DialogIcon Icon)
    {
        StackPanel Panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        Panel.Children.Add(CreateIconGrid(Icon));
        Panel.Children.Add(CreateTitleTextBlock(Title));

        return Panel;
    }

    private static Grid CreateIconGrid(DialogIcon Icon)
    {
        bool IsInfo = Icon == DialogIcon.Info;

        Grid IconGrid = new Grid
        {
            Width = IconSize,
            Height = IconSize,
            CornerRadius = new CornerRadius(IconRadius),
            Background = IsInfo ? CreateSolidBrush(59, 130, 246) : CreateSolidBrush(239, 68, 68)
        };

        IconGrid.Children.Add(new TextBlock
        {
            Text = IsInfo ? "i" : "!",
            FontSize = 24,
            FontWeight = new Windows.UI.Text.FontWeight { Weight = 700 },
            Foreground = CreateSolidBrush(255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        return IconGrid;
    }

    private static TextBlock CreateTitleTextBlock(string Title)
    {
        return new TextBlock
        {
            Text = Title,
            FontSize = TitleFontSize,
            FontWeight = new Windows.UI.Text.FontWeight { Weight = 700 },
            Foreground = GlobalSettings.Options.Theme == "Light"
                ? CreateSolidBrush(0, 0, 0)
                : CreateSolidBrush(255, 255, 255),
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private static ScrollViewer CreateContentBlock(string Content)
    {
        RichTextBlock Block = new RichTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = ContentFontSize,
            Foreground = GlobalSettings.Options.Theme == "Light"
                ? CreateSolidBrush(0, 0, 0)
                : CreateSolidBrush(200, 200, 200),
            LineHeight = ContentLineHeight,
            MaxWidth = ContentMaxWidth
        };

        Paragraph Paragraph = new Paragraph();
        AddContentWithHyperlinks(Content, Paragraph);
        Block.Blocks.Add(Paragraph);

        return new ScrollViewer
        {
            Content = Block,
            MaxHeight = ContentMaxHeight,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
    }

    private static void AddContentWithHyperlinks(string Content, Paragraph Paragraph)
    {
        Regex UrlRegex = new Regex(@"https?://[^\s,]+", RegexOptions.IgnoreCase);
        MatchCollection Matches = UrlRegex.Matches(Content);

        int LastIndex = 0;

        foreach (Match Match in Matches)
        {
            if (Match.Index > LastIndex)
                Paragraph.Inlines.Add(new Run { Text = Content.Substring(LastIndex, Match.Index - LastIndex) });

            Hyperlink Link = new Hyperlink
            {
                NavigateUri = new Uri(Match.Value),
                Foreground = CreateSolidBrush(100, 150, 255),
                FontWeight = new Windows.UI.Text.FontWeight { Weight = 600 }
            };
            Link.Inlines.Add(new Run { Text = Match.Value });
            Paragraph.Inlines.Add(Link);

            LastIndex = Match.Index + Match.Length;
        }

        if (LastIndex < Content.Length)
            Paragraph.Inlines.Add(new Run { Text = Content.Substring(LastIndex) });
    }

    private static Style CreateButtonStyle(byte BackR, byte BackG, byte BackB, byte ForeR, byte ForeG, byte ForeB, byte BorderR, byte BorderG, byte BorderB)
    {
        var BaseStyle = Application.Current.Resources["DialogButtonStyle"] as Style;
        Style ButtonStyle = new Style(typeof(Button)) { BasedOn = BaseStyle };

        ButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, CreateSolidBrush(BackR, BackG, BackB)));
        ButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, CreateSolidBrush(ForeR, ForeG, ForeB)));
        ButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, CreateSolidBrush(BorderR, BorderG, BorderB)));

        return ButtonStyle;
    }

    private static SolidColorBrush CreateSolidBrush(byte R, byte G, byte B)
    {
        return new SolidColorBrush(Color.FromArgb(255, R, G, B));
    }

    private static string ProcessCustomErrors(string Content, string Title)
    {
        if (Content.Contains("EasyAntiCheat"))
            return $"Easy Anti-Cheat needs to be reinstalled. Go to {GlobalSettings.Options.FortnitePath} and delete the EasyAntiCheat folder and Eon_EAC.exe file, then restart the launcher.";

        if (Content.Contains("because it is being used by another process"))
            return "Fortnite is already running. Close it and try again. If the issue persists, restart your computer.";

        if (Title.Contains("Corrupted Data Detected"))
        {
            var Message = $"Your game files are corrupted. Download the Fortnite build from {ProjectDefinitions.DownloadBuildURL}, extract it, and set the path in the launcher.";
            if (!string.IsNullOrEmpty(Content))
                Message += $"\n\n{Content}";
            return Message;
        }

        if (Content.Contains("SSL"))
            return "Connection issue detected. Download and enable CloudFlare WARP from https://one.one.one.one/ to continue.";

        return Content;
    }

    public static async Task ShowPlayerListDialog(System.Collections.Generic.List<string> Players, string Title)
    {
        if (!ValidateWindow())
            return;

        var Stack = new StackPanel { Spacing = 6, Width = 460 };

        foreach (var Player in Players)
        {
            System.Timers.Timer GlowTimer = null;
            double GlowOffset = 0;

            var Card = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 8, 12, 8),
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0)),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(40, 255, 255, 255))
            };

            Card.Child = new TextBlock
            {
                Text = Player,
                FontSize = 14,
                FontWeight = new Windows.UI.Text.FontWeight { Weight = 600 },
                Foreground = GlobalSettings.Options.Theme == "Light"
                    ? CreateSolidBrush(0, 0, 0)
                    : CreateSolidBrush(255, 255, 255)
            };

            Card.PointerEntered += (s, e) =>
            {
                var B = (Border)s;
                B.BorderThickness = new Thickness(2);
                B.Translation = new System.Numerics.Vector3(0, -2, 0);

                GlowTimer = new System.Timers.Timer(16);
                GlowTimer.Elapsed += (ts, te) =>
                {
                    GlowOffset = (GlowOffset + 0.035) % (Math.PI * 2);
                    B.DispatcherQueue.TryEnqueue(() =>
                    {
                        double T = (Math.Sin(GlowOffset) + 1) / 2;

                        byte GreenR = (byte)(0 + T * 150);
                        byte GreenG = (byte)(200 - T * 200);
                        byte GreenB = (byte)(100 + T * 155);

                        byte PurpleR = (byte)(150 - T * 150);
                        byte PurpleG = (byte)(0);
                        byte PurpleB = (byte)(255 - T * 155);

                        B.BorderBrush = new Microsoft.UI.Xaml.Media.LinearGradientBrush
                        {
                            StartPoint = new Windows.Foundation.Point(0, 0),
                            EndPoint = new Windows.Foundation.Point(1, 0),
                            GradientStops = new Microsoft.UI.Xaml.Media.GradientStopCollection
            {
                new Microsoft.UI.Xaml.Media.GradientStop { Color = Windows.UI.Color.FromArgb(255, GreenR, GreenG, GreenB), Offset = 0 },
                new Microsoft.UI.Xaml.Media.GradientStop { Color = Windows.UI.Color.FromArgb(255, PurpleR, PurpleG, PurpleB), Offset = 1 }
            }
                        };
                    });
                };
                GlowTimer.AutoReset = true;
                GlowTimer.Start();
            };

            Card.PointerExited += (s, e) =>
            {
                var B = (Border)s;
                GlowTimer?.Stop();
                GlowTimer = null;
                GlowOffset = 0;
                B.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(40, 255, 255, 255));
                B.BorderThickness = new Thickness(1);
                B.Translation = new System.Numerics.Vector3(0, 0, 0);
            };

            Stack.Children.Add(Card);
        }

        var ScrollViewer = new ScrollViewer
        {
            Content = Stack,
            MaxHeight = 550,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        var Dialog = new ContentDialog
        {
            XamlRoot = GlobalSettings.Windows.Content.XamlRoot,
            Title = CreateTitlePanel(Title, DialogIcon.Info),
            Content = ScrollViewer,
            CloseButtonText = "OK",
            Background = GlobalSettings.Options.Theme switch
            {
                "Light" => CreateSolidBrush(255, 255, 255),
                "Dark" => CreateSolidBrush(13, 17, 23),
                _ => CreateSolidBrush(30, 30, 40)
            },
            BorderBrush = GlobalSettings.Options.Theme switch
            {
                "Light" => CreateSolidBrush(200, 200, 200),
                "Dark" => CreateSolidBrush(30, 30, 30),
                _ => CreateSolidBrush(60, 60, 70)
            },
            CornerRadius = new CornerRadius(DialogCornerRadius),
            CloseButtonStyle = GlobalSettings.Options.Theme == "Light"
                ? CreateButtonStyle(220, 220, 220, 0, 0, 0, 180, 180, 180)
                : CreateButtonStyle(35, 35, 45, 160, 160, 170, 50, 50, 60)
        };

        await Dialog.ShowAsync();
    }
}