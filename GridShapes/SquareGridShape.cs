using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Dots_and_Lines_Game.GridShapes
{
    public static class SquareGridShape
    {
        public const double DotSize = 10;
        public const double Spacing = 100;

        // Grid size and arrays to store lines and completed boxes
        public static int CurrentGridSize { get; private set; } = 2;
        public static Line[,] ?HorizontalLines { get; private set; }
        public static Line[,] ?VerticalLines { get; private set; }
        public static bool[,] ?CompletedBoxes { get; set; }
        public static Canvas ?GameCanvas { get; set; }

        // Reference to the game timer
        public static GameTimer ?gameTimer;

        
        // Reference to the MainWindow
        private static MainWindow mainWindow;

        // Method to set the MainWindow reference
        public static void SetMainWindow(MainWindow window)
        {
            mainWindow = window;
        }

        
        // Create the base grid with dots and lines
        public static void CreateGrid()
        {
            if (Players.mainGrid.Children.Count > 0)
            {
                Players.mainGrid.Children.Clear();
            }

            GameCanvas = new Canvas
            {
                Width = (CurrentGridSize + 1) * Spacing, // + 1 to account for the extra dots the form a grid
                Height = (CurrentGridSize + 1) * Spacing, 
                Background = Brushes.White
            };

            

            CreateDots();
            CreateBaseLines();

            Players.mainGrid.Children.Add(GameCanvas);
        }

        // Create dots for the grid
        public static void CreateDots()
        {
            for (int row = 0; row <= CurrentGridSize; row++)
            {
                for (int col = 0; col <= CurrentGridSize; col++)
                {
                    Ellipse dot = new Ellipse
                    {
                        Width = DotSize,
                        Height = DotSize,
                        Fill = Brushes.Black
                    };

                    Canvas.SetLeft(dot, col * Spacing - DotSize / 2); // horizontally center the dot
                    Canvas.SetTop(dot, row * Spacing - DotSize / 2);  // vertically center the dot
                    GameCanvas.Children.Add(dot);
                }
            }
        }

        // Create the base lines for the grid
        public static void CreateBaseLines()
        {
            // Draw clickable lines
            for (int row = 0; row <= CurrentGridSize; row++)
            {
                for (int col = 0; col < CurrentGridSize; col++)
                {
                    // Horizontal lines
                    Line hLine = CreateLine(
                        col * Spacing, row * Spacing,
                        (col + 1) * Spacing, row * Spacing
                    );
                    GameCanvas.Children.Add(hLine);
                    HorizontalLines[row, col] = hLine;

                    // Vertical lines (except for last row)
                    if (row < CurrentGridSize)
                    {
                        Line vLine = CreateLine(
                            col * Spacing, row * Spacing,
                            col * Spacing, (row + 1) * Spacing
                        );
                        GameCanvas.Children.Add(vLine);
                        VerticalLines[row, col] = vLine;
                    }
                }
            }

            // Add vertical lines for last column
            for (int row = 0; row < CurrentGridSize; row++)
            {
                Line vLine = CreateLine(
                    CurrentGridSize * Spacing, row * Spacing,
                    CurrentGridSize * Spacing, (row + 1) * Spacing
                );
                GameCanvas.Children.Add(vLine);
                VerticalLines[row, CurrentGridSize] = vLine;
            }
        }

        // Create a line with default properties
        public static Line CreateLine(double x1, double y1, double x2, double y2, MouseButtonEventHandler clickHandler = null)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.LightGray,
                StrokeThickness = 10,
                Tag = "unselected"
            };
            line.MouseLeftButtonDown += clickHandler ?? Line_Click; 
            return line;
        }


        // Handle line click events
        public static void Line_Click(object sender, MouseButtonEventArgs e)
        {
            Line line = (Line)sender;
            if ((string)line.Tag == "unselected")
            {
                // Save the current state of completed boxes and player turn 
                bool[,] completedBoxesState = (bool[,])CompletedBoxes.Clone();
                UndoRedo.moveHistory.Push(new MoveState(line, completedBoxesState, Players.isPlayer1Turn));

                // Draw the new line with player color
                // If it is Player 1's turn, use Player 1's color; otherwise, use Player 2's color
                line.Stroke = Players.isPlayer1Turn ? Players.player1Color : Players.player2Color;
                line.StrokeThickness = 4;
                line.Tag = "selected";

                
                bool boxCompleted = CheckForCompletedBoxes();

                if (!boxCompleted)
                {
                    Players.isPlayer1Turn = !Players.isPlayer1Turn;
                    Players.UpdatePlayerTurnIndicator();

                    // If it's player 2's turn and bot is enabled, make bot move
                    if (!Players.isPlayer1Turn && Bot.isPlayer2Bot)
                    {
                        Bot.MakeBotMoveWithContinuation();
                    }
                }
                else if (!Players.isPlayer1Turn && Bot.isPlayer2Bot)
                {
                    // If bot completed a box, make another move
                    Bot.MakeBotMoveWithContinuation();
                }

                // Clear redo history when a new move is made
                UndoRedo.ClearRedoHistory();

                // Always restart the timer after a valid move
                if (gameTimer != null && gameTimer.IsRunning)
                {
                    gameTimer.RestartTimer();
                }

                // Check if the game is over
                if (IsGameOver())
                {
                    gameTimer?.StopTimer();

                    // Display winner message
                    string winnerMessage = Players.GetWinnerMessage();
                    MessageBoxResult result = MessageBox.Show(winnerMessage + "\n\nPlay again?", "Game Over", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        mainWindow.InitializeGame(CurrentGridSize);
                    }
                    else
                    {
                        mainWindow.Close();
                    }
                }
            }
        }


        // Check if a box is completed
        public static bool CheckForCompletedBoxes()
        {
            bool boxCompleted = false;

            for (int row = 0; row < CurrentGridSize; row++)
            {
                for (int col = 0; col < CurrentGridSize; col++)
                {
                    // Check if the current box is not already completed and if it is now complete
                    if (!CompletedBoxes[row, col] && IsBoxComplete(row, col))
                    {
                        CompletedBoxes[row, col] = true;
                        boxCompleted = true;
                        Players.UpdatePlayerScore();
                        AddCompletedBox(row, col);
                    }
                }
            }

            return boxCompleted;
        }

        // Add a semi-transparent box to indicate a completed box
        public static void AddCompletedBox(int row, int col)
        {
            Rectangle box = new Rectangle
            {
                Width = Spacing,
                Height = Spacing,
                Fill = new SolidColorBrush(
                    (Players.isPlayer1Turn ? Players.player1Color : Players.player2Color).Color)
                { Opacity = 0.3 }
            };
            Canvas.SetLeft(box, col * Spacing);
            Canvas.SetTop(box, row * Spacing);
            GameCanvas.Children.Add(box);
        }

        // Check if all lines around a box are selected
        public static bool IsBoxComplete(int row, int col)
        {
            return IsLineSelected(HorizontalLines[row, col]) &&      // Top
                   IsLineSelected(HorizontalLines[row + 1, col]) &&  // Bottom
                   IsLineSelected(VerticalLines[row, col]) &&        // Left
                   IsLineSelected(VerticalLines[row, col + 1]);      // Right
        }

        // Check if a line is selected
        public static bool IsLineSelected(Line line)
        {
            return (string)line.Tag == "selected";
        }


        // Update the grid size and initialize the arrays
        public static void UpdateGridSize(int size)
        {
            CurrentGridSize = size;
            HorizontalLines = new Line[size + 1, size];
            VerticalLines = new Line[size, size + 1];
            CompletedBoxes = new bool[size, size];

            // Make sure the timer continues if it was running
            if (gameTimer != null && gameTimer.IsRunning)
            {
                gameTimer.RestartTimer();
            }
        }


        // Check if the game is over
        public static bool IsGameOver()
        {
            for (int row = 0; row < CurrentGridSize; row++)
            {
                for (int col = 0; col < CurrentGridSize; col++)
                {
                    // Check if the current box is not completed and is not complete
                    if (!CompletedBoxes[row, col] && !IsBoxComplete(row, col))
                    {
                        // If any box is not completed and not complete, the game is not over
                        return false;
                    }
                }
            }
            // If all boxes are completed or complete, the game is over
            return true;
        }
    }
}