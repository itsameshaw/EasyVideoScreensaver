using EasyVideoScreensaver.Properties;
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
using Vlc.DotNet.Wpf;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for PrimaryVideoWindow.xaml
    /// </summary>
    public partial class PrimaryVideoWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        private string settingsFilename = ((App)Application.Current).settingsFilename;
        private string[] Videos = new string[0];
        private int CurrentVideoIndex = 0;
        private VlcControl SharedVLCControl { get; set; }
        private EventHandler<VlcControl> VLCCreated = delegate { };
        public event EventHandler<VlcControl> VLCControlCreated {
            add { 
                VLCCreated += value;
                if (SharedVLCControl != null)
                {
                    value.Invoke(this, SharedVLCControl);
                }
            } 
            remove { VLCCreated -= value; }
        }

        public PrimaryVideoWindow(string[] urls)
        {
            InitializeComponent();

            Videos = urls;
            InitializeVLCIfNeeeded();

            Closing += PrimaryVideoWindow_Closing;

            Hide();
        }

        private void PrimaryVideoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Save resume position
            if (settings.Resume)
            {
                settings.ResumePosition = vlcControl.SourceProvider?.MediaPlayer?.Position ?? 0;
                settings.Save(settingsFilename);
            }
        }

        private void Play()
        {
            var fileToPlay = Videos.First();
            var uri = new Uri(fileToPlay);
            vlcControl.SourceProvider.MediaPlayer.Play(uri);
        }

        private void PlayNext()
        {

        }

        private void InitializeVLCIfNeeeded()
        {
            if (!VLCValidator.IsValidVLCInstallation(settings.VLCPath)) { return; }

            var libDirectory = new DirectoryInfo(settings.VLCPath);

            //var mediaLog = new FileStream("F:\\vlc_log.txt", FileMode.Create); 
            var vlcOptions = new[] { "-vvv", // Verbose mode
                $"--file-logging", // Enable logging to a file
                $"--logfile=F:\\vlc_log.txt" // Specify log file
            };

            vlcControl.SourceProvider.CreatePlayer(libDirectory, vlcOptions);
            vlcControl.SourceProvider.MediaPlayer.Playing += MediaPlayer_Playing;
            vlcControl.SourceProvider.MediaPlayer.EndReached += MediaPlayer_EndReached;
            vlcControl.SourceProvider.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            vlcControl.SourceProvider.MediaPlayer.Buffering += MediaPlayer_Buffering;
            vlcControl.SourceProvider.MediaPlayer.Audio.Volume = settings.Mute ? 0 : (int)(settings.Volume * 100);
            SharedVLCControl = vlcControl;
            Play();

            VLCCreated?.Invoke(this, vlcControl);
        }

        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            Trace.WriteLine("Started playing");
        }

        private void MediaPlayer_Buffering(object sender, Vlc.DotNet.Core.VlcMediaPlayerBufferingEventArgs e)
        {
            Trace.WriteLine("Buffering");
        }

        private void MediaPlayer_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            Trace.WriteLine("Got an error");
            PlayNext();
        }

        private void MediaPlayer_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            Trace.WriteLine("End reached");
            PlayNext();
        }
    }
}
