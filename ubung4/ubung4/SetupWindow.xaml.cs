using System.Windows;

namespace FittsExercise
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public uint ExperimentId { get; set; }

        public uint NbrOfTasks { get; set; }

        public bool ResetMousePos { get; set; }

        public bool Precuing { get; set; }

        public SetupWindow(uint _experimentId, uint _nbrOfTasks, bool _resetMousePos, bool _precuing)
        {
            ExperimentId = _experimentId;
            NbrOfTasks = _nbrOfTasks;
            ResetMousePos = _resetMousePos;
            Precuing = _precuing;

            InitializeComponent();

            Loaded += WpfUtil.Util.Setup_MinWidth_MinHeight;
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
