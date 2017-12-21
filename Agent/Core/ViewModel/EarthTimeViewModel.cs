﻿using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Core.ViewModel
{
    public class EarthTimeViewModel : VM
    {
        DateTime time = new DateTime(2017, 12, 20);
        public DateTime Time
        {
            get => time;
            set => Set(ref time, value);
        }

        private string cycle;
        public string Cycle
        {
            get => cycle;
            set => Set(ref cycle, value);
        }

        private string cycleIcon;
        public string CycleIcon
        {
            get => cycleIcon;
            set => Set(ref cycleIcon, value);
        }

        private string colorOne;
        public string ColorOne
        {
            get => colorOne;
            set => Set(ref colorOne, value);
        }

        private string colorTwo;
        public string ColorTwo
        {
            get => colorTwo;
            set => Set(ref colorTwo, value);
        }

        public string LocationName { get; } = "Земля";

        public void UpdateTime()
        {
            var unixTime = Tools.Time.ToUnixTime(DateTime.Now) / 3600000;
            var hour = unixTime % 24;

            if ((hour >= 0 && hour < 4) || (hour >= 8 && hour < 12) || (hour >= 16 && hour < 20))
            {
                Cycle = "День";
                CycleIcon = "SunOutline";
                ColorOne = "#CCFFA500";
                ColorTwo = "#4CFFA500";
            }
            else
            {
                Cycle = "Ночь";
                CycleIcon = "MoonOutline";
                ColorOne = "#CC3782CD";
                ColorTwo = "#4C3782CD";
            }

            var hourleft = 3 - (hour % 4);
            var minutes = 59 - DateTime.Now.Minute;
            var seconds = 59 - DateTime.Now.Second;

            Time = DateTime.Parse($"{hourleft}:{minutes}:{seconds}");
        }
    }
}
