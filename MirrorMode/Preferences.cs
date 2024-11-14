using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MirrorMode
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isEnabled = false;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isMirrorBabeArt = false;

        public bool IsMirrorBabeArt
        {
            get => _isMirrorBabeArt;
            set
            {
                _isMirrorBabeArt = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
