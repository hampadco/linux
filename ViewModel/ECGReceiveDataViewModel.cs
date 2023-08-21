using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ECGReceiveDataViewModel
    {
        public ECGReceiveDataViewModel()
        {
            Channel = new Channel[]
            {
                new Channel("I",0,false),
                new Channel("II",0, false),
                new Channel("III",0,false),
                new Channel("aVR",0,false),
                new Channel("aVL",0,false),
                new Channel("aVF",0,false),
                new Channel("C1",0, false),
                new Channel("Resp",0,false),
                new Channel("C2",0, false),
                new Channel("C3",0, false),
                new Channel("C4",0,false),
                new Channel("C5",0, false),
                new Channel("C6",0,false)
            };
        }
        public Channel[] Channel { get; set; }
        public bool IsATrace { get; set; }
        public double XValue { get; set; }
    }
    public class Channel
    {
        public string ChannelName;
        public bool isActive;
        public double value;

        public Channel(string n, double value, bool isActive)
        {
            ChannelName = n;
            this.value = value;
            this.isActive = isActive;
        }
        public Channel() { }
    }
}
