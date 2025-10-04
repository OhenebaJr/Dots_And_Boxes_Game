using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;


// Left and bottom lines already drawn

namespace Dots_and_Lines_Game.GridShapes
{
    public static class Icelandic
    {
        // Method to initialize Icelandic grid style
        public static void InitializeIcelandicGrid()
        {
            // Create the base grid first using existing functionality
            SquareGridShape.CreateGrid();

            // Select left and bottom lines
            SelectLeftAndBottomLines();
        }

        private static void SelectLeftAndBottomLines()
        {
            int size = SquareGridShape.CurrentGridSize;

            // Select bottom horizontal lines
            for (int col = 0; col < size; col++)
            {
                Line bottomLine = SquareGridShape.HorizontalLines[size, col];
                bottomLine.Stroke = Brushes.Black;
                bottomLine.StrokeThickness = 8;
                bottomLine.Tag = "selected";
            }

            // Select leftmost vertical lines
            for (int row = 0; row < size; row++)
            {
                Line leftLine = SquareGridShape.VerticalLines[row, 0];
                leftLine.Stroke = Brushes.Black;
                leftLine.StrokeThickness = 8;
                leftLine.Tag = "selected";
            }

            // Check for any boxes completed by the lines
            SquareGridShape.CheckForCompletedBoxes();
        }
    }
}
