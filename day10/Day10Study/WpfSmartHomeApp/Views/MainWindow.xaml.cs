﻿using System.Windows;
using System.Windows.Input;
using WpfSmartHomeApp.Helpers;
using WpfSmartHomeApp.ViewModels;

namespace WpfSmartHomeApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // 앱 완전종료
            Common.LOGGER.Info("스마트홈 모니터링앱 종료!");
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 제목표시줄 X버튼 누를때, Alt+F4 누를때 발생하는 버튼
            e.Cancel = true; // 앱종료를 막는 기능
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadedCommand.Execute(null); // LoadedCommand -> ViewModel의 OnLoad메서드 실행
            }
        }
    }
}