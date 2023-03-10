using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Jbh
{
    /// <summary>
    ///   Contains helper methods for UI, so far just one for showing a waitcursor and only cancelling it once the system is not busy
    /// </summary>
    internal static class UiServices
    {
        /// <summary>
        ///   A value indicating whether the UI is currently busy
        /// </summary>
        private static bool _isBusy;

        /// <summary>
        /// Sets the busystate as busy.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        /// Sets the busystate to busy or not busy.
        /// </summary>
        /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
        private static void SetBusyState(bool busy)
        {
            if (busy != _isBusy)
            {
                _isBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (_isBusy)
                {
#pragma warning disable CA1806 // Do not ignore method results
                    _ = new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle
                        , DispatcherTimer_Tick, Application.Current.Dispatcher);
#pragma warning restore CA1806 // Do not ignore method results
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (sender is DispatcherTimer dispatcherTimer)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }

    }
}
