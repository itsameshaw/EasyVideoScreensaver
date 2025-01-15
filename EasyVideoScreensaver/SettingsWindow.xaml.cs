using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        private string settingsFilename = ((App)Application.Current).settingsFilename;

        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = settings;

            //Load settings
            VideoFilenameTextBox.Text = settings.VideoFilename;
            StretchModeComboBox.ItemsSource = new List<string> { "Fit", "Fill", "Center" };
            StretchModeComboBox.SelectedValue = settings.StretchMode;
            VideoPlayMode.ItemsSource = new List<string> { "In Order", "Random" };
            VideoPlayMode.SelectedValue = settings.VideoPlayMode;
            VolumeSlider.Value = settings.Volume;
            MuteCheckBox.IsChecked = settings.Mute;
            ResumeCheckBox.IsChecked = settings.Resume;

            Videos.ItemsSource = settings.Videos;
            VLCValidate();

            //Set initial focus
            VideoFilenameTextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Validate video file exists
            if (!string.IsNullOrEmpty(VideoFilenameTextBox.Text) && !System.IO.File.Exists(VideoFilenameTextBox.Text))
            {
                MessageBox.Show("The specified file does not exist.", "File Does Not Exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            //Save settings
            if (settings.VideoFilename != VideoFilenameTextBox.Text)
            {
                //If video filename has changed, then reset resume position
                settings.ResumePosition = 0;
            }
            settings.VideoFilename = VideoFilenameTextBox.Text;
            settings.StretchMode = (string)StretchModeComboBox.SelectedValue;
            settings.Volume = VolumeSlider.Value;
            settings.Mute = MuteCheckBox.IsChecked == true;
            settings.Resume = ResumeCheckBox.IsChecked == true;
            settings.Save(settingsFilename);

            //Close window
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Close window
            this.Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //Show open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Video File";
            if (string.IsNullOrEmpty(settings.VideoFilename))
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else
            {
                dialog.FileName = settings.VideoFilename;
                dialog.InitialDirectory = new System.IO.FileInfo(settings.VideoFilename).DirectoryName;
            }

            dialog.Filter = @"Video Files|*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mov;*.wmv;*.avi;*.mkv;*.mk3d;*.m2ts;*.m2t;*.mts;*.ts;*.tts|MP4 Video Files |*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2|QuickTime Movie Files|*.mov|Windows Video Files|*.wmv;*.avi|MKV Video Files|*.mkv|MK3D video file|*.mk3d|MPEG-2 TS Video Files|*.m2ts;*.m2t;*.mts;*.ts;*.tts|All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                VideoFilenameTextBox.Text = dialog.FileName;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeValueLabel.Content = VolumeSlider.Value.ToString("P0");
        }

        private void MuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = false;
        }

        private void MuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = true;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ShowScreensaver();
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (Videos.SelectedIndex == -1) { return; }
            ChooseFileWindow fileWindow = new ChooseFileWindow(Videos.SelectedItem as string);
            fileWindow.Owner = this;
            if (fileWindow.ShowDialog() == true)
            {
                settings.Videos[Videos.SelectedIndex] = fileWindow.FileUri;
                Videos.Reload();
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (Videos.SelectedIndex == -1) { return; }
            settings.Videos.RemoveAt(Videos.SelectedIndex);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            ChooseFileWindow fileWindow = new ChooseFileWindow();
            fileWindow.Owner = this;
            if (fileWindow.ShowDialog() == true)
            {
                settings.Videos.Add(fileWindow.FileUri);
            }
        }

        private void VLC_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select VLC Library file";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            dialog.Filter = @"VLC Library|libvlc.dll;";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                settings.VLCPath = new System.IO.FileInfo(dialog.FileName).DirectoryName;
                VLCValidate();
            }
        }

        private void VLCValidate()
        {
            if (VLCValidator.IsValidVLCInstallation(settings.VLCPath))
            {
                VLCValid.Content = "✔";
                VLCValid.Foreground = new SolidColorBrush(Colors.DarkGreen);
            } else
            {
                VLCValid.Content = "❌";
                VLCValid.Foreground = new SolidColorBrush(Colors.DarkRed);
            }
        }
    }

    public static class DirectoryInfoExtension
    {
        public static bool ContainsFile(this System.IO.DirectoryInfo directoryInfo, string path)
        {
            var filePath = System.IO.Path.Combine(directoryInfo.FullName, path);
            var fileInfo = new System.IO.FileInfo(filePath);
            return fileInfo.Exists;
        }
    }

    public static class ListBoxExtension
    {
        public static void Reload(this ListBox instance)
        {
            var previousSource = instance.ItemsSource;
            instance.ItemsSource = null;
            instance.ItemsSource = previousSource;
        }
    }
}
