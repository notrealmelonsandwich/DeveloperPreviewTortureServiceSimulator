using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SandboxDeveloperPreviewTortureServiceSimulator.MVVM.Model
{
    public class ParticipantModel : INotifyPropertyChanged
    {
        private string _name;
        private BitmapImage _photo;
        private bool _isClient;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public BitmapImage Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged("Photo");
            }
        }

        public bool IsClient
        {
            get => _isClient;
            set
            {
                _isClient = value;
                OnPropertyChanged("IsClient");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
