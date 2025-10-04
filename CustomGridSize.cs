using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Dots_and_Lines_Game
{
    public class CustomGridSize : Window
    {
        private TextBox sizeInput;
        private int result;

        public CustomGridSize()
        {
            Title = "Custom Grid Size";
            Width = 300;
            Height = 150;

            // Center the window on the owner
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Create a grid to hold the content
            var grid = new Grid();
            Content = grid;

            // Create a stack panel with margin to hold the input controls
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            grid.Children.Add(stackPanel);

            // Add a text block to prompt the user for input
            stackPanel.Children.Add(new TextBlock { Text = "Enter grid size (2-10):", Margin = new Thickness(0, 0, 0, 5) });

            // Create a text box for user input
            sizeInput = new TextBox { Margin = new Thickness(0, 0, 0, 10) };
            stackPanel.Children.Add(sizeInput);

            // Create a panel to hold the buttons
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var okButton = new Button { Content = "OK", Width = 60, Margin = new Thickness(0, 0, 5, 0) };
            var cancelButton = new Button { Content = "Cancel", Width = 60 };

            // Add event handlers for the buttons
            okButton.Click += OkButton_Click;
            cancelButton.Click += (s, e) => Close();

            // Add the buttons to the button panel
            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            // Add the button panel to the stack panel
            stackPanel.Children.Add(buttonPanel);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Try to parse the user input as an integer
            if (int.TryParse(sizeInput.Text, out int size) && size >= 2 && size <= 10)
            {
                // If the input is valid, set the result and close the dialog with a positive result
                result = size;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a number from 2 to 10.", "Invalid Input");
            }
        }

        // Method to show the dialog and return the result
        public int? ShowDialog(Window owner)
        {
            // Set the owner of the dialog
            Owner = owner;

            // Show the dialog and return the result
            return ShowDialog() == true ? result : null;
        }
    }
}


