using StreamingClientLinux.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StreamingClientLinux
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static IControlWrapper controller { get; set; }
        //Button connect = null;
        //Button disconnect = null;
        //Button start = null;
        //Button stop = null;

        public MainPage()
        {
            this.InitializeComponent();
            controller = App.Controller;
            connect = (Button)this.FindName("connect");
            disconnect = (Button)this.FindName("disconnect");
            start = (Button)this.FindName("start");
            stop = (Button)this.FindName("stop");
        }

        private async void connectClick(object sender, RoutedEventArgs e)
        {
            
            connect.IsEnabled = false;
            disconnect.IsEnabled = true;
            start.IsEnabled = true;
            stop.IsEnabled = false;

            await controller.Connect();
        }

        private async void disconnectClick(object sender, RoutedEventArgs e)
        {
            connect.IsEnabled = true;
            disconnect.IsEnabled = false;
            start.IsEnabled = false;
            stop.IsEnabled = false;

            await controller.Stop();
            await controller.Disconnect();
        }

        private async void startClick(object sender, RoutedEventArgs e)
        {
            start.IsEnabled = false;
            stop.IsEnabled = true;

            await controller.Start();
        }

        private async void stopClick(object sender, RoutedEventArgs e)
        {
            start.IsEnabled = true;
            stop.IsEnabled = false;

            await controller.Stop();
        }
    }
}
