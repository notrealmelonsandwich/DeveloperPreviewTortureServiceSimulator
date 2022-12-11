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
        private const int MaxParticipantsInRow = 30;

        private int _keys;
        private int _participants;

        public Random _random;

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

        private void DrawGrid()
        {
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
                participant.Index = ParticipantGridRows[ParticipantGridRows.Count - 1].Children.Count - 1;
                participant.RowIndex = ParticipantGridRows.Count - 1;

            }
        }

        private void UpdateGrid()
        {
            ClearGrid();
            DrawGrid();
        }

        private void ClearGrid()
        {
            foreach (StackPanel stackPanel in StackPanelParticipantGrid.Children)
            {
                stackPanel.Children.RemoveRange(0, stackPanel.Children.Count);
            }
            StackPanelParticipantGrid.Children.RemoveRange(0, ParticipantGridRows.Count);

            ParticipantViewModel.Participants.Clear();
        }

        private int GetDrawnUserCount()
        {
            int result = 0;
            foreach (var row in ParticipantGridRows)
            {
                result += row.Children.Count;
            }
            return result;
        }

        private void DrawUserCell(ParticipantModel participant)
        {
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

            if (participant.IsClient)
            {
                border.BorderThickness = new Thickness(3);
                border.BorderBrush = Brushes.Pink;
            }

            ParticipantGridRows[ParticipantGridRows.Count - 1].Children.Add(border);
        }

        private void CreateRow()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            StackPanelParticipantGrid.Children.Add(stackPanel);
            ParticipantGridRows.Add(stackPanel);
        }

        private Border GetParticipantCell(ParticipantModel participant)
        {
            return (Border)ParticipantGridRows[participant.RowIndex].Children[participant.Index];
        }

        private void DrawWinner(ParticipantModel participant)
        {
            Border border = GetParticipantCell(participant);
            border.BorderThickness = new Thickness(3);
            border.BorderBrush = Brushes.Yellow;
        }

        #endregion

        private void ButtonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxKeyAmount.Text, out _keys);
            int.TryParse(TextBoxParticipants.Text, out _participants);

            Simulate(_keys, _participants, false);
        }

        private void ButtonStartMultipleSimulation_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TextBoxKeyAmount.Text, out _keys);
            int.TryParse(TextBoxParticipants.Text, out _participants);

            Simulate(_keys, _participants, true);
        }

        private void Simulate(int keys, int participants, bool multipleTimes)
        {
            ClearGrid();
            GenerateUsers(participants);

            if (!multipleTimes)
            {
                DrawGrid();
                //UpdateGrid();
                PickWinner();
            }
            else
            {
                //bool victory = false;
                int iterations = 1;

                while (true)
                {
                    ParticipantModel[] winners = new ParticipantModel[_keys];
                    bool containsClient = false;

                    for (int i = 1; i <= _keys; i++)
                    {
                        ParticipantModel winner = ParticipantViewModel.Participants[_random.Next(0, ParticipantViewModel.Participants.Count)];
                        if (winner.IsClient)
                            containsClient = true;

                        winners[i - 1] = winner;
                    }

                    if (!containsClient)
                    {
                        iterations++;
                    }
                    else
                    {
                        double hours = iterations * 6;
                        TimeSpan span = TimeSpan.FromHours(hours);
                        MessageBox.Show($"This is {hours} hours of tireless waiting\nOR: {span.TotalDays} days, ~{span.TotalDays / 30:0.00} months, ~{(span.TotalDays / 365):0.00} years", $"You won after {iterations} times!");
                        break;
                    }
                }
            }
        }

        private void PickWinner()
        {
            ParticipantModel[] winners = new ParticipantModel[_keys];
            bool containsClient = false;

            for (int i = 1; i <= _keys; i++)
            {
                ParticipantModel winner = ParticipantViewModel.Participants[_random.Next(0, ParticipantViewModel.Participants.Count)];
                if (winner.IsClient)
                    containsClient = true;

                winners[i - 1] = winner;
                DrawWinner(winner);
            }

            string text = "The winners are: ";
            for (int i = 0; i < winners.Length; i++)
            {
                text += $"{winners[i].Name}";
                if (winners[i].IsClient)
                    text += " (You)";

                if (i != winners.Length - 1)
                    text += ", ";
            }

            string caption = containsClient ? "You won" : "You lost. Again.";
            MessageBox.Show(text, caption);
        }

        private void GenerateUsers(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // deciding which type of a participant to add:
                // a predefined one with a nickname and a photo, or with a random photo and nickname
                ParticipantModel participant = GenerateParticipant(_random.NextBool());
                if (i == 0)
                    participant.IsClient = true;

                ParticipantViewModel.Participants.Add(participant);
                
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

        private void RaffleParameters_TextChanged(object sender, TextChangedEventArgs e)
        {
            // bruh wpf is shit
            if (TextBlockEstimatedOdds == null || TextBoxKeyAmount == null || TextBoxParticipants == null)
                return;

            double estimatedOdds = 0;
            double keys = 0;
            double users = 0;

            try
            {
                keys = (double)int.Parse(TextBoxKeyAmount.Text);
                users = (double)int.Parse(TextBoxParticipants.Text);
                estimatedOdds = keys / users * 100;

                LabelKeyAmount.Text = keys.ToString("0");
                LabelParticipantAmount.Text = users.ToString("0");

                if (keys > users)
                    return;
            }
            catch (Exception) { }

            TextBlockEstimatedOdds.Text = $"estimated odds {estimatedOdds:0.00}%";
        }
    }
}
