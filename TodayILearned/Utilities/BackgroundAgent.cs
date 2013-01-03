﻿using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using TodayILearned.Core;
using TodayILearned.Resources;

namespace TodayILearned.Utilities
{
    internal sealed class BackgroundAgent
    {
        private const string TaskName = "TodayILearned.LiveTileScheduledTask";

        #region Public Methods

        public void Toggle()
        {
            bool result = false;
            AppSettings.LiveTileDisabled = !AppSettings.LiveTileDisabled;

            if (AppSettings.LiveTileDisabled)
            {
                Stop();
                ResetTileToDefault();
                result = true;
            }
            else
            {
                result = StartIfEnabledInternal();
            }

            // do not switch values if not succesful
            if (!result)
            {
                AppSettings.LiveTileDisabled = !AppSettings.LiveTileDisabled;
            }
        }

        public bool StartIfEnabled()
        {
            bool result = true;
            if (!AppSettings.LiveTileDisabled)
            {
                result = StartIfEnabledInternal();
            }
            return result;
        }

        #endregion

        #region Helper Methods

        private bool StartIfEnabledInternal()
        {
            bool result = true;

            Stop();
            result = Start();

#if DEBUG
            // If we're debugging, attempt to start the task immediately 
            ScheduledActionService.LaunchForTest(TaskName, new TimeSpan(0, 0, 1));
#endif

            return result;
        }

        private bool Start()
        {
            bool result = false;
            try
            {
                PeriodicTask task = new PeriodicTask(TaskName);
                task.Description = "Service to update Trivia Buff live tile";
                ScheduledActionService.Add(task);
                result = true;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    Strings.ErrorCouldNotEnableLiveTileDescription,
                    Strings.ErrorCouldNotEnableLiveTileTitle,
                    MessageBoxButton.OK);
            }
            catch (Exception)
            {
                // still show the main UI
            }

            return result;
        }

        private void Stop()
        {
            try
            {
                if (ScheduledActionService.Find(TaskName) != null)
                {
                    ScheduledActionService.Remove(TaskName);
                }
            }
            catch (Exception)
            {
                // ignore, best effort cleanup
            }
        }

        public bool ResetTileToDefault()
        {
            bool result = false;

            try
            {
                StandardTileData tileData = new StandardTileData()
                {
                    Title = "On This Day...",
                    BackgroundImage = new Uri("/icons/Application_Icon_173.png", UriKind.Relative),
                    BackBackgroundImage = new Uri("NONESUCH.png", UriKind.Relative),
                    BackTitle = string.Empty,
                    BackContent = string.Empty
                };

                ShellTile tile = ShellTile.ActiveTiles.First();
                tile.Update(tileData);
                result = true;
            }
            catch (Exception)
            {
                // ignore, best effort cleanup
            }

            return result;
        }

        #endregion
    }
}
