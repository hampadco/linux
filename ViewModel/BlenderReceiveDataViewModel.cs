using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Enum;

namespace ViewModel
{
    public class BlenderReceiveDataViewModel : EventArgs
    {
        public int Id { get; set; }
        public int OximeterValue { get; set; }
        public float FlowValue { get; set; }
        public float Oxygen { get; set; }
        public int SPo2 { get; set; }
        public float Air_pr_value { get; set; }
        public float O2MV { get; set; }
        public float Temp { get; set; }
        public float O2_pr_Sensor { get; set; }
        public int HR { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ExtraData { get; set; }
        public EnumDevice Device { get; set; }
        public string DeviceSerialNumber { get; set; }
    }
}
