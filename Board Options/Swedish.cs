using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;


// Outer lines already drawn

namespace Dots_and_Lines_Game.GridShapes
{
    public static class Swedish
    {
        // Method to initialize Swedish grid style
        public static void InitializeSwedishGrid()
        {
            // Create the base grid first using existing functionality
            SquareGridShape.CreateGrid();

            // Select all perimeter lines
            SelectPerimeterLines();
        }

        private static void SelectPerimeterLines()
        {
            int size = SquareGridShape.CurrentGridSize;

            // Select top horizontal lines
            for (int col = 0; col < size; col++)
            {
                Line topLine = SquareGridShape.HorizontalLines[0, col];
                topLine.Stroke = Brushes.Black;
                topLine.StrokeThickness = 8;
                topLine.Tag = "selected";
            }

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

            // Select rightmost vertical lines
            for (int row = 0; row < size; row++)
            {
                Line rightLine = SquareGridShape.VerticalLines[row, size];
                rightLine.Stroke = Brushes.Black;
                rightLine.StrokeThickness = 8;
                rightLine.Tag = "selected";
            }

            // Check for any boxes completed by the perimeter lines
            SquareGridShape.CheckForCompletedBoxes();
        }
    }
}
