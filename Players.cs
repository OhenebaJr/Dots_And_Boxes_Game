using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dots_and_Lines_Game
{
    internal class Players
    {
        // Object to hold player information
        public static bool isPlayer1Turn = true;
        public static SolidColorBrush player1Color = Brushes.Blue;
        public static SolidColorBrush player2Color = Brushes.Red;
        public static int player1Score = 0;
        public static int player2Score = 0;

        public static TextBlock player1ScoreText { get; set; }
        public static TextBlock player2ScoreText { get; set; }
        public static Grid mainGrid { get; set; }
        public static TextBlock playerTurnIndicator { get; set; }


        public static void Initialize(TextBlock p1ScoreText, TextBlock p2ScoreText, TextBlock turnIndicator, Grid grid)
        {
            player1ScoreText = p1ScoreText;
            player2ScoreText = p2ScoreText;
            playerTurnIndicator = turnIndicator;
            mainGrid = grid;
        }


        public static void UpdatePlayerTurnIndicator()
        {
            if (playerTurnIndicator != null)  // Add null check
            {
                // Update the player turn indicator text and color
                playerTurnIndicator.Text = isPlayer1Turn ? "Player 1's Turn" : "Player 2's Turn";
                playerTurnIndicator.Foreground = isPlayer1Turn ? player1Color : player2Color;
            }
        }

        // Update the player score
        public static void UpdatePlayerScore()
        {
            if (isPlayer1Turn)
            {
                player1Score++;
                player1ScoreText.Text = $"Player 1: {player1Score}";
                player1ScoreText.Foreground = player1Color;
            }
            else
            {
                player2Score++;
                player2ScoreText.Text = $"Player 2: {player2Score}";
                player2ScoreText.Foreground = player2Color;
            }
        }

        public static void SetPlayer1Color(string color)
        {
            switch (color)
            {
                case "Red":
                    player1Color = Brushes.Red;
                    break;
                case "Yellow":
                    player1Color = Brushes.Yellow;
                    break;
                case "Green":
                    player1Color = Brushes.Green;
                    break;
                case "Blue":
                    player1Color = Brushes.Blue;
                    break;
                case "Black":
                    player1Color = Brushes.Black;
                    break;
            }
            UpdatePlayerTurnIndicator();
        }

        public static void SetPlayer2Color(string color)
        {
            switch (color)
            {
                case "Red":
                    player2Color = Brushes.Red;
                    break;
                case "Yellow":
                    player2Color = Brushes.Yellow;
                    break;
                case "Green":
                    player2Color = Brushes.Green;
                    break;
                case "Blue":
                    player2Color = Brushes.Blue;
                    break;
                case "Black":
                    player2Color = Brushes.Black;
                    break;
            }
            UpdatePlayerTurnIndicator();
        }

        // Get winner message
        public static string GetWinnerMessage()
        {
            if (player1Score > player2Score)
            {
                return "Player 1 wins!";
            }
            else if (player2Score > player1Score)
            {
                return "Player 2 wins!";
            }
            else
            {
                return "It's a tie!";
            }
        }

        
    }
}
