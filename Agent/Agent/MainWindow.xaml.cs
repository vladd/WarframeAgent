﻿using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Agent.Data;
using Agent.Events;
using static Agent.Events.GlobalEvents;
using System.Windows.Threading;
using System.Threading;

namespace Agent
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAnimation();
            ThemeChange(Settings.Program.Theme);
            GameDataEvent.Connected += GameDataEvent_Connected;
            GameDataEvent.Disconnected += GameDataEvent_Disconnected;
            GameDataEvent.Updated += GameDataEvent_Updated;
        }

        private void GameDataEvent_Updated()
        {
            Game.Load();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                if (alertbox.ItemsSource != null) alertbox.ItemsSource = null;
                alertbox.ItemsSource = Game.Data.Alerts;
            });
        }

        #region События

        private void GameDataEvent_Disconnected()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ConnLostImg.Visibility = Visibility.Visible;
            });
        }

        private void GameDataEvent_Connected()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ConnLostImg.Visibility = Visibility.Collapsed;
            });
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"w.{e.NewSize.Width} h.{e.NewSize.Height}");
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Settings.Program.Save();
        }

        /// <summary>
        /// Изменение темы приложения
        /// </summary>
        /// <param name="theme">Темная или светлая</param>
        private void ThemeChange(Themes theme)
        {
            var uri = new Uri($"Styles/Theme/{theme}.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            //Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            Settings.Program.Theme = theme;
            Settings.Program.Save();
        }

        #endregion

        #region Взаимодействие с окном

        //Перетаскиваем окно мышью.
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion

        #region Закрытие и сворачивание

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Settings.Program.Save();
            Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            //var alertPage = new AlertsPage();
            //alertPage._statsTimer.Stop(); //TODO: Включить, после создания. (обновление таймера в инфе таймера)
            //TaskbarIcon.Visibility = Visibility.Visible;
            //Hide();
        }



        #endregion

        #region Анимация

        private void InitializeAnimation()
        {
            LeftPanelAnimation(); //Анимация боковой панели
        }

        #region Боковая панель

        private void LeftPanelAnimation()
        {
            LeftPanelContent.Opacity = 0;
            var animation = new Storyboard();
            var a = new DoubleAnimation()
            {
                From = 0,
                To = 40,
                Duration = TimeSpan.FromSeconds(0.3),
            };
            a.Completed += LeftPanelAnimation_Completed;
            Storyboard.SetTarget(a, LeftPanelGrid);
            Storyboard.SetTargetProperty(a, new PropertyPath(WidthProperty));
            animation.Children.Add(a);
            animation.Begin();
        }

        private void LeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            var animation = new Storyboard();
            var a = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTarget(a, LeftPanelContent);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            animation.Children.Add(a);
            animation.Begin();
        }
        #endregion

        #endregion

        private void ButtonEvent(string name)
        {
            switch (name)
            {
                case "ThemeBtn":
                    var res = MessageBox.Show("При смене темы могут возникнуть \"артефакты\".\nРекомендую перезапустить приложение.", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (Settings.Program.Theme == Themes.Dark)
                    {
                        if (res == MessageBoxResult.OK) ThemeChange(Themes.Light);
                    }
                    else
                    { 
                        if (res == MessageBoxResult.OK) ThemeChange(Themes.Dark);
                    }

                    break;
                case "ChangeBg":
                    //RandomBackground();
                    break;
                case "NewsBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/NewsPage.xaml", UriKind.Relative));
                    break;
                case "AlertsBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/AlertsPage.xaml", UriKind.Relative));
                    break;
                case "TradeBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    TradeBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/TradePage.xaml", UriKind.Relative));
                    break;
                case "InvasionsBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/InvasionsPage.xaml", UriKind.Relative));
                    break;
                case "InfoBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/InfoPage.xaml", UriKind.Relative));
                    break;
                case "SettingsBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    //BodyFrame.Navigate(new Uri("Pages/SettingsPage.xaml", UriKind.Relative));
                    break;
                case "ActMissionsBtn":
                    NewsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    AlertsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InvasionsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    InfoBtn.Style = (Style)Application.Current.Resources["Menu"];
                    SettingsBtn.Style = (Style)Application.Current.Resources["Menu"];
                    TradeBtn.Style = (Style)Application.Current.Resources["Menu"];
                    ActMissionsBtn.Style = (Style)Application.Current.Resources["MenuIn"];
                    //BodyFrame.Navigate(new Uri("Pages/ActiveMissionsPage.xaml", UriKind.Relative));
                    break;
            }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button srcButton) ButtonEvent(srcButton.Name);
        }
    }
}
