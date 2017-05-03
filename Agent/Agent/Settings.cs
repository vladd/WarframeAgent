﻿using Core;

namespace Agent
{
    #region ViewModel Setting

    /// <summary>
    ///     ViewModel для настроек.
    /// </summary>
    internal class MainSettings : SettingCore<MainSettings>
    {
        public Directories Directories = new Directories();
        public Urls Urls = new Urls();
    }

    internal class Directories
    {
        public string Data { get; set; } = "Data";
        public string Temp { get; set; } = "Data/Temp";
    }

    internal class Urls
    {
        public string Game { get; set; } = "http://content.warframe.com/dynamic/worldState.php";
        public string News { get; set; } = "XXX";
    }

    #endregion

    #region Settings class 

    /// <summary>
    ///     Класс настроек приложения
    /// </summary>
    internal static class Settings
    {
        /// <summary>
        ///     Настройки программы.
        /// </summary>
        public static MainSettings Program;

        /// <summary>
        ///     Выполняем загрузку всех необходимых настроек из файла/сервера.
        /// </summary>
        public static void Load()
        {
            Program = MainSettings.Load();
        }
    }

    #endregion
}
