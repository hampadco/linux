using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class AutoStartDataViewModel : EventArgs
    {
        public float Oxygen { get; set; }
        public int Spo2 { get; set; }
        public DateTime CheckTime { get; set; }
        public float PreOxygen { get; set; }
        public int PreSpo2 { get; set; }
        public DateTime PreCheckTime { get; set; }
        public int NewValue { get; set; }
    }

    public class AutoPageViewModel : INotifyPropertyChanged
    {
        //public byte StepUp { get; set; }
        private byte _stepUp { get; set; }
        public byte StepUp { get { return _stepUp; } set { if (value != _stepUp) { _stepUp = value; OnPropertyChanged("StepUp"); } } }

        private byte _stepDown { get; set; }
        public byte StepDown { get { return _stepDown; } set { if (value != _stepDown) { _stepDown = value; OnPropertyChanged("StepDown"); } } }

        private byte _waitingTime { get; set; }
        public byte WaitingTime { get { return _waitingTime; } set { if (value != _waitingTime) { _waitingTime = value; OnPropertyChanged("WaitingTime"); } } }

        private byte _dif { get; set; }
        public byte Dif { get { return _dif; } set { if (value != _dif) { _dif = value; OnPropertyChanged("Dif"); } } }

        private bool _isAutoStart { get; set; }
        public bool IsAutoStart { get { return _isAutoStart; } set { if (value != _isAutoStart) { _isAutoStart = value; OnPropertyChanged("IsAutoStart"); } } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            var handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
