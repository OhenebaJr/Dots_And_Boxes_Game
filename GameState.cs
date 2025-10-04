using Newtonsoft.Json;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using Dots_and_Lines_Game.GridShapes;
using Dots_and_Lines_Game;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

public class GameState
{
    // Grid state
    public int GridSize { get; set; }
    public bool[,] CompletedBoxes { get; set; }
    public LineState[,] HorizontalLines { get; set; }
    public LineState[,] VerticalLines { get; set; }

    // Player state
    public bool IsPlayer1Turn { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public string Player1Color { get; set; }
    public string Player2Color { get; set; }

    // Timer state (if timer is running)
    public bool IsTimerRunning { get; set; }
    public int CurrentTimerDuration { get; set; }

    // Bot state (if bot is enabled)
    public bool IsBotEnabled { get; set; }
}

public class LineState
{
    public bool IsSelected { get; set; }
    public string Color { get; set; }
}

public static class GameStateManager
{
    // Save the current game state to a file------------------------------------------------------------------------------------------------
    public static void SaveGame()
    {
        var gameState = new GameState
        {
            // Grid state
            GridSize = SquareGridShape.CurrentGridSize,
            CompletedBoxes = SquareGridShape.CompletedBoxes,
            HorizontalLines = ConvertLinesToState(SquareGridShape.HorizontalLines),
            VerticalLines = ConvertLinesToState(SquareGridShape.VerticalLines),

            // Player state
            IsPlayer1Turn = Players.isPlayer1Turn,
            Player1Score = Players.player1Score,
            Player2Score = Players.player2Score,
            Player1Color = Players.player1Color.ToString(),
            Player2Color = Players.player2Color.ToString(),

            // Timer state
            IsTimerRunning = SquareGridShape.gameTimer?.IsRunning ?? false,
            CurrentTimerDuration = MainWindow.CurrentTimerDuration,

            // Bot state
            IsBotEnabled = Bot.isPlayer2Bot

        };

        // Save the game state to a file
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Game Save Files (*.dotsave)|*.dotsave",
            DefaultExt = "dotsave",
            Title = "Save Game"
        };

        // Show the save file dialog
        if (saveFileDialog.ShowDialog() == true)
        {
            string json = JsonConvert.SerializeObject(gameState);
            File.WriteAllText(saveFileDialog.FileName, json);
        }
    }

    // Convert the line array to a state array
    private static LineState[,] ConvertLinesToState(Line[,] lines)
    {
        int rows = lines.GetLength(0);
        int cols = lines.GetLength(1);
        var lineStates = new LineState[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (lines[i, j] != null)
                {
                    lineStates[i, j] = new LineState
                    {
                        IsSelected = (string)lines[i, j].Tag == "selected",
                        Color = lines[i, j].Stroke.ToString()
                    };
                }
            }
        }

        return lineStates;
    }


    //public static GameState LoadGame(string filePath)
    //{
    //    string json = File.ReadAllText(filePath);
    //    return JsonConvert.DeserializeObject<GameState>(json);
    //}



    // Load a saved game state from a file------------------------------------------------------------------------------------------------
    public static void LoadGame(MainWindow mainWindow)
    {
        // Show the open file dialog
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Game Save Files (*.dotsave)|*.dotsave",
            Title = "Load Game"
        };

        // Load the game state from the file
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var gameState = JsonConvert.DeserializeObject<GameState>(json);

                // Load grid size and create base grid
                SquareGridShape.UpdateGridSize(gameState.GridSize);
                SquareGridShape.CreateGrid();

                // Restore player states
                Players.isPlayer1Turn = gameState.IsPlayer1Turn;
                Players.player1Score = gameState.Player1Score;
                Players.player2Score = gameState.Player2Score;

                // Update score displays
                Players.player1ScoreText.Text = $"Player 1: {gameState.Player1Score}";
                Players.player2ScoreText.Text = $"Player 2: {gameState.Player2Score}";

                // Restore player colors
                Players.player1Color = (SolidColorBrush)new BrushConverter().ConvertFromString(gameState.Player1Color);
                Players.player2Color = (SolidColorBrush)new BrushConverter().ConvertFromString(gameState.Player2Color);

                // Restore completed boxes
                SquareGridShape.CompletedBoxes = gameState.CompletedBoxes;
                RestoreCompletedBoxes(gameState);

                // Restore line states
                RestoreLines(gameState.HorizontalLines, SquareGridShape.HorizontalLines);
                RestoreLines(gameState.VerticalLines, SquareGridShape.VerticalLines);

                // Update turn indicator
                Players.UpdatePlayerTurnIndicator();

                // Restore timer state if it was running
                if (gameState.IsTimerRunning)
                {
                    MainWindow.CurrentTimerDuration = gameState.CurrentTimerDuration;
                    SquareGridShape.gameTimer?.StartTimer(gameState.CurrentTimerDuration);
                }

                // Restore bot state
                if (gameState.IsBotEnabled)
                {
                    Bot.EnableBot();
                }
                else
                {
                    Bot.DisableBot();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private static void RestoreLines(LineState[,] savedLines, Line[,] currentLines)
    {
        int rows = savedLines.GetLength(0);
        int cols = savedLines.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (savedLines[i, j] != null && currentLines[i, j] != null)
                {
                    if (savedLines[i, j].IsSelected)
                    {
                        var line = currentLines[i, j];
                        line.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(savedLines[i, j].Color);
                        line.StrokeThickness = 4;
                        line.Tag = "selected";
                    }
                }
            }
        }
    }

    private static void RestoreCompletedBoxes(GameState gameState)
    {
        for (int row = 0; row < gameState.GridSize; row++)
        {
            for (int col = 0; col < gameState.GridSize; col++)
            {
                if (gameState.CompletedBoxes[row, col])
                {
                    // Determine which player completed this box by checking the surrounding lines
                    var color = DetermineBoxColor(gameState, row, col);
                    Rectangle box = new Rectangle
                    {
                        Width = SquareGridShape.Spacing,
                        Height = SquareGridShape.Spacing,
                        Fill = new SolidColorBrush(color.Color) { Opacity = 0.3 }
                    };
                    Canvas.SetLeft(box, col * SquareGridShape.Spacing);
                    Canvas.SetTop(box, row * SquareGridShape.Spacing);
                    SquareGridShape.GameCanvas.Children.Add(box);
                }
            }
        }
    }

    private static SolidColorBrush DetermineBoxColor(GameState gameState, int row, int col)
    {
        // Check the color of any of the surrounding lines to determine the player
        var horizontalLine = gameState.HorizontalLines[row, col];
        var verticalLine = gameState.VerticalLines[row, col];

        string colorString = horizontalLine?.Color ?? verticalLine?.Color;
        return (SolidColorBrush)new BrushConverter().ConvertFromString(colorString);
    }
}