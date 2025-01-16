using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVideoScreensaver
{
    public interface ISaverPlugin
    {
        Version Version { get; }

        string Name { get; }
        string Author { get; }
        string Website { get; }
        string UpdateUrl { get; }

        bool CanHandleURL(string url);
        Task<Uri> HandleURL(string url);
    }
}
