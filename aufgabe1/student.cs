using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace aufgabe1
{
    public class student : INotifyPropertyChanged
    {
        private string _name;
        private int _matrikelnummer;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public int Matrikelnummer
        {
            get
            {
                return _matrikelnummer;
            }
            set
            {
                _matrikelnummer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Matrikelnummer)));

            }
        }

        public student(string name, int matrikelnummer)
        {
            this.Name = name;
            this.Matrikelnummer = matrikelnummer;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
