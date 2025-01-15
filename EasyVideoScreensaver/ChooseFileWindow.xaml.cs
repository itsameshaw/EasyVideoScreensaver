using EasyVideoScreensaver.Properties;
using Microsoft.Win32;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for ChooseFileWindow.xaml
    /// </summary>
    public partial class ChooseFileWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        public string FileUri { get; private set; } = "";

        public ChooseFileWindow(string fileUri = "")
        {
            InitializeComponent();

            FileUri = fileUri;
            UpdateSelectedFile();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (FileUri.Length == 0)
            {
                ShakeWindow();
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Video File";
            if (string.IsNullOrEmpty(FileUri))
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else
            {
                if (FileUri.StartsWith("file://"))
                {
                    var filePath = FileUri.Substring(7);
                    dialog.FileName = new System.IO.FileInfo(filePath).Name;
                    dialog.InitialDirectory = new System.IO.FileInfo(filePath).DirectoryName;
                }
            }

            dialog.Filter = @"Video Files|*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mov;*.wmv;*.avi;*.mkv;*.mk3d;*.m2ts;*.m2t;*.mts;*.ts;*.tts|MP4 Video Files |*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2|QuickTime Movie Files|*.mov|Windows Video Files|*.wmv;*.avi|MKV Video Files|*.mkv|MK3D video file|*.mk3d|MPEG-2 TS Video Files|*.m2ts;*.m2t;*.mts;*.ts;*.tts|All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                FileUri = $"file://{dialog.FileName}";
                UpdateSelectedFile();
            }
        }

        private void UpdateSelectedFile()
        {
            if (FileUri.StartsWith("file://"))
            {
                LocalFile.Content = FileUri.Substring(7);
                URLFile.Text = "";
            }
            else
            {
                LocalFile.Content = "Local file";
                URLFile.Text = FileUri;
            }
        }

        private void ShakeWindow()
        {
            var originalLeft = this.Left; 
            var animation = new DoubleAnimation { 
                From = originalLeft - 10, 
                To = originalLeft + 10, 
                Duration = TimeSpan.FromMilliseconds(50), 
                AutoReverse = true, 
                RepeatBehavior = new RepeatBehavior(5) 
            }; 
            animation.Completed += (s, e) => { this.Left = originalLeft; }; 
            this.BeginAnimation(Window.LeftProperty, animation);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (textBox.Text.Trim().StartsWith("http"))
            {
                FileUri = textBox.Text;
                UpdateSelectedFile();
            }
        }
    }
}
