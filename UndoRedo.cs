using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dots_and_Lines_Game.GridShapes
{
    // Class to store the state of a move so it can be undone/redone
    public class MoveState
    {
        // Properties to store the state of the move
        public Line Line { get; set; }
        public bool[,] CompletedBoxesState { get; set; }
        public bool IsPlayer1Turn { get; set; }

        // Constructor to initialize the move state
        public MoveState(Line line, bool[,] completedBoxesState, bool isPlayer1Turn)
        {
            Line = line;
            CompletedBoxesState = (bool[,])completedBoxesState.Clone();
            IsPlayer1Turn = Players.isPlayer1Turn;

        }
    }

    internal class UndoRedo
    {
        // Stacks to store the move history and redo history
        public static Stack<MoveState> moveHistory = new Stack<MoveState>();
        public static Stack<MoveState> redoHistory = new Stack<MoveState>();

        // Game timer object
        public static GameTimer? gameTimer;



        public static void UndoLastMove()
        {
            if (moveHistory.Count > 0)
            {
                MoveState lastMove = moveHistory.Pop();
                Line lastLine = lastMove.Line;
                lastLine.Stroke = Brushes.LightGray;
                lastLine.StrokeThickness = 10;
                lastLine.Tag = "unselected";

                // Restore the state of completed boxes
                SquareGridShape.CompletedBoxes = lastMove.CompletedBoxesState;

                // Remove the visual indication of completed boxes
                RemoveCompletedBoxes();

                // Restore the player turn state
                Players.isPlayer1Turn = lastMove.IsPlayer1Turn;
                Players.UpdatePlayerTurnIndicator();

                

                // Add to redo history
                redoHistory.Push(lastMove);
            }
        }

        public static void RedoLastMove()
        {
            if (redoHistory.Count > 0)
            {
                MoveState redoMove = redoHistory.Pop();
                Line redoLine = redoMove.Line;
                redoLine.Stroke = Players.isPlayer1Turn ? Players.player1Color : Players.player2Color;
                redoLine.StrokeThickness = 4;
                redoLine.Tag = "selected";

                // Restore the state of completed boxes
                SquareGridShape.CompletedBoxes = redoMove.CompletedBoxesState;

                // Restore the player turn state
                Players.isPlayer1Turn = redoMove.IsPlayer1Turn;
                Players.UpdatePlayerTurnIndicator();


                // Add back to move history
                moveHistory.Push(redoMove);

                bool boxCompleted = SquareGridShape.CheckForCompletedBoxes();

                if (!boxCompleted)
                {
                    Players.isPlayer1Turn = !Players.isPlayer1Turn;
                    Players.UpdatePlayerTurnIndicator();
                }

                if (gameTimer != null && gameTimer.IsRunning)
                {
                    gameTimer.RestartTimer();
                }
            }
        }



        // Method to remove visual indication of completed boxes
        public static void RemoveCompletedBoxes()
        {
            for (int i = SquareGridShape.GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (SquareGridShape.GameCanvas.Children[i] is Rectangle)
                {
                    SquareGridShape.GameCanvas.Children.RemoveAt(i);
                }
            }

            // Re-add the visual indication of completed boxes
            for (int row = 0; row < SquareGridShape.CurrentGridSize; row++)
            {
                for (int col = 0; col < SquareGridShape.CurrentGridSize; col++)
                {
                    if (SquareGridShape.CompletedBoxes[row, col])
                    {
                        SquareGridShape.AddCompletedBox(row, col);
                    }
                }
            }
        }


        // Add method to clear redo history (call this when a new move is made)
        public static void ClearRedoHistory()
        {
            redoHistory.Clear();
        }
    }


}
