using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.Model;
using SandboxDeveloperPreviewTortureServiceSimulator.MVVM.ViewModel;
using SandboxDeveloperPreviewTortureServiceSimulator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SandboxDeveloperPreviewTortureServiceSimulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxParticipantsInRow = 36;

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

        private async void DrawGrid()
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

                await DrawUserCell(participant);
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

        private async Task DrawUserCell(ParticipantModel participant)
        {
            Dispatcher.Invoke(() =>
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
            });

            // fix animation to make it sync with the raffle start
            await Task.Delay(0);
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

        private void UndrawWinner(ParticipantModel participant)
        {
            Border border = GetParticipantCell(participant);
            border.BorderThickness = new Thickness(0);
        }

        #endregion

        private void ParseSettings()
        {
            int.TryParse(TextBoxKeyAmount.Text, out _keys);
            int.TryParse(TextBoxParticipants.Text, out _participants);
        }

        private void ButtonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            ParseSettings();
            Simulate(_keys, _participants, false);
        }

        private void ButtonStartMultipleSimulation_Click(object sender, RoutedEventArgs e)
        {
            ParseSettings();
            Simulate(_keys, _participants, true);
        }

        private async void Simulate(int keys, int participants, bool multipleTimes)
        {
            ClearGrid();
            GenerateUsers(participants);

            if (!multipleTimes)
            {
                DrawGrid();
                //UpdateGrid();
                await GiveawayAnimation(PickWinners());
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

                        // avoiding repeatings
                        if (!winners.ToList().Contains(winner))
                            winners[i - 1] = winner;
                        else
                            i -= 1;
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

        #region Animations
        private async Task GiveawayAnimation(ParticipantModel[] winners)
        {
            Dispatcher.Invoke(() =>
            {
                ButtonStartSimulation.Visibility = Visibility.Hidden;
                ButtonStartMultipleSimulation.Visibility = Visibility.Hidden;
                PanelWinner.Visibility = Visibility.Visible;
                LabelWinner.Text = "...";
            });
            await Task.Delay(3000);

            foreach (ParticipantModel winner in winners)
            {
                Dispatcher.Invoke(() =>
                {
                    DrawWinner(winner);
                    LabelWinner.Text = winner.Name;
                    LabelKeyAmount.Text = (int.Parse(LabelKeyAmount.Text) - 1).ToString();
                });
                await Task.Delay(3000);
                Dispatcher.Invoke(() => UndrawWinner(winner));
            }

            Dispatcher.Invoke(() =>
            {
                ButtonStartSimulation.Visibility = Visibility.Visible;
                ButtonStartMultipleSimulation.Visibility = Visibility.Visible;
                PanelWinner.Visibility = Visibility.Hidden;
                LabelWinner.Text = "...";
            });
            ClearGrid();
        }

        private void PlayAnimation(bool victory)
        {
            // TODO
        }
        #endregion

        private ParticipantModel[] PickWinners()
        {
            ParticipantModel[] winners = new ParticipantModel[_keys];
            bool containsClient = false;

            for (int i = 1; i <= _keys; i++)
            {
                ParticipantModel winner = ParticipantViewModel.Participants[_random.Next(0, ParticipantViewModel.Participants.Count)];
                if (winner.IsClient)
                    containsClient = true;

                winners[i - 1] = winner;
                //DrawWinner(winner);
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
            //MessageBox.Show(text, caption);
            return winners;
        }

        private void GenerateUsers(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // deciding which type of a participant to add:
                // a predefined one with a nickname and a photo, or with a random photo and nickname
                ParticipantModel participant = GenerateParticipant(_random.NextBool());
                if (i == 0)
                {
                    participant.Name = "You";
                    participant.Photo = Users.Photos[1];
                    participant.IsClient = true;
                }

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
            try
            {
                double keys = (double)int.Parse(TextBoxKeyAmount.Text);
                double users = (double)int.Parse(TextBoxParticipants.Text);
                estimatedOdds = keys / users * 100;

                LabelKeyAmount.Text = keys.ToString("0");
                LabelParticipantAmount.Text = users.ToString("0");

                if (keys > users)
                    return;
            }
            catch (Exception) { }

            TextBlockEstimatedOdds.Text = $"estimated odds {estimatedOdds:0.00}%";
        }

        private const string SteamID = "STEAM_0:1:82476071";

        private async void ButtonCopySteamID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SteamID);
            Dispatcher.Invoke(() =>
            {
                ButtonCopySteamID.Content = "copied";
            });
            await Task.Delay(1000);
            Dispatcher.Invoke(() =>
            {
                ButtonCopySteamID.Content = "copy steam id";
            });
        }
    }
}
