using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SandboxDeveloperPreviewTortureServiceSimulator.MVVM.ViewModel
{
    public class ParticipantViewModel : INotifyPropertyChanged
    {
        private ParticipantModel _selectedParticipant;

        public static ObservableCollection<ParticipantModel> Participants { get; set; }

        public ParticipantViewModel()
        {
            Participants = new ObservableCollection<ParticipantModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
