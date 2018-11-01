using System.Windows.Input;
using LauncherCore.Settings;
using Mvvm;
using Mvvm.Commands;

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
            get { return _isFlipped; }
            set { SetProperty(ref _isFlipped, value); }
        }

        public MainViewModel()
        {
            _settings = IoCContainer.Instance.Resolve<ISettings>();
            Front = new StartViewModel();
            Back = new LauncherSettingsViewModel(_settings);
        }
    }
}
