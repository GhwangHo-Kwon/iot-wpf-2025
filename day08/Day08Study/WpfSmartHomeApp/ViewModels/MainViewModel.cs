using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private double _homeTemp;
        private double _homeHumid;

        private bool _isDetectOn;
        private bool _isRainOn;
        private bool _isAirConOn;
        private bool _isLightOn;
        private string _detectResult;
        private string _rainResult;
        private string _airConResult;
        private string _lightResult;

        // 온도 속성
        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }

        // 습도 속성
        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }

        // 사람인지
        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        // 사람인지여부
        public bool IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }

        // RainResult, RainOn
        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        public bool IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }

        // AirConResult, IsAirConOn
        public string AirConResult
        {
            get => _airConResult;
            set => SetProperty(ref _airConResult, value);
        }

        public bool IsAirConOn
        {
            get => _isAirConOn;
            set => SetProperty(ref _isAirConOn, value);
        }

        // LightResult, IsLightOn
        public string LightResult
        {
            get => _lightResult;
            set => SetProperty(ref _lightResult, value);
        }

        public bool IsLightOn
        {
            get => _isLightOn;
            set => SetProperty(ref _isLightOn, value);
        }

        // LoadedCommand 에서 앞에 On이 붙고 Command는 삭제
        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 30;
            HomeHumid = 43.2;

            DetectResult = "Detected Human!";
            IsDetectOn = true;
            RainResult = "Raining";
            IsRainOn = true;
            AirConResult = "AirCon On!";
            IsAirConOn = true;
            LightResult = "Light On~";
            IsLightOn = true;
        }
    }
}
