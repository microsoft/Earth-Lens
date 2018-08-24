using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UIKit;

[assembly:AssemblyVersion("1.0")]
[assembly:CLSCompliant(false)]
[assembly:ComVisible(false)]

namespace EarthLens.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
