using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.Model;
using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.ViewModel;
using SandboxDeveloperPreviewTortureServiceSimulator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandboxDeveloperPreviewTortureServiceSimulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxParticipantsInRow = 24;

        private int _keys;
        private int _participants;

        private Random _random;

        public static MainWindow Instance { get; private set; }

        private List<StackPanel> ParticipantGridRows = new List<StackPanel>();

        public MainWindow()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitializeComponent();
            DataContext = new ParticipantViewModel();
            _random = new Random();

        }

        #region Participant grid drawing

        private void UpdateGrid()
        {
            ParticipantGridRows.Clear();
            StackPanelParticipantGrid.Children.Clear();

            CreateRow();

            foreach (StackPanel row in StackPanelParticipantGrid.Children)
            {
                ParticipantGridRows.Add(row);
            }

            foreach (ParticipantModel participant in ParticipantViewModel.Participants)
            {
                if (ParticipantGridRows[ParticipantGridRows.Count - 1].Children.Count >= MaxParticipantsInRow)
                    CreateRow();
                DrawUserCell(participant);
                
            }
        }

        private void DrawUserCell(ParticipantModel participant)
        {
            // checking if there are not more or equals than MaxParticipantsInRow otherwise creating a new row
            //if (!(ParticipantGridRows[ParticipantGridRows.Count - 1].Children.Count >= MaxParticipantsInRow))
            //{
                Border border = new Border();
                ImageBrush borderBrush = new ImageBrush();

                border.CornerRadius = new CornerRadius(4);
                border.Height = 32;
                border.Width = 32;
                border.ToolTip = participant.Name;
                border.Margin = new Thickness(2);
                border.Background = borderBrush;

                borderBrush.Stretch = Stretch.Fill;
                borderBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/{participant.Photo}"));

                ParticipantGridRows[ParticipantGridRows.Count - 1].Children.Add(border);
            /*}
            //else
            {
                CreateRow();
                DrawUserCell(participant);
            }*/
        }

        private void CreateRow()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            StackPanelParticipantGrid.Children.Add(stackPanel);
            ParticipantGridRows.Add(stackPanel);
        }

        #endregion

        private void ButtonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxKeyAmount.Text, out _keys);
            int.TryParse(TextBoxParticipants.Text, out _participants);

            Simulate(_keys, _participants);
        }

        private void Simulate(int keys, int participants)
        {
            GenerateUsers(participants);
            UpdateGrid();
        }

        private void GenerateUsers(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // deciding which type of a participant to add:
                // a predefined one with a nickname and a photo, or with a random photo and nickname
                ((ObservableCollection<ParticipantModel>)ParticipantViewModel.Participants).Add(GenerateParticipant(_random.NextBool()));
                
            }
        }

        private ParticipantModel GenerateParticipant(bool predefined)
        {
            // generating predefined
            if (predefined && !AllPredefinedParticipantsPresent())
            {
                foreach (var pair in Users.Predefined)
                {
                    if (!IsParticipantPresent(pair.Key))
                        return new ParticipantModel(pair.Key, pair.Value);
                }
                return null; // not possible i hope
            }
            else
            {
                return new ParticipantModel();
            }
        }

        private bool IsParticipantPredefined(ParticipantModel participant)
        {
            foreach (var pair in Users.Predefined)
            {
                if (pair.Key == participant.Name && pair.Value == participant.Photo)
                    return true;
            }
            return false;
        }

        private bool IsParticipantPresent(string name)
        {
            foreach (ParticipantModel participant in ParticipantViewModel.Participants)
            {
                if (participant.Name == name)
                    return true;
            }
            return false;
        }

        private bool AllPredefinedParticipantsPresent()
        {
            int predefinedCount = Users.Predefined.Count;
            int predefinedPresent = 0;

            foreach (ParticipantModel participant in ParticipantViewModel.Participants)
            {
                if (IsParticipantPredefined(participant))
                    predefinedPresent++;
            }

            return predefinedPresent == predefinedCount;
        }

        private List<ParticipantModel> PresentPredefinedParticipants()
        {
            List<ParticipantModel> list = new List<ParticipantModel>();
            foreach (ParticipantModel participant in ParticipantViewModel.Participants)
            {
                if (IsParticipantPredefined(participant))
                    list.Add(participant);
            }
            return list;
        }
    }
}
