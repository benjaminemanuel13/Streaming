using GLib;
using StreamConsole;
using StreamingClientLinux.Skia.Gtk.Wrappers;
using System;
using Uno.UI.Runtime.Skia;

namespace StreamingClientLinux.Skia.Gtk
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
            {
                Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
                expArgs.ExitApplication = true;
            };

            
            App.Controller = new ControlWrapper();
            var host = new GtkHost(() => new App(), args);

            host.Run();
        }
    }
}
