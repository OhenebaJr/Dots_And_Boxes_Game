using System;
using System.Windows;
using System.Windows.Controls;

namespace Dots_and_Lines_Game
{
    public class CustomTimer : Window
    {
        private TextBox timeInput;
        private int? result;

        public CustomTimer()
        {
            // Window settings
            Title = "Custom Timer";
            Width = 250;
            Height = 120;

            // Center the window on the owner
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            

            // Create main container
            var mainGrid = new Grid();

            // Add rows to the grid
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Create input panel
            var inputPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var label = new TextBlock
            {
                Text = "Enter seconds: ",
                Margin = new Thickness(5)
            };

            timeInput = new TextBox
            {
                Width = 50,
                Margin = new Thickness(5)
            };

            // Add input panel to the grid
            inputPanel.Children.Add(label);
            inputPanel.Children.Add(timeInput);
            Grid.SetRow(inputPanel, 0);
            mainGrid.Children.Add(inputPanel);

            // Create button panel
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 60,
                Height = 25,
                Margin = new Thickness(5),
                IsDefault = true
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 60,
                Height = 25,
                Margin = new Thickness(5),
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (int.TryParse(timeInput.Text, out int time) && time > 0)
            {
                result = time;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid positive number.", "Invalid Input");
            }
        }

        // Show dialog and return result
        public int? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog() == true ? result : null;
        }
    }
}