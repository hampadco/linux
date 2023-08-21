using DataAccess.Model.Enum;
using SerialPortReader.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using ViewModel;
using ViewModel.Enum;
using Timer = System.Timers.Timer;

namespace SerialPortReader
{
    //    public class ECGDevice : BaseDevice, IECGDevice
    //    {
    //        private IDictionary<string, string> DeviceStatus;
    //        private static volatile ECGDevice instance;
    //        private static object syncRoot = new Object();
    //        private List<ECGReceiveDataViewModel> dataList;
    //        public event EventHandler<ECGReceiveDataViewModel> DeviceStatusChangedEvent;
    //        //public event nextIdentifyDeleg StartBeginOnvoke;
    //        SerialPort ecgSerialPort;
    //        bool tone = false;
    //        bool save = false;
    //        byte[] buffer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //        byte buf = 0;
    //        public static ECGDevice Instance
    //        {
    //            get
    //            {
    //                if (instance == null)
    //                {
    //                    lock (syncRoot)
    //                    {
    //                        if (instance == null)
    //                            instance = new ECGDevice();
    //                    }
    //                }

    //                return instance;
    //            }
    //        }
    //        private ECGDevice()
    //        {
    //            DeviceStatus = new Dictionary<string, string>();
    //            dataList = new List<ECGReceiveDataViewModel>();
    //            //InitSerialPort();
    //            Start();
    //        }

    //        private void InitSerialPort()
    //        {
    //            Task.Factory.StartNew(async () =>
    //            {
    //                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    //                ecgSerialPort = new SerialPort(config.AppSettings.Settings["ECGPort"].Value);
    //                ecgSerialPort.BaudRate = 115200;
    //                ecgSerialPort.Parity = Parity.Even;
    //                ecgSerialPort.ReceivedBytesThreshold = 10;
    //                //ecgSerialPort.StopBits = StopBits.One;
    //                //ecgSerialPort.DataBits = 8;
    //                //ecgSerialPort.ReadBufferSize = 30;
    //                //ecgSerialPort.Handshake = Handshake.None;
    //                ecgSerialPort.DataReceived += new SerialDataReceivedEventHandler(ecgSerialPort_DataReceived);
    //                while (!ecgSerialPort.IsOpen)
    //                {
    //                    try
    //                    {
    //                        ecgSerialPort.Open();
    //                        SetStatus(ActionType.StartDevice.ToString());
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        SetStatus(ex.Message);
    //                    }
    //                    await Task.Delay(1000);
    //                }
    //                //StartSpO2Read();
    //            });
    //        }
    //        private void StartSpO2Read()
    //        {
    //            Task.Factory.StartNew(() =>
    //            {
    //                while (true)
    //                {
    //                    try
    //                    {
    //                        //CoreReadSpo2();
    //                        //await Task.Delay(10);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        //Error in StartSpO2Read\r\n
    //                        SaveUserActivity(ActionType.OpenPortError, $"{ex.Message}");
    //                        if (ex.Message.Contains("I/O"))
    //                        {
    //                            ecgSerialPort.Dispose();
    //                            InitSerialPort();
    //                            break;
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //        private void SetStatus(string newStatus, EnumDevice device = EnumDevice.Oximeter)
    //        {
    //            Task.Factory.StartNew(() =>
    //            {
    //                if (!DeviceStatus.ContainsKey(device.ToString()))
    //                    DeviceStatus.Add(device.ToString(), string.Empty);
    //                if (DeviceStatus[device.ToString()] != newStatus)
    //                {
    //                    DeviceStatus[device.ToString()] = newStatus;
    //                    var model = new ECGReceiveDataViewModel { };
    //                    DeviceStatusChanged(model);
    //                    SaveUserActivity(ActionType.DeviceStatusChanged, $"{device}=> {newStatus}");
    //                }
    //            });
    //        }
    //        private void DeviceStatusChanged(ECGReceiveDataViewModel e)
    //        {
    //            DeviceStatusChangedEvent?.Invoke(this, e);
    //        }


    //        private delegate void nextLimbDeleg(byte[] limb);
    //        private delegate void nextChestDeleg(byte[] chest);
    //        private delegate void nextValueDeleg(byte[] value);
    //        private delegate void nextStatusDeleg(byte[] status);
    //        private delegate void nextStatusChestDeleg(byte[] statusChest);
    //        private delegate void nextIdentifyDeleg(String message);
    //        private delegate void nextUpdateDeleg();
    //        // parse serial Data
    //        private void ecgSerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
    //        {
    //            while (ecgSerialPort.BytesToRead > 6)
    //            {

    //                buf = Convert.ToByte(ecgSerialPort.ReadByte());

    //                if (buf > 0xf7)
    //                {
    //                    if (buf == 0xfd)
    //                    {
    //                        string temp = "";//Status = buf;

    //                        temp = ecgSerialPort.ReadTo("\0");

    //                        //var tmp = new nextIdentifyDeleg(ekg.nextIdentify);
    //                    }
    //                    else
    //                    {
    //                        byte[] tempbuffer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //                        buffer[0] = buf;
    //                        if (ecgSerialPort.IsOpen)
    //                        {
    //                            buffer[1] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                        }

    //                        if (buffer[0] == 0xf8)
    //                        {
    //                            //SPO = buf;
    //                            int temp = ((buffer[1]) >> 4) + 2;

    //                            if (temp < 11)
    //                            {
    //                                for (int i = 2; i < temp; i++)
    //                                {
    //                                    if (ecgSerialPort.IsOpen)
    //                                    {
    //                                        buffer[i] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                                    }
    //                                }
    //                                // byte[] tempbuffer ={ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0,0};
    //                                Buffer.BlockCopy(buffer, 0, tempbuffer, 0, temp);
    //                                //this.BeginInvoke(new nextLimbDeleg(ekg.nextLimb), new object[] { tempbuffer });
    //                            }
    //                        }
    //                        else if (buffer[0] == 0xfe)
    //                        {

    //                            int temp = ((buffer[1]) >> 4) + 2;
    //                            if (temp < 8)
    //                            {
    //                                for (int i = 2; i < temp; i++)
    //                                {
    //                                    if (ecgSerialPort.IsOpen)
    //                                    {
    //                                        buffer[i] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                                    }
    //                                }
    //                                Buffer.BlockCopy(buffer, 0, tempbuffer, 0, temp);
    //                                //this.BeginInvoke(new nextChestDeleg(ekg.nextChest), new object[] { tempbuffer });
    //                            }
    //                        }

    //                        else if ((buffer[0] == 0xf9) || (buffer[0] == 0xfa))
    //                        {
    //                            if (ecgSerialPort.IsOpen)
    //                            {
    //                                buffer[2] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                            }

    //                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 3);
    //                            //this.BeginInvoke(new nextValueDeleg(ekg.nextValue), new object[] { tempbuffer });
    //                        }
    //                        else if (buffer[0] == 0xfc)
    //                        {

    //                            for (int i = 2; i < 6; i++)
    //                            {
    //                                if (ecgSerialPort.IsOpen)
    //                                {
    //                                    buffer[i] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                                }
    //                            }

    //                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 6);
    ////                            this.BeginInvoke(new nextStatusDeleg(ekg.nextStatus), new object[] { tempbuffer });
    //                        }
    //                        else if (buffer[0] == 0xff)
    //                        {
    //                            //Status = buf;
    //                            for (int i = 2; i < 4; i++)
    //                            {
    //                                if (ecgSerialPort.IsOpen)
    //                                {
    //                                    buffer[i] = Convert.ToByte(ecgSerialPort.ReadByte());
    //                                }
    //                            }

    //                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 4);
    //                            //this.BeginInvoke(new nextStatusChestDeleg(ekg.nextStatusChest), new object[] { tempbuffer });
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        public async Task<bool> ChangeMode(ECGMode mode)
    //        {
    //            await Task.Factory.StartNew(() =>
    //            {
    //                if (mode == ECGMode.Normal)
    //                {
    //                    if (ecgSerialPort.IsOpen)
    //                        ecgSerialPort.WriteLine("M0");
    //                }
    //                if (mode == ECGMode.Simulate)
    //                {
    //                    if (ecgSerialPort.IsOpen)
    //                        ecgSerialPort.WriteLine("M1");
    //                }
    //            });
    //            return true;
    //        }
    //        // open Comport
    //        public void Start()
    //        {
    //            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    //            ecgSerialPort = new SerialPort(config.AppSettings.Settings["ECGPort"].Value);
    //            ecgSerialPort.BaudRate = 115200;
    //            ecgSerialPort.Parity = Parity.Even;
    //            ecgSerialPort.ReceivedBytesThreshold = 10;
    //            ecgSerialPort.DataReceived += new SerialDataReceivedEventHandler(ecgSerialPort_DataReceived);
    //            if (ecgSerialPort.IsOpen)
    //                ecgSerialPort.DiscardInBuffer();
    //            ecgSerialPort.Close();
    //            try
    //            {
    //                ecgSerialPort.Open();

    //            }
    //            catch(Exception ex)
    //            {
    //                throw new Exception("Serial port " + ecgSerialPort.PortName +
    //                        " cannot be opened!" + " RS232 tester ");
    //            }


    //            if (ecgSerialPort.IsOpen)
    //            {
    //                ecgSerialPort.WriteLine("I");


    //                //radioButton_Amp1.PerformClick();
    //                //radioButton_100.PerformClick();
    //                //radioButton_normal.PerformClick();
    //                //radioButton_emgOff.PerformClick();
    //                //radioButton_Monitor.PerformClick();
    //                //radioButton_paceOn.PerformClick();
    //                //radioButton_50.PerformClick();
    //            }

    //        }
    //        // search all Comports 
    //        private void list_comport()
    //        {
    //            {
    //                // Get a list of serial port names. 
    //                string[] ports = SerialPort.GetPortNames();
    //                // Display each port name to the console. 
    //                foreach (string port in ports)
    //                {

    //                }
    //            }
    //        }
    //    }
    public class ECGDevice
    {
        SerialPort serialPort1;
        Timer timer1;
        Timer timer2;


        private delegate void nextLimbDeleg(byte[] limb);
        private delegate void nextChestDeleg(byte[] chest);
        private delegate void nextValueDeleg(byte[] value);
        private delegate void nextStatusDeleg(byte[] status);
        private delegate void nextStatusChestDeleg(byte[] statusChest);
        private delegate void nextIdentifyDeleg(String message);
        private delegate void nextUpdateDeleg();

        public event EventHandler<object[]> OnReceivedData;




        public EKG ekg = null;
        bool tone = false;
        bool save = false;
        byte[] buffer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte buf = 0;




        public ECGDevice()
        {
            ekg = new EKG();
            list_comport();
            ekg.Rand = 30;
            Start();
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            //toolStripStatusLabel_time.Text = DateTime.Now.ToLongTimeString();
        }

        private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            // this.BeginInvoke(new nextUpdateDeleg(ekg.update), new object[] { });
        }

        // search all Comports 
        private void list_comport()
        {
            {
                // Get a list of serial port names. 
                string[] ports = SerialPort.GetPortNames();

                foreach (string port in ports)
                {
                }
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            list_comport();
        }
        
        
        private void Start()
        {
            ekg = new EKG();
            serialPort1 = new SerialPort("COM8");
            serialPort1.ReceivedBytesThreshold = 10;
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = Parity.Even;

            serialPort1.DataReceived += serialPort1_DataReceived;
            if (serialPort1.IsOpen)
                serialPort1.DiscardInBuffer();
            serialPort1.Close();
            try
            {
                serialPort1.Open();

            }
            catch (Exception ex)
            {
            }
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.WriteLine("I");
                    ChangeMode();
                    Channel1_Checked();
                }
                catch(Exception ex)
                {

                }
            }
        }
        // parse serial Data
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            ////Console.WriteLine("SerialPort1_DataReceived");
            while (serialPort1.BytesToRead > 6)
            {

                buf = Convert.ToByte(serialPort1.ReadByte());

                if (buf > 0xf7)
                {
                    if (buf == 0xfd)
                    {
                        string temp = "";//Status = buf;

                        temp = serialPort1.ReadTo("\0");

                        
                        //this.BeginInvoke(new nextIdentifyDeleg(ekg.nextIdentify), new object[] { temp });
                    }
                    else
                    {
                        byte[] tempbuffer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        buffer[0] = buf;
                        if (serialPort1.IsOpen)
                        {
                            buffer[1] = Convert.ToByte(serialPort1.ReadByte());
                        }

                        if (buffer[0] == 0xf8)
                        {
                            //SPO = buf;
                            int temp = ((buffer[1]) >> 4) + 2;

                            if (temp < 11)
                            {
                                for (int i = 2; i < temp; i++)
                                {
                                    if (serialPort1.IsOpen)
                                    {
                                        buffer[i] = Convert.ToByte(serialPort1.ReadByte());
                                        if(i==9)
                                            Console.WriteLine("{0},", buffer[i]);
                                    }
                                }
                                // byte[] tempbuffer ={ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0,0};
                                Buffer.BlockCopy(buffer, 0, tempbuffer, 0, temp);
                                //OnReceivedData?.Invoke(new nextLimbDeleg(ekg.nextLimb), new object[] { tempbuffer });
                                //ekg.nextLimb(tempbuffer);
                                nextLimbDeleg del = ekg.nextLimb;
                                del(tempbuffer);
                                //OnReceivedData?.Invoke(tempbuffer,null);
                                //new nextLimbDeleg(ekg.nextLimb(tempbuffer));
                                //this.BeginInvoke(new nextLimbDeleg(ekg.nextLimb), new object[] { tempbuffer });
                            }
                        }
                        else if (buffer[0] == 0xfe)
                        {

                            int temp = ((buffer[1]) >> 4) + 2;
                            if (temp < 8)
                            {
                                for (int i = 2; i < temp; i++)
                                {
                                    if (serialPort1.IsOpen)
                                    {
                                        buffer[i] = Convert.ToByte(serialPort1.ReadByte());
                                    }
                                }
                                Buffer.BlockCopy(buffer, 0, tempbuffer, 0, temp);
                                //ekg.nextChest(tempbuffer);
                                nextLimbDeleg del = ekg.nextChest;
                                del(tempbuffer);
                                //this.BeginInvoke(new nextChestDeleg(ekg.nextChest), new object[] { tempbuffer });
                            }
                        }

                        else if ((buffer[0] == 0xf9) || (buffer[0] == 0xfa))
                        {
                            if (serialPort1.IsOpen)
                            {
                                buffer[2] = Convert.ToByte(serialPort1.ReadByte());
                            }

                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 3);
                            // this.BeginInvoke(new nextValueDeleg(ekg.nextValue), new object[] { tempbuffer });
                        }
                        else if (buffer[0] == 0xfc)
                        {

                            for (int i = 2; i < 6; i++)
                            {
                                if (serialPort1.IsOpen)
                                {
                                    buffer[i] = Convert.ToByte(serialPort1.ReadByte());
                                }
                            }

                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 6);
                            //this.BeginInvoke(new nextStatusDeleg(ekg.nextStatus), new object[] { tempbuffer });
                        }
                        else if (buffer[0] == 0xff)
                        {
                            //Status = buf;
                            for (int i = 2; i < 4; i++)
                            {
                                if (serialPort1.IsOpen)
                                {
                                    buffer[i] = Convert.ToByte(serialPort1.ReadByte());
                                }
                            }

                            Buffer.BlockCopy(buffer, 0, tempbuffer, 0, 4);
                            //this.BeginInvoke(new nextStatusChestDeleg(ekg.nextStatusChest), new object[] { tempbuffer });
                        }
                    }
                }
            }
        }

        // Check all electrodes
        private void button_electrodes_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.WriteLine("q0");
        }

        // simulate / Normal-mode
        private void ChangeMode(string mode = "Simulate")
        {
            if (mode == "Normal")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("M0");
            }
            if (mode == "Simulate")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("M1");
            }
        }

        private void Modus_Checked(string modus = "Adult")
        {
            if (modus == "Adult")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("N0");
            }
            if (modus == "Neonates")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("N1");
            }
        }
        // 50Hz / 60Hz filter
        private void ChangeFilter(string filter = "50 Hz")
        {
            if (filter == "50 Hz")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("51");
            }
            if (filter == "60 Hz")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("52");
            }
            if (filter == "Off")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("50");
            }
        }

        // Speed of the Graph
        private void Speed_Checked(string speed)
        {
            if (speed == "50/sec")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("S0");

            }
            else if (speed == "100/sec")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("S1");
            }
            else if (speed == "150/sec")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("S2");
            }

            else if (speed == "300/sec")
            {
                if (serialPort1.IsOpen)
                    serialPort1.WriteLine("S7");
            }
            //toolStripStatusLabel_Speed.Text =((RadioButton)sender).Text;
        }

        // Check Channels
        private void Channel1_Checked()
        {
            int cnt_channel_temp = 0;
            byte[] temp = { 0x43, 0 };
            byte[] temp2 = { 0x44, 0 };

            for (int i = 0; i < 8; i++) //CHECK
            {
                temp[1] |= (byte)(1 << i);

                cnt_channel_temp++;
            }
            ekg.Channel_limb_Count = cnt_channel_temp;
            for (int i = 0; i < 5; i++) //CHECK
            {
                temp2[1] |= (byte)(1 << i);

                cnt_channel_temp++;
            }

            if (serialPort1.IsOpen)
            {

                serialPort1.Write(temp, 0, 2);
                //if (ekg.flag_eg1200 == true)
                    serialPort1.Write(temp2, 0, 2);
            }
            ekg.Channel_Count = cnt_channel_temp;

            ekg.Index = ekg.Rand + 2;

            ekg.Packetsread = 0;
            //ekg.display_diag();




        }


        private void Form1_Leave(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
            if (ekg.outfileCtr > 0)
                ekg.outfile.Close();

        }

        // update time in the statusbar
        private void timer1_Tick(object sender, EventArgs e)
        {
            //toolStripStatusLabel_time.Text = DateTime.Now.ToLongTimeString();
        }

        string File = null;



        // en- / disable Datasaving 
        

        private void tableLayoutPanel1_Resize(object sender, EventArgs e)
        {
            if (ekg != null)
            {
                //grenzen.Height = pictureBox_Graph.Height;
                //grenzen.Width = pictureBox_Graph.Width;
                //ekg.display_diag();

            }
        }




        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        public class EKG
        {
            public event EventHandler<object[]> OnReceivedData;
            private static int Flag_LL = 0x01;      // 00000001
            private static int Flag_RL = 0x02;      // 00000010
            private static int Flag_LA = 0x04;      // 00000100
            private static int Flag_RA = 0x08;      // 00001000
            private static int Flag_Chest = 0x10;   // 00010000
            private static int Flag_C2 = 0x0100;    // 00000001
            private static int Flag_C3 = 0x0200;    // 00000010
            private static int Flag_C4 = 0x0400;    // 00000100
            private static int Flag_C5 = 0x0800;    // 00001000
            private static int Flag_C6 = 0x1000;    // 00010000

            string[] Signal = { "I", "II", "III", "aVR", "aVL", "aVF", "C1",
                                "Resp", "C2", "C3", "C4", "C5", "C6" };

            string[] status ={ "Normal", "Normal + Pacemaker detected", "not used", "not used", "Initializing",
                               "Searching for electrodes", "not used", "not used", "Simulated Output", "not used",
                               "Selftest Error", "not used", "not used", "not used", "not used", "not used" };


            

            StreamWriter Outfile;


            // Channel class for all parameter
            public class channel
            {
                public string CH_name;
                public int _hoehe;
                public int _base;
                public Color Color;
                public bool xoffset;
                public int old_wave;

                public channel(string n, int h, int b, Color col, bool off)
                {
                    CH_name = n;
                    _hoehe = h;
                    _base = b;
                    Color = col;
                    xoffset = off;
                    old_wave = 0;
                }
            }

            int Outfilectr = -1;
            int cnt_Channel = 0;
            int cnt_Channel1 = 0;


            channel[] Channel = new channel[]
            {
                new channel("I",0, 0, Color.Blue, false),
                new channel("II",0, 0, Color.Brown, false),
                new channel("III",0, 0, Color.Green, false),
                new channel("aVR",0, 0, Color.Black , false),
                new channel("aVL",0, 0, Color.Blue, false),
                new channel("aVF",0, 0, Color.Indigo, false),
                new channel("C1",0, 0, Color.Green, false),
                new channel("Resp",0, 0, Color.BlueViolet, false),
                new channel("C2",0, 0, Color.MediumAquamarine, false),
                new channel("C3",0, 0, Color.IndianRed, false),
                new channel("C4",0, 0, Color.MediumPurple, false),
                new channel("C5",0, 0, Color.Orange, false),
                new channel("C6",0, 0, Color.PaleVioletRed, false)
            };


            int column = 1;         // Column of the graph
            byte rand;
            int index;
            byte gr_rand = 20;
            int max_Wert;


            int[] Datensatz;        // Array of last Wavevalues
            int packetsread = 0;

            bool Flag_eg1200 = false;

            public EKG()
            {

                Datensatz = new int[13];
                index = rand + 2;


                //form1.pictureBox_Graph.Image = new Bitmap(form1.pictureBox_Graph.Width, form1.pictureBox_Graph.Height);

            }

            public StreamWriter outfile
            {
                get
                {
                    return Outfile;
                }

                set
                {
                    Outfile = value;
                }
            }

            public bool flag_eg1200
            {
                get
                {
                    return Flag_eg1200;
                }
                set
                {
                    Flag_eg1200 = value;
                }

            }

            public int Channel_Count
            {
                get
                {

                    return cnt_Channel;
                }
                set
                {
                    if (value > 7)
                        column = 2;
                    else column = 1;
                    cnt_Channel = value;
                }
            }

            public int outfileCtr
            {
                get
                {
                    return Outfilectr;
                }
                set
                {
                    Outfilectr = value;
                }
            }

            public int Channel_limb_Count
            {
                get
                {
                    return cnt_Channel1;
                }
                set
                {
                    cnt_Channel1 = value;
                }
            }

            public int Index
            {
                get
                {
                    return index;
                }
                set
                {
                    index = value;
                }
            }

            public int Packetsread
            {
                get
                {
                    return packetsread;
                }
                set
                {
                    packetsread = value;
                }
            }

            public int Max_Wert
            {
                get
                {
                    return max_Wert;
                }
                set
                {
                    max_Wert = value;
                }
            }

            public byte Rand
            {
                get
                {
                    return rand;
                }
                set
                {
                    rand = value;
                }
            }
            public void nextLimb(byte[] buffer)
            {
                int lng = (buffer[1] >> 4) + 2;
                int check = (buffer[1] & 0x0f);
                int temp = 0;
                for (int i = 2; i < lng; i++)
                {
                    temp += buffer[i];
                }
                temp += buffer[0];
                temp = temp % 16;

                //if (temp == check)
                {
                    for (int i = 2; i < lng; i++)
                    {
                        Datensatz[i - 2] = buffer[i];
                    }
                    OnReceivedData?.Invoke(Datensatz, null);
                    //Display();

                }
            }
            private void Display()
            {

                //bmp = (Bitmap)Form3.pictureBox_Graph.Image;

                //if ((Form3.pictureBox_Graph.Image != null))
                {
                    int i = 0;


                    packetsread += 1;
                    if (packetsread > 5)
                    {
                        packetsread = 5;
                    }
                    foreach (channel Member in Channel)
                    {
                        //if (Member._hoehe > 0)
                        if (1==1)
                        {
                            i = display_wave(Member, i);
                        }

                    }
                    if (1==2)
                    {
                        if (Outfilectr > 0)
                        {
                            int j = 0;
                            if (Flag_eg1200 == true)
                            {
                                foreach (channel Member in Channel)

                                {
                                    if (Member._hoehe > 0)
                                    {
                                        Outfile.Write(Datensatz[j++]);
                                        Outfile.Write("\t");
                                    }
                                    else
                                    {
                                        Outfile.Write(0);
                                        Outfile.Write("\t");
                                    }
                                }
                            }
                            else
                            {
                                foreach (channel Member in Channel)
                                {
                                    //Control[] temp = Form3.splitContainer1.Panel1.Controls.Find(("checkbox_" + Member.CH_name), true);
                                    if (1 > 0)
                                    {
                                        if (Member._hoehe > 0)
                                        {
                                            Outfile.Write(Datensatz[j++]);
                                            Outfile.Write("\t");
                                        }
                                        else
                                        {
                                            Outfile.Write(0);
                                            Outfile.Write("\t");
                                        }
                                    }
                                }
                            }

                            Outfile.WriteLine(Outfilectr);
                            Outfilectr++;
                        }
                    }

                }
            }

            //parsing identify message
            public void nextIdentify(string message)
            {
                //form1.label_Identify.Text = message;
                if (message.Contains("EG12000"))
                {
                    Flag_eg1200 = true;
                    //form1.WaveValue = new int[13];
                }
                else
                {
                    Flag_eg1200 = false;
                    //form1.WaveValue = new int[8];
                }

                if (Flag_eg1200 == true)
                {
                    //form1.checkBox_C2.Enabled = true;
                    //form1.checkBox_C3.Enabled = true;
                    //form1.checkBox_C4.Enabled = true;
                    //form1.checkBox_C5.Enabled = true;
                    //form1.checkBox_C6.Enabled = true;
                    //form1.label_C2.Enabled = true;
                    //form1.label_C3.Enabled = true;
                    //form1.label_C4.Enabled = true;
                    //form1.label_C5.Enabled = true;
                    //form1.label_C6.Enabled = true;
                }
            }

            //  81   82   83   84   85   86   87   88   89   90   91   92   93   94   95   96    97    98    99    100
            int[] pt = { 440, 466, 494, 523, 554, 587, 622, 659, 698, 740, 784, 831, 880, 932, 988, 1046, 1109, 1175, 1244, 1328 };

            //Funktion for Sound of Piep
            private void Piep(double Frequenz, int time)
            {
                const int MinFreq = 0x25;
                const int MaxFreq = 0x7FFF;

                if (Frequenz < MinFreq)
                    Frequenz = MinFreq;
                else if (Frequenz > MaxFreq)
                    Frequenz = MaxFreq;
                Console.Beep((int)(Frequenz), time);
            }

            //parsing Value for pulse and respiration
            public void nextValue(byte[] buffer)
            {
                int lng = 3;
                int check = (buffer[1] & 0x7f);
                int temp = 0;
                for (int i = 2; i < lng; i++)
                {
                    temp += buffer[i];
                }
                temp += buffer[0];
                temp = temp % 128;
                if (temp == check)
                {
                    if ((buffer[0] & 0x03) == 1)
                    {
                        //form1.label_Pulse.Text = buffer[2].ToString();
                        if ((buffer[2] != 0))     // Have to beep ?
                        {

                            int frq = 0;


                            Piep(pt[frq], 50);
                        }
                    }
                    else if ((buffer[0] & 0x03) == 2)
                    {
                        //form1.label_Resp.Text = buffer[2].ToString();
                    }
                }
            }

            bool Status_ok = false;

            // parsing statusmessage and electrodes I - C1
            

            // parsing for Status of Electrodes C2-C6 (only EG12000)
            public void nextStatusChest(byte[] buffer)
            {
                int lng = 4;
                int check = (buffer[1] & 0x7f);
                int temp = 0;
                for (int i = 2; i < lng; i++)
                {
                    temp += buffer[i];
                }
                temp += buffer[0];
                temp = temp % 128;
                if (temp == check)
                {
                    electrode_check(((buffer[2] & 0x1F) << 8) + 0x8000);
                }

            }

            enum cha { chi, chii, chiii, chvr, chvl, chvf, chc1, chresp, chc2, chc3, chc4, chc5, chc6 };

            //parsing Wavevalue message I - C1
         

            //parsing Wavevalue message C2- C6 (only EG12000)
            public void nextChest(byte[] buffer)
            {
                int lng = (buffer[1] >> 4) + 2;
                int check = (buffer[1] & 0x0f);
                int temp = 0;
                for (int i = 2; i < lng; i++)
                {
                    temp += buffer[i];
                }
                temp += buffer[0];
                temp = temp % 16;

                if (temp == check)
                {
                    for (int i = 2; i < lng; i++)
                    {
                        Datensatz[i - 2 + Channel_limb_Count] = buffer[i];
                    }
                    OnReceivedData?.Invoke(Datensatz, null);

                }
            }

            public void electrode_check(int electrode)
            {
            }

            //Bitmap bmp;

            // draw waveform to graph
            private int display_wave(channel ch, int i)
            {
                ch.old_wave = Datensatz[i++];
                ////Console.WriteLine("I: " + i + " " + ch.CH_name + " old_wave: " + ch.old_wave + " _base: " + ch._base + " _hoehe: " + ch._hoehe);
                return i;
            }

            // Draw Graphs and save Value
            

            // Refresh Picturebox of graph


            bool display_diag_exit = false;

           // Bitmap bmp1;

            // Funktion to draw diagrammlegend
            

            // Draw all Diagramms
            
        }

        private void label_Identify_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("I");
            }
        }

        private void button_Calibrate_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("K");
            }
        }

    }
}
