using Dots_and_Lines_Game.GridShapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Dots_and_Lines_Game
{
    public partial class MainWindow : Window
    {
        
        private GameTimer _gameTimer;
        

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the Players class with references to the XAML controls
            Players.Initialize(player1ScoreText, player2ScoreText, playerTurnIndicator, mainGrid);

            
            // Initialize the game timer with callbacks
            _gameTimer = new GameTimer(
                // Update timer display
                (timeLeft) => timerText.Text = $"Time Left: {timeLeft}",

                // Switch player callback
                () =>
                {
                    Players.isPlayer1Turn = !Players.isPlayer1Turn; // switches player turns
                    Players.UpdatePlayerTurnIndicator();

                    // Start the timer again for the next player with the same duration
                    _gameTimer.StartTimer(CurrentTimerDuration);
                }
            );

            // instance of the game timer in the main window
            SquareGridShape.gameTimer = _gameTimer;  

            Bot.SetMainWindow(this); // Set the MainWindow reference in the Bot class
            SquareGridShape.SetMainWindow(this); // Set the MainWindow reference in the SquareGridShape class

            // Set initial game state
            SquareGridShape.GameCanvas = gameCanvas;
            InitializeGame(2);
        }

        public void InitializeGame(int size)
        {
            _gameTimer.StopTimer();
            SquareGridShape.UpdateGridSize(size);
            Players.player1Score = 0;
            Players.player2Score = 0;
            Players.player1ScoreText.Text = "Player 1: 0";
            Players.player2ScoreText.Text = "Player 2: 0";
            Players.isPlayer1Turn = true;
            SquareGridShape.CreateGrid();
            Players.UpdatePlayerTurnIndicator();
        }

        
        // Click events for player color selection
        private void Player1_ColorSelect(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;  // casts the sender object to a MenuItem
            string color = menuItem.Tag.ToString(); // gets the Tag property of the MenuItem converted to a string
            Players.SetPlayer1Color(color);
        }

        private void Player2_ColorSelect(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            string color = menuItem.Tag.ToString();
            Players.SetPlayer2Color(color);
        }

        

        // Click events for grid size
        private void Grid2x2_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(2);
        }

        private void Grid3x3_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(3);
        }

        private void Grid5x5_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(5);
        }

        private void Grid7x7_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(7);
        }

        private void CustomGrid_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomGridSize();
            int? size = dialog.ShowDialog(this);
            if (size.HasValue)
            {
                InitializeGame(size.Value);
            }
        }



        // Click events for grid shape 
        // Triangle and Hexagon grid shape is not implemented yet
        public void SquareGrid_Click(object sender, RoutedEventArgs e)
        {
            SquareGridShape.CreateGrid();
            
        }

        public void TriangleGrid_Click(object sender, RoutedEventArgs e)
        {
            TriangleGridShape.UpdateGridSize(SquareGridShape.CurrentGridSize); // Make sure grid size is set
            TriangleGridShape.CreateGrid();

        }

        public void HexagonGrid_Click(object sender, RoutedEventArgs e)
        {
            //SquareGridShape.CreateHexagonGrid();
            MessageBox.Show("Hexagon grid selected");
        }






        // Add field to track current timer duration
        private static int _currentTimerDuration;  // Private backing field
        public static int CurrentTimerDuration    // Public static property
        {
            get { return _currentTimerDuration; }
            set { _currentTimerDuration = value; }
        }

        // timer click handlers
        private void TimerOff_Click(object sender, RoutedEventArgs e)
        {
            _gameTimer.StopTimer();
            timerText.Text = "Timer Off";
        }

        private void Timer5Seconds_Click(object sender, RoutedEventArgs e)
        {
            CurrentTimerDuration = 5;
            _gameTimer.StartTimer(5);
        }

        private void Timer10Seconds_Click(object sender, RoutedEventArgs e)
        {
            CurrentTimerDuration = 10;
            _gameTimer.StartTimer(10);
        }

        private void Timer15Seconds_Click(object sender, RoutedEventArgs e)
        {
            CurrentTimerDuration = 15;
            _gameTimer.StartTimer(15);
        }

        private void TimerCustom_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomTimer();
            int? customTime = dialog.ShowDialog(this);
            if (customTime.HasValue)
            {
                CurrentTimerDuration = customTime.Value;
                _gameTimer.StartTimer(customTime.Value);
            }
        }


        // click events for saving, loading from a saved file, restarting, undoing move and quitting the game
        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GameStateManager.SaveGame();
                MessageBox.Show("Game saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GameStateManager.LoadGame(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(SquareGridShape.CurrentGridSize);
            timerText.Text = "Timer Off";
        }

        private void UndoMove_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.UndoLastMove();
        }

        private void RedoMove_Click(object sender, RoutedEventArgs e)
        {
            UndoRedo.RedoLastMove();
        }

        private void QuitGame_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        // Click events for board options
        private void Swedish_Click(object sender, RoutedEventArgs e)
        {
            Swedish.InitializeSwedishGrid();
        }

        private void American_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame(SquareGridShape.CurrentGridSize); // Uses the default initialization since American grid is the same as the default grid
        }

        private void Icelandic_Click(object sender, RoutedEventArgs e)
        {
            Icelandic.InitializeIcelandicGrid();
        }



        // Click events for Bot activation
        private void BotOn_Click(object sender, RoutedEventArgs e)
        {
            Bot.EnableBot();
            MessageBox.Show($"Bot is activated, One Player Mode!");
        }

        private void BotOff_Click(object sender, RoutedEventArgs e)
        {
            Bot.DisableBot();
            MessageBox.Show("Bot is turned Off, Two Player Mode!");
        }



    }
}