using System;
using System.Windows.Threading;
using System.Media;

namespace Dots_and_Lines_Game
{
    public class GameTimer
    {
        // Timer object
        private readonly DispatcherTimer timer;
        private int timeLeft;
        private Action<int> updateTimerDisplay;
        private Action switchPlayer;
        private int _currentTimerDuration;


        // Returns true if timer is currently running
        public bool IsRunning => timer.IsEnabled;

        // Constructor
        public GameTimer(Action<int> updateDisplayCallback, Action switchPlayerCallback)
        {
            // Initialize the timer with callbacks
            updateTimerDisplay = updateDisplayCallback;
            switchPlayer = switchPlayerCallback;

            // Initialize timer
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;

        }

        // Timer tick event
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                updateTimerDisplay(timeLeft);

                if (timeLeft == 0)
                {
                    StopTimer();
                    Console.Beep(800, 200);
                    switchPlayer?.Invoke();
                }
            }
        }

        // Start the timer with a given duration
        public void StartTimer(int seconds)
        {
            _currentTimerDuration = seconds; // Store the duration
            // Start a new timer countdown
            timeLeft = seconds;
            updateTimerDisplay(timeLeft);
            timer.Start();
        }

        // Restart the timer with the same duration
        public void RestartTimer()
        {
            if (IsRunning)
            {
                StopTimer();
                StartTimer(_currentTimerDuration);
            }
        }

        // Stop the timer
        public void StopTimer()
        {
            timer.Stop();
            updateTimerDisplay(0);
        }

        
    }
}