using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FittsExercise
{
    /// <summary>
    /// Experiment addressing Fitt's law.
    /// 
    /// Measures time to acquire ("click") a randomly placed button ("target").
    /// 
    /// Precuing (optionally): The target is displayed before a measurement starts. 
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random myRandomizer;
        private uint counter = 0;

        private uint experimentId = 1;
        // number of measurements per experiment
        private uint nbrOfTasks = 2;
        // mouse pointer automatically set to start button once target is clicked
        private bool resetMousePos = false;
        // display new target once current target is clicked 
        private bool precuing = true;

        // flag, indicates if the start button was pressed (...and the hit button should be released)
        private bool doAcquireTarget = false;

        // "click me!" brush
        readonly Brush brClickMe = Brushes.GreenYellow;
        readonly Brush brDontClickMe = Brushes.White;

        // number of errors across all tasks of this experiment session
        private uint sessionErrorCount = 0;
        // number of errors observed during the current target acquisition task
        private uint taskErrorCount = 0;

        // Create a logger to record target acquisition data
        Logger? log;

        /// <summary>
        /// Construct the main window. Initializes all UI elements.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.myRandomizer = new Random();

            // Compute random id for the upcoming experiment
            this.experimentId = (uint)this.myRandomizer.Next(1, 10000);

            // Highlight start button
            this.bnStart.Background = brClickMe;

            SetupWindow setupWindow = new SetupWindow(experimentId, nbrOfTasks, resetMousePos, precuing);
            setupWindow.ShowDialog();
            if (setupWindow.DialogResult.HasValue && setupWindow.DialogResult.Value)
            {
                this.experimentId = setupWindow.ExperimentId;
                this.nbrOfTasks = setupWindow.NbrOfTasks;
                this.resetMousePos = setupWindow.ResetMousePos;
                this.precuing = setupWindow.Precuing;

                // Execute if setup was confirmed
                log = new Logger(nbrOfTasks);
                Loaded += delegate (object sender, RoutedEventArgs e) { CreateTarget(); };
            }
            else
            {
                // Executed if setup was canceled
                Close();
            }
        }

        /// <summary>
        /// Repositions and resizes target.
        /// 
        /// Repositioning is continued until a valid position is found - e.g., one not
        /// hiding the start button.
        /// </summary>
        public void CreateTarget()
        {
            // Hide target if precuing is disabled
            if (!this.precuing) this.Target.Visibility = Visibility.Hidden;

            // Target of random size
            this.Target.Width = this.Target.Height = this.myRandomizer.Next(5, 100);
            do
            {
                // Assign a random position to target
                this.Target.Margin = new Thickness(this.myRandomizer.Next(0, 500), this.myRandomizer.Next(0, 500), 0, 0);
                // Ensure that margins and sizes are up-to-date
                UpdateLayout();
            } while (TargetPositionInvalid());
        }

        /// <summary>
        /// Validates the target's current position.
        /// 
        /// Used for positioning the target at the beginning of an experiment, and for collision
        /// detection in case of animated targets.
        /// </summary>
        /// <returns></returns>
        private bool TargetPositionInvalid()
        {
            Rect rTarget = BoundsRelativeTo(Target, Canvas);
            Rect rStart = BoundsRelativeTo(bnStart, Canvas);
            return rTarget.IntersectsWith(rStart);
        }
        public static Rect BoundsRelativeTo(FrameworkElement element, Visual relativeTo)
        {
            return element.TransformToVisual(relativeTo).TransformBounds(new Rect(element.RenderSize));
        }
        public Point GetCenter(FrameworkElement el)
        {
            return new Point(el.Margin.Left + el.ActualWidth / 2, el.Margin.Top + el.ActualHeight / 2);
        }

        private void OnStartClicked(object sender, RoutedEventArgs e)
        {
            if (this.doAcquireTarget)
            {
                // start was clicked, but the user should have clicked target
                this.taskErrorCount++;
            }
            else
            {
                // Start counting errors
                this.taskErrorCount = 0;

                // Record target acquisition setup
                this.log?.BeginLogEntry(counter, Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y, (uint)this.Target.Width, (uint)this.Target.Height);

                // Unlock the target button
                this.doAcquireTarget = true;
                this.bnStart.Background = this.brDontClickMe;
                this.Target.Fill = this.brClickMe;
                this.Target.Visibility = Visibility.Visible;
            }
            // prevent error count to be increased by window mouseLeftButtonDown handler
            e.Handled = true;
        }

        private void OnTargetClicked(object sender, MouseButtonEventArgs e)
        {
            if (this.doAcquireTarget)
            {
                // enforce need to click start button
                this.doAcquireTarget = false;

                // Update overall error count
                this.sessionErrorCount += this.taskErrorCount;

                // Finalize target acquisition setup
                this.log?.EndLogEntry(counter, Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y, taskErrorCount);

                // decide on what to do in the next iteration
                this.counter++;
                if (this.counter < this.nbrOfTasks)
                {
                    CreateTarget();

                    // highlight the button to be clicked next
                    bnStart.Background = brClickMe;
                    Target.Fill = brDontClickMe;

                    if (resetMousePos) SetMouseCursorPos(MainWindowInstance.Width / 2, MainWindowInstance.Height / 2);
                }
                else
                {
                    // experiment accomplished
                    this.EndExperiment();
                }
            }
            // prevent error count to be increased by window mouseLeftButtonDown handler
            e.Handled = true;
        }

        private void WindowClicked(object sender, MouseButtonEventArgs e)
        {
            // Clicked on window instead of target
            if (this.doAcquireTarget)
            {
                this.taskErrorCount++;
            }
        }

        private void SetMouseCursorPos(double x, double y)
        {
            Point pUpperLeftCorner = new Point(this.Left + x, this.Top + y);
            // Compute device-dependant (pixel) coordinates from device-independant coordinates
            Point pPixelCoordinates = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.Transform(pUpperLeftCorner);
            // Move mouse cursor
            NativeMethods.SetCursorPos((int)pPixelCoordinates.X, (int)pPixelCoordinates.Y);
        }

        protected void EndExperiment()
        {
            this.bnStart.Visibility = Visibility.Hidden;
            this.Target.Visibility = Visibility.Hidden;

            // Open a message box, which displays the number of errors occurred in this experiment
            MessageBox.Show("Number of errors: " + sessionErrorCount, "Summary");

            // Record this session's data in a file (name includes session id)
            string? fileName = "testResults_" + resetMousePos + "_" + precuing + "_" + experimentId;
            string fileNameExt = ".csv";
            fileName += fileNameExt;

            // Use fileName and fileNameExt to configure a save dialog, which allows for confirming or editing the filename (default folder is ./bin/debug)
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = fileName,
                DefaultExt = fileNameExt,
                Filter = "Text Files (*.prn, *.txt, *.csv)|*.csv" // Filter files by extension
            };
            fileName = dlg.ShowDialog() == true ? dlg.FileName : null;

            // Process dialog result
            if (fileName != null)
            {
                log?.Save(fileName, experimentId, resetMousePos, precuing);
            }

            // Close the application
            Close();
        }
    }

    internal static class NativeMethods
    {
        // DLL für API-Funktion importieren; externe Methode SetCursorPos deklarieren; eigene Methode setMouseCursorPos definieren; 
        [DllImport("user32.dll")]
        internal static extern int SetCursorPos(int x, int y);
    }
}