using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Wpf;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for BlackoutWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        private string settingsFilename = ((App)Application.Current).settingsFilename;
        private DVDAnimation vlcNotFoundAnimation;
        private bool firstStart = true;

        public VideoWindow(PrimaryVideoWindow primaryWindow)
        {
            InitializeComponent();

            if (VLCValidator.IsValidVLCInstallation(settings.VLCPath))
            {
                VLCErrorLabel.Visibility = Visibility.Hidden;
                primaryWindow.VLCControlCreated += PrimaryWindow_VLCControlCreated;
            }
            else
            {
                vlcNotFoundAnimation = new DVDAnimation(VLCErrorLabel, this);
            }

        }

        private void PrimaryWindow_VLCControlCreated(object sender, VlcControl e)
        {
            SetVisualBrushes(e);
        }

        private void SetVisualBrushes(VlcControl vlcControl)
        {
            BackgroundCanvas.Width = this.Width;
            BackgroundCanvas.Height = this.Height;

            BackgroundRectangle.Width = BackgroundCanvas.Width; 
            BackgroundRectangle.Height = BackgroundCanvas.Height;
            
            var visualBrush = new VisualBrush(vlcControl) { 
                Stretch = Stretch.UniformToFill 
            }; 
            BackgroundRectangle.Fill = visualBrush; 
            ForegroundRectangle.Fill = visualBrush;
            CenterAndSizeForeground(vlcControl);

            if (firstStart)
            {
                firstStart = false;
                this.SizeChanged += delegate
                {
                    SetVisualBrushes(vlcControl);
                };
                vlcControl.SourceProvider.MediaPlayer.MediaChanged += delegate
                {
                    CenterAndSizeForeground(vlcControl);
                };
            }
        }

        private void CenterAndSizeForeground(VlcControl vlcControl)
        {
            double videoWidth = -1;
            double videoHeight = -1;

            var mediaPlayer = vlcControl.SourceProvider.MediaPlayer;
            var tracks = mediaPlayer.GetMedia().TracksInformations.ToList();
            var videoTracks = tracks.FindAll(track => track.Type == MediaTrackTypes.Video);
            if (videoTracks.Count > 0)
            {
                var videoTrack = videoTracks.First();
                videoWidth = videoTrack.Video.Width;
                videoHeight = videoTrack.Video.Height;
            }

            if (videoHeight <= 0 || videoWidth <= 0 || ActualHeight <= 0 || ActualHeight <= 0)
            {
                Trace.WriteLine("Wrong video size, hopefully will be fixed in the next call");
                return;
            }

            double windowAspect = ActualWidth / ActualHeight;
            double videoAspect = videoWidth / videoHeight;

            if (windowAspect > videoAspect)
            {
                // Window is wider than video
                double newHeight = ActualHeight;
                double newWidth = newHeight * videoAspect;
                ForegroundRectangle.Width = newWidth;
                ForegroundRectangle.Height = newHeight;
            }
            else
            {
                // Window is taller than video
                double newWidth = ActualWidth;
                double newHeight = newWidth / videoAspect;
                ForegroundRectangle.Width = newWidth;
                ForegroundRectangle.Height = newHeight;
            }

            // Center the ForegroundRectangle
            Canvas.SetLeft(ForegroundRectangle, (ActualWidth - ForegroundRectangle.Width) / 2);
            Canvas.SetTop(ForegroundRectangle, (ActualHeight - ForegroundRectangle.Height) / 2);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            CloseScreensaver();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            CloseScreensaver();
        }

        private void CloseScreensaver()
        {
            //Close screensaver
            Application.Current.Shutdown();
        }

    }
}
