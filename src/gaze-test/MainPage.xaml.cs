using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace gaze_test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GazeDeviceWatcherPreview _watcher;

        private uint _noofEyeTrackers = 0;
        private GazeInputSourcePreview _gazeSource;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HandleEyeTrackingChanged();
            StartDeviceWatcher();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopDeviceWatcher();

            base.OnNavigatedFrom(e);
        }

        private void StartDeviceWatcher()
        {
            _watcher = GazeInputSourcePreview.CreateWatcher();
            _watcher.Added += OnDeviceAdded;
            _watcher.Removed += OnDeviceRemoved;
            _watcher.Updated += OnDeviceUpdated;

            _watcher.Start();
        }

        private void StopDeviceWatcher()
        {
            _watcher.Stop();
            _watcher = null;
        }

        private void HandleEyeTrackingChanged()
        {
            gazeTrace.Visibility = _noofEyeTrackers > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (_noofEyeTrackers > 0 && _gazeSource == null)
            {
                _gazeSource = GazeInputSourcePreview.GetForCurrentView();
                _gazeSource.GazeMoved += GazeMovedHandler;
            }

            if (_noofEyeTrackers == 0 && _gazeSource != null) {
                _gazeSource.GazeMoved -= GazeMovedHandler;
                _gazeSource = null;
            }
        }

        private void OnDeviceAdded(GazeDeviceWatcherPreview sender, GazeDeviceWatcherAddedPreviewEventArgs args)
        {
            if (args.Device.CanTrackEyes == true)
            {
                _noofEyeTrackers++;
                HandleEyeTrackingChanged();
            }
        }

        private void OnDeviceUpdated(GazeDeviceWatcherPreview sender, GazeDeviceWatcherUpdatedPreviewEventArgs args)
        {
            if (args.Device.CanTrackEyes == true)
            {
                // do something
            }
        }

        private void OnDeviceRemoved(GazeDeviceWatcherPreview sender, GazeDeviceWatcherRemovedPreviewEventArgs args)
        {
            if(args.Device.CanTrackEyes == true)
            {
                _noofEyeTrackers--;
                HandleEyeTrackingChanged();
            }
        }

        private void GazeMovedHandler(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            var gazePoint = args.CurrentPoint.EyeGazePosition.Value;

            _gazeMarkerTransform.X = gazePoint.X - gazeTrace.ActualWidth / 2;
            _gazeMarkerTransform.Y = gazePoint.Y - gazeTrace.ActualHeight / 2;
        }

    }
}
