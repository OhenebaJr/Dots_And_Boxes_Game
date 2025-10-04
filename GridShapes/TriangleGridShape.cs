using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.Windows; 

namespace Dots_and_Lines_Game.GridShapes
{
    public static class TriangleGridShape
    {
        public const double DotSize = SquareGridShape.DotSize;
        public const double Spacing = SquareGridShape.Spacing;

        public static int CurrentGridSize { get; private set; } = 2;
        public static Line[,] DiagonalLinesLeft { get; private set; }    // Lines going up-left to down-right
        public static Line[,] DiagonalLinesRight { get; private set; }   // Lines going up-right to down-left
        public static Line[,] HorizontalLines { get; private set; }      // Horizontal base lines
        public static bool[,] CompletedTriangles { get; private set; }   // Tracks both up and down triangles
        public static Canvas GameCanvas { get; set; }

        public static void UpdateGridSize(int size)
        {
            CurrentGridSize = size;
            DiagonalLinesLeft = new Line[size + 1, size + 1];
            DiagonalLinesRight = new Line[size + 1, size + 1];
            HorizontalLines = new Line[size + 1, size];
            CompletedTriangles = new bool[size * 2, size]; // *2 because each row has up and down triangles
        }

        public static void CreateBaseGrid()
        {
            if (Players.mainGrid.Children.Count > 0)
            {
                Players.mainGrid.Children.Clear();
            }

            GameCanvas = new Canvas
            {
                Width = (CurrentGridSize + 1) * Spacing,
                Height = (CurrentGridSize + 1) * Spacing,
                Background = Brushes.White
            };

            CreateDots();
            CreateBaseLines();

            Players.mainGrid.Children.Add(GameCanvas);
        }

        public static void CreateDots()
        {
            // Reuse SquareGridShape's dot creation as it's the same pattern
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

                    Canvas.SetLeft(dot, col * Spacing - DotSize / 2);
                    Canvas.SetTop(dot, row * Spacing - DotSize / 2);
                    GameCanvas.Children.Add(dot);
                }
            }
        }

        public static void CreateBaseLines()
        {
            // Create all possible lines for the triangular grid
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

                    // Diagonal lines (both left and right) if not at last row
                    if (row < CurrentGridSize)
                    {
                        // Diagonal line going down-right
                        Line diagLineRight = CreateLine(
                            col * Spacing, row * Spacing,
                            (col + 1) * Spacing, (row + 1) * Spacing
                        );
                        GameCanvas.Children.Add(diagLineRight);
                        DiagonalLinesRight[row, col] = diagLineRight;

                        // Diagonal line going down-left
                        Line diagLineLeft = CreateLine(
                            (col + 1) * Spacing, row * Spacing,
                            col * Spacing, (row + 1) * Spacing
                        );
                        GameCanvas.Children.Add(diagLineLeft);
                        DiagonalLinesLeft[row, col] = diagLineLeft;
                    }
                }
            }
        }

        public static Line CreateLine(double x1, double y1, double x2, double y2)
        {
            return SquareGridShape.CreateLine(x1, y1, x2, y2, Line_Click);
        }

        public static void CreateGrid()
        {
            CreateBaseGrid();
        }

        private static void Line_Click(object sender, MouseButtonEventArgs e)
        {
            Line line = (Line)sender;
            if ((string)line.Tag == "unselected")
            {
                line.Stroke = Players.isPlayer1Turn ? Players.player1Color : Players.player2Color;
                line.StrokeThickness = 4;
                line.Tag = "selected";

                bool triangleCompleted = CheckForCompletedTriangles();

                if (!triangleCompleted)
                {
                    Players.isPlayer1Turn = !Players.isPlayer1Turn;
                    Players.UpdatePlayerTurnIndicator();
                }
            }
        }

        public static bool CheckForCompletedTriangles()
        {
            bool triangleCompleted = false;

            for (int row = 0; row < CurrentGridSize; row++)
            {
                for (int col = 0; col < CurrentGridSize; col++)
                {
                    // Check upward-pointing triangle
                    int upTriangleIndex = row * 2;
                    if (!CompletedTriangles[upTriangleIndex, col] && IsUpwardTriangleComplete(row, col))
                    {
                        CompletedTriangles[upTriangleIndex, col] = true;
                        triangleCompleted = true;
                        UpdatePlayerScore();
                        AddCompletedTriangle(row, col, true);
                    }

                    // Check downward-pointing triangle
                    int downTriangleIndex = row * 2 + 1;
                    if (!CompletedTriangles[downTriangleIndex, col] && IsDownwardTriangleComplete(row, col))
                    {
                        CompletedTriangles[downTriangleIndex, col] = true;
                        triangleCompleted = true;
                        UpdatePlayerScore();
                        AddCompletedTriangle(row, col, false);
                    }
                }
            }

            return triangleCompleted;
        }

        private static bool IsUpwardTriangleComplete(int row, int col)
        {
            return IsLineSelected(DiagonalLinesLeft[row, col]) &&
                   IsLineSelected(DiagonalLinesRight[row, col]) &&
                   IsLineSelected(HorizontalLines[row, col]);
        }

        private static bool IsDownwardTriangleComplete(int row, int col)
        {
            return IsLineSelected(DiagonalLinesLeft[row, col]) &&
                   IsLineSelected(DiagonalLinesRight[row, col]) &&
                   IsLineSelected(HorizontalLines[row + 1, col]);
        }

        private static void UpdatePlayerScore()
        {
            // Reuse SquareGridShape's score update logic
            Players.UpdatePlayerScore();
        }

        private static void AddCompletedTriangle(int row, int col, bool isUpward)
        {
            // Create a triangle shape instead of a rectangle
            Polygon triangle = new Polygon();

            Point[] points;
            if (isUpward)
            {
                points = new[]
                {
                    new Point(col * Spacing, row * Spacing),
                    new Point((col + 1) * Spacing, row * Spacing),
                    new Point((col + 0.5) * Spacing, (row + 1) * Spacing)
                };
            }
            else
            {
                points = new[]
                {
                    new Point(col * Spacing, (row + 1) * Spacing),
                    new Point((col + 1) * Spacing, (row + 1) * Spacing),
                    new Point((col + 0.5) * Spacing, row * Spacing)
                };
            }

            triangle.Points = new PointCollection(points);
            triangle.Fill = new SolidColorBrush(
                (Players.isPlayer1Turn ? Players.player1Color : Players.player2Color).Color)
            { Opacity = 0.3 };

            GameCanvas.Children.Add(triangle);
        }

        private static bool IsLineSelected(Line line)
        {
            return (string)line.Tag == "selected";
        }
    }
}