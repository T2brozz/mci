using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace uebung3
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        protected MyClass myObject = new MyClass();
        public MyClass MyObject
        {
            get { return myObject; }
            set { myObject = value; }
        }

        public Window1()
        {

            InitializeComponent();

            Binding myBinding = new Binding(nameof(MyObject.MyField));
            myBinding.Source = myObject;
            A.SetBinding(Label.ContentProperty, myBinding);

            D.DataContext = myObject;
        }

        private void Change(object sender, RoutedEventArgs e)
        {
            myObject.MyField = 9876543;
        }


    }

    /// <summary>
    /// A simple representation of a student's properties.
    /// </summary>
    public class MyClass : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor, creates an empty object.
        /// </summary>
        public MyClass()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise a PropertyChanged event.
		///
		/// Should be called after a value change in one of this' properties. 
        /// </summary>
        /// <param name="pname">A string describing the name of the property that has changed.</param>
        protected void OnPropertyChanged(string pname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pname));
            }
        }

        int _MyField = 708312;
        public int MyField
        {
            set
            {
                _MyField = value;
                OnPropertyChanged(nameof(MyField));
            }
            get
            {
                return _MyField;
            }
        }
    }
}

