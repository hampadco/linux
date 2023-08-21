using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ParameterSettingsViewModel : INotifyPropertyChanged
    {
        public int FlowValue { get; set; }
        public int OxygenValue { get; set; }
        private int _flowMinValue { get; set; }
        public int FlowMinValue { get { return _flowMinValue; } set { if (_flowMinValue != value) { _flowMinValue = value; OnPropertyChanged("FlowMinValue"); } } }
        private int _flowMaxValue { get; set; }
        public int FlowMaxValue { get { return _flowMaxValue; } set { if (_flowMaxValue != value) { _flowMaxValue = value; OnPropertyChanged("FlowMaxValue"); } } }
        private int _oxygenMinValue { get; set; }
        public int OxygenMinValue { get { return _oxygenMinValue; } set { if (_oxygenMinValue != value) { _oxygenMinValue = value; OnPropertyChanged("OxygenMinValue"); } } }
        private int _oxygenMaxValue { get; set; }
        public int OxygenMaxValue { get { return _oxygenMaxValue; } set { if (_oxygenMaxValue != value) { _oxygenMaxValue = value; OnPropertyChanged("OxygenMaxValue"); } } }
        public int SilentTime { get; set; }
        public int HRValue { get; set; }
        private int _minHRValue { get; set; }
        public int MinHRValue { get { return _minHRValue; } set { if (_minHRValue != value) { _minHRValue = value; OnPropertyChanged("MinHRValue"); } } }
        private int _maxHRValue { get; set; }
        public int MaxHRValue { get { return _maxHRValue; } set { if (_maxHRValue != value) { _maxHRValue = value; OnPropertyChanged("MaxHRValue"); } } }
        public int Spo2Value { get; set; }
        private int _minSpo2Value { get; set; }
        public int MinSpo2Value { get { return _minSpo2Value; } set { if (_minSpo2Value != value) { _minSpo2Value = value; OnPropertyChanged("MinSpo2Value"); } } }
        private int _maxSpo2Value { get; set; }
        public int MaxSpo2Value { get { return _maxSpo2Value; } set { if (_maxSpo2Value != value) { _maxSpo2Value = value; OnPropertyChanged("MaxSpo2Value"); } } }
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
