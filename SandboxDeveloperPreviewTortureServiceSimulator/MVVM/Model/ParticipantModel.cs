using SandboxDeveloperPreviewTortureServiceSimulator.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SandboxDeveloperPreviewTortureServiceSimulator.MVVM.Model
{
    public class ParticipantModel : INotifyPropertyChanged
    {
        private string _name;
        private string _photo;
        private bool _isClient;
        private bool _isWinner;
        
        public int Index { get; set; }
        public int RowIndex { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Photo
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

        public bool IsWinner
        {
            get => _isWinner;
            set
            {
                _isWinner = value;
                OnPropertyChanged("IsWinner");
            }
        }

        /// <summary>
        /// Generated a participant with a random name
        /// </summary>
        public ParticipantModel()
        {
            Name = Users.Nicknames[MainWindow.Instance._random.Next(0, Users.Nicknames.Count)];
            Photo = Users.Photos[MainWindow.Instance._random.Next(0, Users.Photos.Count)];

            /*
            if (Name == "melonsandwich")
                IsClient = true;
            */
        }

        public ParticipantModel(string name)
        {
            Name = name;
        }

        public ParticipantModel(string name, string photo)
        {
            Name = name;
            Photo = photo;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
