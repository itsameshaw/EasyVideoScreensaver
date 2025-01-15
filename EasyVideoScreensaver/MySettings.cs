using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EasyVideoScreensaver
{ 
    public class MySettings
    {
        public List<string> Videos { get; set; } = new List<string>();
        public string VideoPlayMode { get; set; } = "In Order";
        public string LastPlayedVideo {  get; set; }
        public string VideoFilename { get; set; }
        public string VLCPath { get; set; }
        public string StretchMode { get; set; }
        public double Volume { get; set; }
        public Boolean Mute { get; set; }
        public Boolean Resume { get; set; }

        public double ResumePosition { get; set; }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                xmls.Serialize(sw, this);
            }
        }

        public static MySettings Load(string filename)
        {
            MySettings settings;
            if (System.IO.File.Exists(filename))
            {
                //Load settings
                using (StreamReader sw = new StreamReader(filename))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(MySettings));
                    settings = xmls.Deserialize(sw) as MySettings;
                }
            }
            else
            {
                //Get default settings
                settings = new MySettings();
                settings.LastPlayedVideo = "";
                settings.VideoFilename = "";
                settings.VLCPath = "";
                settings.StretchMode = "Fit";
                settings.Mute = false;
                settings.Volume = .5;
                settings.Resume = false;
                settings.ResumePosition = 0;
                return settings;
            }

            if (settings.VideoFilename.Length > 0 && settings.Videos.Count == 0)
            {
                settings.Videos.Add($"file://{settings.VideoFilename}");
            }

            //Validate volume
            if (settings.Volume < 0)
                settings.Volume = 0;
            if (settings.Volume > 1)
                settings.Volume = 1;

            return settings;
        }
    }
}