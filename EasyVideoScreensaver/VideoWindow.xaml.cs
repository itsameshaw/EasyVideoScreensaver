using System;
using System.Collections.Generic;
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

        public static VlcControl GlobalVlcControl;

        public VideoWindow(string[] urls)
        {
            InitializeComponent();
            InitializeVLCIfNeeeded();

            if (VLCValidator.IsValidVLCInstallation(settings.VLCPath))
            {
                VLCErrorLabel.Visibility = Visibility.Hidden;
                // Play the video
            }
            else
            {
                vlcNotFoundAnimation = new DVDAnimation(VLCErrorLabel, this);
            }
        }

        private void Play()
        {
            if (GlobalVlcControl == null) { return; }
            var fileToPlay = settings.Videos.First();
            var uri = new Uri(fileToPlay);
            vlcControl.SourceProvider.MediaPlayer.Play(uri);
        }

        private void PlayNext()
        {

        }

        private void InitializeVLCIfNeeeded()
        {
            if (GlobalVlcControl != null) { return; }
            if (!VLCValidator.IsValidVLCInstallation(settings.VLCPath)) { return; }

            var libDirectory = new DirectoryInfo(settings.VLCPath);

            //var mediaLog = new FileStream("F:\\vlc_log.txt", FileMode.Create); 
            var vlcOptions = new[] { "-vvv", // Verbose mode
                $"--file-logging", // Enable logging to a file
                $"--logfile=F:\\vlc_log.txt" // Specify log file
            };

            vlcControl.SourceProvider.CreatePlayer(libDirectory, vlcOptions);
            vlcControl.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
            vlcControl.SourceProvider.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            vlcControl.SourceProvider.MediaPlayer.Buffering += MediaPlayer_Buffering;
            vlcControl.SourceProvider.MediaPlayer.Audio.Volume = settings.Mute ? 0 : (int)(settings.Volume * 100);
            GlobalVlcControl = vlcControl;
            Play();
        }

        private void MediaPlayer_Buffering(object sender, Vlc.DotNet.Core.VlcMediaPlayerBufferingEventArgs e)
        {
        }

        private void MediaPlayer_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            PlayNext();
        }

        private void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            PlayNext();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Close screensaver when key is pressed
            e.Handled = true;
            CloseScreensaver();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            //Close screensaver when mouse is moved
            e.Handled = true;
            CloseScreensaver();
        }

        private void CloseScreensaver()
        {
            //Save resume position
            if (settings.Resume)
            {
                settings.ResumePosition = vlcControl.SourceProvider?.MediaPlayer?.Position ?? 0;
                settings.Save(settingsFilename);
            }

            //Close screensaver
            Application.Current.Shutdown();
        }

    }
}
