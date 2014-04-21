using System;
using System.Windows;
using Microsoft.Phone.Shell;
using TodayILearned.Core;
using TodayILearned.Utilities;

namespace TodayILearned
{
    public class LiveTileCreator : LiveTile
    {
        public static void CreateLiveTile(string title, string content)
        {
            //application tile is always the first tile, even if it is not pinned
            if (!IsTargetedVersion)
            {
                var data = GetTile(title, content);
                var tiles = ShellTile.ActiveTiles;
                foreach (var tile in tiles)
                {
                    if (tile != null)
                    {
                        tile.Update(data);
                    }
                }
                ShellTile.Create(new Uri("/MainPage.xaml?DefaultTitle=new", UriKind.Relative), data);
            }
            else
            {
                string fontSize = Application.Current.Resources["PhoneFontSizeLarge"].ToString();
                string fileNameMed = WriteTileToDisk(title, content, 336, 336, fontSize, new Thickness(19, 13, 13, 38));
                string fileNameBig = WriteTileToDisk(title, content, 691, 336, fontSize, new Thickness(19, 13, 13, 38));

                var backBackgroundImage = new Uri("", UriKind.Relative);
                var wideBackBackgroundImage = new Uri("", UriKind.Relative);
                const string backTitle = "";

                UpdateFlipTile(
                    title,
                    backTitle,
                    string.Empty,
                    string.Empty,
                    0,
                    new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                    new Uri("isostore:" + fileNameMed),
                    backBackgroundImage,
                    new Uri("isostore:" + fileNameBig),
                    wideBackBackgroundImage);

                CreateFlipTile(
                    title,
                    backTitle,
                    string.Empty,
                    string.Empty,
                    0,
                    new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                    new Uri("isostore:" + fileNameMed),
                    backBackgroundImage,
                    new Uri("isostore:" + fileNameBig),
                    wideBackBackgroundImage);
            }
        }

        private static void CreateFlipTile(
            string title,
            string backTitle,
            string backContent,
            string wideBackContent,
            int count,
            Uri smallBackgroundImage,
            Uri backgroundImage,
            Uri backBackgroundImage,
            Uri wideBackgroundImage,
            Uri wideBackBackgroundImage)
        {
            if (!IsTargetedVersion)
                return;

            var flipTileData = new FlipTileData
            {
                Title = title,
                Count = count,
                BackTitle = backTitle,
                BackContent = backContent,
                SmallBackgroundImage = smallBackgroundImage,
                BackgroundImage = backgroundImage,
                BackBackgroundImage = backBackgroundImage,
                WideBackgroundImage = wideBackgroundImage,
                WideBackBackgroundImage = wideBackBackgroundImage,
                WideBackContent = wideBackContent
            };

            ShellTile.Create(new Uri("/MainPage.xaml?DefaultTitle=new", UriKind.Relative), flipTileData, true);
        }
    }
}
