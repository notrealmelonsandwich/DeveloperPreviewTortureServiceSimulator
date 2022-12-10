using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.ViewModel;
using System;
using System.Windows;

namespace SandboxDeveloperPreviewTortureServiceSimulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _keys;
        private int _participants;

        private Random _random;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ParticipantViewModel();
            _random = new Random();
        }

        private void ButtonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxKeyAmount.Text, out _keys);
            int.TryParse(TextBoxParticipants.Text, out _participants);

            Simulate(_keys, _participants);
        }

        private void Simulate(int keys, int participants)
        {
            GenerateUsers(participants);
        }

        private void GenerateUsers(int amount)
        {

        }
    }
}
