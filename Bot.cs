using Dots_and_Lines_Game.GridShapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;

namespace Dots_and_Lines_Game
{
    internal class Bot
    {
        // set player 2 to be the bot
        public static bool isPlayer2Bot = false; // Start as false by default

        private static Random random = new Random();

        // Reference to the MainWindow
        private static MainWindow mainWindow;

        // Method to set the MainWindow reference
        public static void SetMainWindow(MainWindow window)
        {
            mainWindow = window;
        }

        // Method to enable bot as player 2
        public static void EnableBot()
        {
            isPlayer2Bot = true;
            Players.player2Color = Brushes.Purple; // Change player 2 now bot's color to Purple
            mainWindow.player2ScoreText.Text = "Player 2 (Bot): 0"; // Append " (Bot)" to player 2's name
        }

        // Method to disable bot
        public static void DisableBot()
        {
            isPlayer2Bot = false;
            mainWindow.player2ScoreText.Text = "Player 2: 0"; // Remove " (Bot)" from player 2's name
        }

        // Bot logic - should be called after player 1's turn
        public static void MakeBotMove()
        {
            if (isPlayer2Bot && !Players.isPlayer1Turn)
            {
                bool moveSuccess = SelectLastLine(); // Try to complete a box first

                if (!moveSuccess) // If SelectLastLine doesn't find a move
                {
                    SelectRandomLine();
                }
            }
        }

        // New method to handle bot moves with continuation
        public static async void MakeBotMoveWithContinuation()
        {
            await Task.Delay(500); // Initial delay before first move

            bool continueMoving = true;
            do
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MakeBotMove();

                    // Check if a box was completed and it's still bot's turn
                    continueMoving = !Players.isPlayer1Turn && isPlayer2Bot;
                });

                if (continueMoving)
                {
                    await Task.Delay(500); // Delay between consecutive moves
                }
            } while (continueMoving);
        }

        private static void SelectRandomLine()
        {
            // Get all the lines that are not yet drawn
            var availableLines = SquareGridShape.HorizontalLines.Cast<Line>()
                .Concat(SquareGridShape.VerticalLines.Cast<Line>())
                .Where(l => !SquareGridShape.IsLineSelected(l)).ToList();

            if (availableLines.Count > 0)
            {
                // Randomly select a line to draw
                var randomLine = availableLines[random.Next(availableLines.Count)];

                // Draw the line
                randomLine.Stroke = Brushes.Purple; // Use purple color for bot's lines
                SquareGridShape.Line_Click(randomLine, null); // Simulate the click event
            }
        }

        private static bool SelectLastLine()
        {
            // Check each potential box in the grid
            for (int row = 0; row < SquareGridShape.CurrentGridSize; row++)
            {
                for (int col = 0; col < SquareGridShape.CurrentGridSize; col++)
                {
                    if (!SquareGridShape.CompletedBoxes[row, col])  // Only check uncompleted boxes
                    {
                        // Count selected lines around this box
                        int selectedLines = 0;
                        Line missingLine = null;

                        // Check top horizontal line
                        if (SquareGridShape.IsLineSelected(SquareGridShape.HorizontalLines[row, col]))
                            selectedLines++;
                        else
                            missingLine = SquareGridShape.HorizontalLines[row, col];

                        // Check bottom horizontal line
                        if (SquareGridShape.IsLineSelected(SquareGridShape.HorizontalLines[row + 1, col]))
                            selectedLines++;
                        else if (missingLine == null)
                            missingLine = SquareGridShape.HorizontalLines[row + 1, col];

                        // Check left vertical line
                        if (SquareGridShape.IsLineSelected(SquareGridShape.VerticalLines[row, col]))
                            selectedLines++;
                        else if (missingLine == null)
                            missingLine = SquareGridShape.VerticalLines[row, col];

                        // Check right vertical line
                        if (SquareGridShape.IsLineSelected(SquareGridShape.VerticalLines[row, col + 1]))
                            selectedLines++;
                        else if (missingLine == null)
                            missingLine = SquareGridShape.VerticalLines[row, col + 1];

                        // If we found a box with exactly 3 lines drawn
                        if (selectedLines == 3 && missingLine != null && (string)missingLine.Tag == "unselected")
                        {
                            // Draw the line
                            missingLine.Stroke = Players.player2Color;
                            SquareGridShape.Line_Click(missingLine, null);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}