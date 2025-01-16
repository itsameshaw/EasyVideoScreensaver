using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVideoScreensaver.Buit_in_Plugins
{
    internal class DefaultHandler : ISaverPlugin
    {
        string ISaverPlugin.Name { get { return "DefaultHandler"; } }

        Version ISaverPlugin.Version { get { return new Version(1, 0, 0); } }

        string ISaverPlugin.Author { get { return "It's-a Me Shaw"; } }

        string ISaverPlugin.Website { get { return "https://github.com/itsameshaw"; } }

        string ISaverPlugin.UpdateUrl { get { return "";  } }

        bool ISaverPlugin.CanHandleURL(string url)
        {
            return url.StartsWith("file://") || url.StartsWith("http://") || url.StartsWith("https://");
        }

        Task<Uri> ISaverPlugin.HandleURL(string url)
        {
            return Task.FromResult(new Uri(url));
        }
    }
}
