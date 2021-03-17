using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AutomaticImageFramer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string WAITING_MESSAGE = "Waiting for pasted image...";
        private const string UserSettingsFilename = "UserSettings.xml";

        public string WaitingMessageBindingProperty => WAITING_MESSAGE;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillSettings();
            this.PasteTarget.KeyDown += PasteTarget_KeyDown;
            this.PasteTarget.MouseDown += PasteTarget_MouseDown;
            this.PasteTarget.Focus();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            if (File.Exists(UserSettingsFilename) == false)
            {
                File.Create(UserSettingsFilename).Close();
            }
            var settings = new Settings()
            {
                BorderWidth = this.BorderWidth_textBox.Text
            };
            if (this.ColorPicker.SelectedColor.HasValue)
            {
                settings.Color = this.ColorPicker.SelectedColor.Value;
            }
            settings.Save(UserSettingsFilename);
        }

        private void FillSettings()
        {
            if (File.Exists(UserSettingsFilename))
            {
                var settings = Settings.Read(UserSettingsFilename);
                this.BorderWidth_textBox.Text = settings.BorderWidth;
                this.ColorPicker.SelectedColor = settings.Color;
            }
        }

        private void PasteTarget_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.PasteTarget.Focus();
        }

        private void PasteTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.V)
                {
                    ReplaceCopiedImage();
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReplaceCopiedImage();
        }

        private void ReplaceCopiedImage()
        {
            if (Clipboard.ContainsImage())
            {
                this.LogOutput.Text = "Image received, converting...";
                if (int.TryParse(this.BorderWidth_textBox.Text, out int borderWidth))
                {
                    var converted = ImageHelper.FrameBitmapSource(Clipboard.GetImage(), borderWidth, this.ColorPicker.SelectedColor);
                    Clipboard.SetImage(converted);
                    this.LogOutput.Text = "Image framed and added to clipboard";
                }
                else
                {
                    this.LogOutput.Text = "Invalid border width";
                }
            }
            else
            {
                this.LogOutput.Text = "That's not an image";
            }
            ResetLogMessage();
        }

        private void ResetLogMessage()
        {
            Task.Delay(2000).ContinueWith(x =>
                App.Current.Dispatcher.Invoke(() => this.LogOutput.Text = WAITING_MESSAGE)
            );
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

        }
    }
}
