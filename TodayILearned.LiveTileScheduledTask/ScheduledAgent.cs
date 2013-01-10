using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Info;
using Microsoft.Phone.Scheduler;
using TodayILearned.Core;

namespace TodayILearned.LiveTileScheduledTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
                var model = new MainViewModel();
                model.OnLoaded += () =>
                {
                    if (model.Item == null) return;

                    string title = "Trivia Buff";
                    string description = model.Item.Title;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            LiveTile.UpdateLiveTile(title, description);
                            Debug.WriteLine("Current memory - updated: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);
                        }
                        catch
                        {
                            // TODO: log error
                        }
                        
                        NotifyComplete();
                    });
                };
                model.OnError += exception =>
                {
                    // TODO: log error
                    NotifyComplete();
                };
                model.LoadData();
            }
            catch (Exception e)
            {
                NotifyComplete();
            }
        }
    }
}