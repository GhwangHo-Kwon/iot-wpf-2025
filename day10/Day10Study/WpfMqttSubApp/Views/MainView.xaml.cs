using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfMqttSubApp.ViewModels;

namespace WpfMqttSubApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();

            var vm = new MainViewModel(DialogCoordinator.Instance);
            this.DataContext = vm;
            vm.PropertyChanged += (sender, e) => {
                if (e.PropertyName == nameof(vm.LogText))
                {   // ViewModel의 LogText 속성 값이 변경되었으면
                    // Dispatcher 객체 내의 UI렌더링을 넣어줘야 동작함
                    Dispatcher.InvokeAsync(() => {
                        LogBox.ScrollToEnd();  // 윈앱에서 이미 사용
                    });
                }
            };
        }
    }
}
