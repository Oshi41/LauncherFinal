using Core.Settings;
using LauncherFinal.Models;
using Mvvm;

namespace LauncherFinal.ViewModels
{
    class MainViewModel : BindableBase
    {
        private bool _isFlipped;
        private readonly ISettings _settings;
        public StartViewModel Front { get;  }
        public LauncherSettingsViewModel Back { get;  }

        public bool IsFlipped
        {
            get => _isFlipped;
            set => SetProperty(ref _isFlipped, value);
        }

        public MainViewModel()
        {
            _settings = IoCContainer.Instance.Resolve<ISettings>();
            Front = new StartViewModel();
            Back = new LauncherSettingsViewModel(_settings);
        }
    }
}
