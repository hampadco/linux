using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Configuration;
using ViewModel;
using ViewModel.Enum;
using DataAccess;
using DataAccess.Model;
using DataAccess.Model.Enum;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SerialPortReader
{
    public abstract class BaseDevice
    {
        public DB db;
        public BaseDevice()
        {
            db = new DB();
        }
        public async virtual void SaveAsync(ParameterName param, string value)
        {
            await db.UpdateSettingsValueAsync(param, value);
        }
        public void SaveUserActivity(ActionType action, string message)
        {
            db.InsertUserActivityDataAsync(new UserActivityDataModel
            {
                ActionId = action,
                OtherInfo = message
            });
        }
    }
    public class BlenderDevice : BaseDevice, IBlenderDevice
    {
        public event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataOximeter;
        public event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataFlow;
        public event EventHandler<BlenderReceiveDataViewModel> DeviceStatusChangedEvent;
        public event EventHandler<AutoStartDataViewModel> AutoStartStatusChangedEvent;
        public event EventHandler<EnumDevice> OnStart;
        public event EventHandler<EnumDevice> OnStop;
        private static volatile BlenderDevice instance;
        private static object syncRoot = new Object();
        private IDictionary<EnumDevice, string> DeviceStatus;
        public IDictionary<EnumDevice, bool> DeviceStartStatus { get; set; }
        public IDictionary<KeyMessage, bool> DeviceModuleStatus { get; set; }
        private List<BlenderReceiveDataViewModel> dataList { get; set; }
        private List<BlenderReceiveDataViewModel> dataListFlow { get; set; }
        static public float Oxygen { get; set; }
        static public float Flow { get; set; }
        static public int OximeterVal { get; set; }
        public int spo2 { get; set; }
        public int hr { get; set; }
        private AutoPageViewModel AutoModel { get; set; }
        public static BlenderDevice Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BlenderDevice();
                    }
                }

                return instance;
            }
        }
        private BlenderDevice()
        {
            DeviceStatus = new Dictionary<EnumDevice, string>();
            InitDeviceStartStatus();
            DeviceModuleStatus = new Dictionary<KeyMessage, bool>();
            dataList = new List<BlenderReceiveDataViewModel>();
            dataListFlow = new List<BlenderReceiveDataViewModel>();
            InitSerialPort();
        }
        private void InitDeviceStartStatus()
        {
            DeviceStartStatus = new Dictionary<EnumDevice, bool>();
            foreach (EnumDevice device in (EnumDevice[])Enum.GetValues(typeof(EnumDevice)))
                DeviceStartStatus.Add(device, false);
        }

        private void FireDataReceivedOximeter(BlenderReceiveDataViewModel data)
        {
            SaveData(data);
            OnReceivedDataOximeterEvent(data);
        }

        private void DeviceStatusChanged(BlenderReceiveDataViewModel data)
        {
            OnDeviceStatusChanged(data);
        }
        DateTime lastInsertFlow;
        private void FireDataReceivedFlow(BlenderReceiveDataViewModel data)
        {
            dataListFlow.Add(data);
            if (DateTime.Now > lastInsertFlow.AddSeconds(5))
            {
                db.InsertData(dataListFlow);
                lastInsertFlow = DateTime.Now;
            }
            OnReceivedDataFlowEvent(data);
        }
        protected virtual void OnReceivedDataOximeterEvent(BlenderReceiveDataViewModel e)
        {
            OnReceivedDataOximeter?.Invoke(this, e);
        }
        protected virtual void OnDeviceStatusChanged(BlenderReceiveDataViewModel e)
        {
            DeviceStatusChangedEvent?.Invoke(this, e);
        }
        protected virtual void OnReceivedDataFlowEvent(BlenderReceiveDataViewModel e)
        {
            OnReceivedDataFlow?.Invoke(this, e);
        }
        SerialPort spo2SerialPort;
        SerialPort flowSerialPort;
        private void InitSerialPort()
        {
            OpenSpO2SerialPOrt();
            OpenFlowSerialPOrt();
        }
        private void OpenFlowSerialPOrt()
        {
            Task.Factory.StartNew(async () =>
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                flowSerialPort = new SerialPort("/dev/tty.usbserial-A50285BI");//config.AppSettings.Settings["FlowPort"].Value
                flowSerialPort.BaudRate = 9600;
                flowSerialPort.Parity = Parity.None;
                flowSerialPort.StopBits = StopBits.One;
                flowSerialPort.DataBits = 8;
                flowSerialPort.ReadBufferSize = 30;
                flowSerialPort.Handshake = Handshake.None;
                //flowSerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandlerFlow);
                while (!flowSerialPort.IsOpen)
                {
                    try
                    {
                        flowSerialPort.Open();
                        SetStatus(ActionType.StartDevice.ToString(), EnumDevice.Flow);
                    }
                    catch (Exception ex)
                    {
                        SetStatus(ex.Message, EnumDevice.Flow);
                        //if (ex.Message != errorMsgOpenFlowSerialPOrt)
                        //{
                        //    //errorMsgOpenFlowSerialPOrt = ex.Message;
                        //    ///Error in opening serial port for Flow\r\n
                        //    //SaveUserActivity(ActionType.OpenPortError, $"{ex.Message}");
                        //    SetStatus(ex.Message, EnumDevice.Flow);
                        //}
                    }
                    await Task.Delay(1000);
                }
                StartFlowRead();
            });
        }
        private void OpenSpO2SerialPOrt()
        {
            Task.Factory.StartNew(async () =>
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                spo2SerialPort = new SerialPort("/dev/tty.usbserial-1420");//new SerialPort(config.AppSettings.Settings["SPO2Port"].Value);
                spo2SerialPort.BaudRate = 9600;
                spo2SerialPort.Parity = Parity.None;
                spo2SerialPort.StopBits = StopBits.One;
                spo2SerialPort.DataBits = 8;
                spo2SerialPort.ReadBufferSize = 30;
                spo2SerialPort.Handshake = Handshake.None;
                //spo2SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                while (!spo2SerialPort.IsOpen)
                {
                    try
                    {
                        spo2SerialPort.Open();
                        SetStatus(ActionType.StartDevice.ToString());
                    }
                    catch (Exception ex)
                    {
                        SetStatus(ex.Message);
                    }
                    await Task.Delay(1000);
                }
                StartSpO2Read();
            });
        }
        private void SetStatus(string newStatus, EnumDevice device = EnumDevice.Oximeter)
        {
            Task.Factory.StartNew(() =>
            {
                if (!DeviceStatus.ContainsKey(device))
                    DeviceStatus.Add(device, string.Empty);
                if (DeviceStatus[device] != newStatus)
                {
                    DeviceStatus[device] = newStatus;
                    var model = new BlenderReceiveDataViewModel { ExtraData = newStatus, Device = device };
                    DeviceStatusChanged(model);
                    SaveUserActivity(ActionType.DeviceStatusChanged, $"{device}=> {newStatus}");
                }
            });
        }
        private void StartSpO2Read()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        CoreReadSpo2();
                        //await Task.Delay(10);
                    }
                    catch (Exception ex)
                    {
                        //Error in StartSpO2Read\r\n
                        SaveUserActivity(ActionType.OpenPortError, $"{ex.Message}");
                        if (ex.Message.Contains("I/O"))
                        {
                            spo2SerialPort.Dispose();
                            OpenSpO2SerialPOrt();
                            break;
                        }
                    }
                }
            });
        }
        private void CoreReadSpo2()
        {
            var val = getccb();

            if (val == 0xF8)
            {
                while ((val = getccb()) < 0xF0)
                {
                    if (val == 0)
                    {

                    }
                    OximeterVal = val;
                    FireDataReceivedOximeter(new BlenderReceiveDataViewModel
                    {
                        Device = EnumDevice.Oximeter,
                        OximeterValue = val,
                        SPo2 = spo2,
                        HR = hr,
                        FlowValue = Flow,
                        Oxygen = Oxygen,
                        ExtraData = String.Empty,
                        ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds()
                    });
                }
            }
            switch (val)
            {
                case 0xF9:
                    spo2 = getccb();
                    //Console.WriteLine("Val3: {0}", spo2 = getccb()); /* print SpO2 */
                    break;
                case 0xFA:
                    hr = (char)getccb();
                    //Console.WriteLine("Val4: {0}", hr = (char)getccb()); /* print pulse */
                    break;
                case 0xFB:
                    switch (getccb())
                    {
                        case 0:
                            AddOrUpdateError(KeyMessage.Oximeter_No_sensor_connected, false);
                            AddOrUpdateError(KeyMessage.Oximeter_No_finger_in_probe, false);
                            AddOrUpdateError(KeyMessage.Oximeter_Low_perfusion, false);
                            //SetStatus("OK"); /* print messages */
                            break;
                        case 1:
                            AddOrUpdateError(KeyMessage.Oximeter_No_sensor_connected, true);
                            //SetStatus("No sensor connected!");
                            break;
                        case 2:
                            AddOrUpdateError(KeyMessage.Oximeter_No_finger_in_probe, true);
                            //SetStatus("No finger in probe!");
                            break;
                        case 3:
                            AddOrUpdateError(KeyMessage.Oximeter_Low_perfusion, true);
                            //SetStatus("Low perfusion!");
                            break;
                    }
                    break;
                case 0xFC:
                    val = getccb();
                    switch (val)
                    {
                        case 0:
                            Console.WriteLine("OK"); /* print messages */
                            break;
                        case 1:
                            Console.WriteLine("No sensor connected!");
                            break;
                        case 2:
                            Console.WriteLine("No finger in probe!");
                            break;
                        case 3:
                            Console.WriteLine("Low perfusion!");
                            break;
                    }
                    break;
                    //printf(”% 02u”, getccb() & 0x0F); /* print quality, mask perf.*/
                    //Console.WriteLine("Val5:{0} Val6:{1}", getccb() & 0x0F, val);
                    break;
            }
        }
        private void CoreReadFlow()
        {
            byte buf1 = 0;
            byte buf2 = 0;
            byte buf3 = 0;
            byte buf4 = 0;

            var data = new BlenderReceiveDataViewModel();
            if (flowSerialPort.IsOpen) buf1 = Convert.ToByte(flowSerialPort.ReadByte());

            if (buf1 == 64)   //byte1 starting byte,check byte 1
            {

                if (flowSerialPort.IsOpen) { buf2 = Convert.ToByte(flowSerialPort.ReadByte()); }
                if (flowSerialPort.IsOpen) { buf3 = Convert.ToByte(flowSerialPort.ReadByte()); }
                if (flowSerialPort.IsOpen) { buf4 = Convert.ToByte(flowSerialPort.ReadByte()); }

                if (buf2 == 64 && buf3 == 64 && buf4 == 64)
                {

                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }  // byte 2 , flow value byte 2
                    float flow_value;
                    flow_value = (float)buf1;
                    flow_value = flow_value * 100;
                    byte RX = buf1;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 3 , flow value byte 3
                    flow_value = flow_value + buf1;
                    flow_value = flow_value / 100;
                    //////!!!!!!this.BeginInvoke(new flowValueDeleg(nextflow), new object[] { flow_value }); // show flow

                    if (flowSerialPort.IsOpen)
                    {
                        buf1 = Convert.ToByte(flowSerialPort.ReadByte());
                    } // byte 4 ,check byte1
                    float check_byte1;
                    check_byte1 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb1ValueMVDeleg(check_byte_sub), new object[] { check_byte1 });
                    //show check byte 1
                    /////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 5,O2 value byte1 /////////////6xy
                    float o2_value;
                    o2_value = (float)buf1;
                    o2_value = o2_value * 100;
                    //byte RX = buf;
                    if (flowSerialPort.IsOpen)
                    {
                        buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 6 , O2 value byte2
                        o2_value = o2_value + buf1;
                        o2_value = o2_value / 100;
                        //////!!!!!! this.BeginInvoke(new o2ValueDeleg(nexto2), new object[] { o2_value });
                    } //show o2 value
                      //
                    if (flowSerialPort.IsOpen)
                    {
                        buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 7 ,check byte 2
                        float check_byte2;
                        check_byte2 = (float)buf1;
                        //////!!!!!!this.BeginInvoke(new checkb2ValueMVDeleg(check_byte2_sub), new object[] { check_byte2 });
                    } //show check byte 2
                      //////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 8
                    float o2_value_mv;
                    o2_value_mv = (float)buf1;
                    o2_value_mv = o2_value_mv * 100;
                    //byte RX = buf;
                    if (flowSerialPort.IsOpen)
                    {
                        buf1 = Convert.ToByte(flowSerialPort.ReadByte());  // byte 9
                        o2_value_mv = o2_value_mv + buf1;
                        o2_value_mv = o2_value_mv / 100;
                        //////!!!!!!this.BeginInvoke(new o2ValueMVDeleg(nexto2_mv), new object[] { o2_value_mv });
                    }
                    /////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 10 ,check byte 3
                    float check_byte3;
                    check_byte3 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb3ValueMVDeleg(check_byte3_sub), new object[] { check_byte3 }); //show check byte 3
                    //////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 11
                    float o2_pr_value;
                    o2_pr_value = (float)buf1;
                    o2_pr_value = o2_pr_value * 100;
                    //byte RX = buf;
                    if (flowSerialPort.IsOpen) buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 12
                    o2_pr_value = o2_pr_value + buf1;
                    o2_pr_value = o2_pr_value / 100;
                    //////!!!!!!this.BeginInvoke(new o2prValueMVDeleg(nexto2_pr), new object[] { o2_pr_value });
                    /////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) buf1 = Convert.ToByte(flowSerialPort.ReadByte()); // byte 13 ,check byte 4
                    float check_byte4;
                    check_byte4 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb4ValueMVDeleg(check_byte4_sub), new object[] { check_byte4 }); //show check byte 4
                    /////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 14
                    float air_pr_value;
                    air_pr_value = (float)buf1;
                    air_pr_value = air_pr_value * 100;
                    //byte RX = buf;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 15
                    air_pr_value = air_pr_value + buf1;
                    air_pr_value = air_pr_value / 100; ////برای ایر پرشر
                    //////!!!!!!this.BeginInvoke(new airprValueMVDeleg(nextair_pr), new object[] { air_pr_value });
                    ///////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 16 ,check byte 5
                    float check_byte5;
                    check_byte5 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb5ValueMVDeleg(check_byte5_sub), new object[] { check_byte5 }); //show check byte 5
                    ///////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 17
                    float temp_value;
                    temp_value = (float)buf1;
                    temp_value = temp_value * 100;
                    //byte RX = buf;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 18
                    temp_value = temp_value + buf1;
                    temp_value = temp_value / 100;
                    //////!!!!!!this.BeginInvoke(new tempValueMVDeleg(nexttemp_value), new object[] { temp_value });
                    ///////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 19 ,check byte 6
                    float check_byte6;
                    check_byte6 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb6ValueMVDeleg(check_byte6_sub), new object[] { check_byte6 }); //show check byte6
                    ///////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 20  Reserved
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 21  Reserved
                                                                                                     ////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 22 ,check byte 7
                    float check_byte7;
                    check_byte7 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb7ValueMVDeleg(check_byte7_sub), new object[] { check_byte7 }); //show check byte7
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 23  Reserved
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 24  Reserved
                                                                                                     //////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 25 ,check byte 8
                    float check_byte8;
                    check_byte8 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb8ValueMVDeleg(check_byte8_sub), new object[] { check_byte8 }); //show check byte8
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 26  Reserved
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 27  Reserved
                                                                                                    //////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }
                    // byte 28 ,check byte 9
                    float check_byte9;
                    check_byte9 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb9ValueMVDeleg(check_byte9_sub), new object[] { check_byte9 }); //show check byte9
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 29 ,ERROR byte 1
                    byte error_byte_1;
                    error_byte_1 = (byte)buf1;
                    CheckByteForError(29, error_byte_1);
                    //////!!!!!!this.BeginInvoke(new errorbyte1ValueMVDeleg(error_byte_1_sub), new object[] { error_byte_1 });
                    //////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 30 ,check byte 10
                    float check_byte10;
                    check_byte10 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb10ValueMVDeleg(check_byte10_sub), new object[] { check_byte10 }); //show check byte10
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 31 ,ERROR byte 2
                    byte error_byte_2;
                    error_byte_2 = (byte)buf1;
                    CheckByteForError(31, error_byte_2);
                    //////!!!!!!this.BeginInvoke(new errorbyte2ValueMVDeleg(error_byte_2_sub), new object[] { error_byte_2 });
                    //////////////////////////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 32 ,check byte 11
                    float check_byte11;
                    check_byte11 = (float)buf1;
                    //////!!!!!!this.BeginInvoke(new checkb11ValueMVDeleg(check_byte11_sub), new object[] { check_byte11 }); //show check byte10
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    #region NewFor Errors

                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); }// byte 33 ,ERROR byte 3
                    byte error_byte_3;
                    error_byte_3 = (byte)buf1;
                    CheckByteForError(33, error_byte_3);
                    //this.BeginInvoke(new errorbyte3ValueMVDeleg(error_byte_3_sub), new object[] { error_byte_3 });
                    ///////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 34 ,check byte 12
                    float check_byte12;
                    check_byte12 = (float)buf1;
                    //this.BeginInvoke(new checkb12ValueMVDeleg(check_byte12_sub), new object[] { check_byte12 }); //show check byte12
                    ///////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 35 ,serial byte 1
                    float serial_byte1;
                    serial_byte1 = (float)buf1;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 36 ,serial byte 2
                    float serial_byte2;
                    serial_byte2 = (float)buf1;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 37 ,serial byte 3
                    float serial_byte3;
                    serial_byte3 = (float)buf1;
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 38 ,serial byte 4
                    float serial_byte4;
                    serial_byte4 = (float)buf1;
                    float serial_number;
                    serial_number = (float)serial_byte4;
                    //this.BeginInvoke(new serialnumberDeleg(serial_number_sub), new object[] { serial_number }); //show check byte12
                    ///////////////////////////////////////////////////////////////////////////
                    if (flowSerialPort.IsOpen) { buf1 = Convert.ToByte(flowSerialPort.ReadByte()); } // byte 39 ,check byte 13
                    float check_byte13;
                    check_byte13 = (float)buf1;
                    //this.BeginInvoke(new checkb13ValueMVDeleg(check_byte13_sub), new object[] { check_byte13 }); //show check byte13
                    ///////////////////////////////////////////////////////////////////////////
                    #endregion

                    data.FlowValue = flow_value;
                    Flow = flow_value;
                    data.Air_pr_value = air_pr_value;
                    data.Oxygen = o2_value;
                    Oxygen = o2_value;
                    ///New
                    data.Temp = temp_value;
                    data.O2_pr_Sensor = o2_pr_value;
                    data.O2MV = o2_value_mv;
                    data.DeviceSerialNumber = serial_byte1.ToString() + serial_byte2.ToString() + serial_byte3.ToString() + serial_byte4.ToString();
                    /////
                    //data.ExtraData = $"flow_value:{flow_value} o2_pr_value:{o2_pr_value} o2_value:{o2_value} o2_value_mv:{o2_value_mv}";
                    data.Device = EnumDevice.Flow;
                    data.OximeterValue = OximeterVal;
                    data.ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                    data.HR = hr;
                    data.SPo2 = spo2;
                    FireDataReceivedFlow(data);
                }
            }
        }
        public void Test()
        {
            var data = new BlenderReceiveDataViewModel();
            var flow_value = 4.56f;
            var air_pr_value = 4.56f;
            var o2_value = 6.56f;
            var temp_value = 16.56f;
            var o2_pr_value = 12.56f;
            var o2_value_mv = 11.56f;
            data.FlowValue = flow_value;
            Flow = flow_value;
            data.Air_pr_value = air_pr_value;
            data.Oxygen = o2_value;
            Oxygen = o2_value;
            ///New
            data.Temp = temp_value;
            data.O2_pr_Sensor = o2_pr_value;
            data.O2MV = o2_value_mv;
            data.DeviceSerialNumber = "1.4.32";
            /////
            //data.ExtraData = $"flow_value:{flow_value} o2_pr_value:{o2_pr_value} o2_value:{o2_value} o2_value_mv:{o2_value_mv}";
            data.Device = EnumDevice.Flow;
            data.OximeterValue = OximeterVal;
            data.ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            data.HR = hr;
            data.SPo2 = spo2;
            FireDataReceivedFlow(data);
        }
        private void StartFlowRead()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        CoreReadFlow();
                        //await Task.Delay(10);
                    }
                    catch (Exception ex)
                    {
                        //Error in opening serial port for Flow\r\n
                        SaveUserActivity(ActionType.OpenPortError, $"{ex.Message}");
                        if (ex.Message.Contains("I/O"))
                        {
                            flowSerialPort.Dispose();
                            OpenFlowSerialPOrt();
                            break;
                        }
                    }
                }
            });
        }
        private byte getccb()
        {
            return Convert.ToByte(spo2SerialPort.ReadByte());
        }
        private void SetFlowValue(int number)
        {
            try
            {
                byte byte1 = (byte)((number & 0x000000FF));
                byte byte2 = (byte)((number & 0x0000FF00) >> 8);
                var dataByte1 = new byte[] { byte1 };
                var dataByte2 = new byte[] { byte2 };
                if (!flowSerialPort.IsOpen)
                    flowSerialPort.Open();
                flowSerialPort.Write("CFA");     // STX = 2
                flowSerialPort.Write(dataByte1, 0, 1);
                flowSerialPort.Write(dataByte2, 0, 1);
            }
            catch (Exception ex)
            {
                //Error in SetFlowValue\r\n
                SaveUserActivity(ActionType.Exception, $"{ex.Message}");
            }
        }
        private void SetOxygenValue(int number)
        {
            try
            {
                byte byte1 = (byte)((number & 0x000000FF));
                byte byte2 = (byte)((number & 0x0000FF00) >> 8);
                var dataByte1 = new byte[] { byte1 };
                var dataByte2 = new byte[] { byte2 };
                string send_auto_o2 = number.ToString();
                if (!flowSerialPort.IsOpen)
                    flowSerialPort.Open();
                flowSerialPort.Write("DOA");     // STX = 2
                flowSerialPort.Write(dataByte1, 0, 1);
                flowSerialPort.Write(dataByte2, 0, 1);
            }
            catch (Exception ex)
            {
                //Error in SetOxygenValue\r\n
                SaveUserActivity(ActionType.Exception, $"{ex.Message}");
            }
        }

        public void SetValueAsync(ParameterName parameter, int number)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var actionType = ActionType.Exception;
                    switch (parameter)
                    {
                        case ParameterName.Oxygen:
                            SetOxygenValue(number);
                            Task.Delay(1000);
                            SetOxygenValue(number);////Request From Company For Twice Send
                            actionType = ActionType.ChangeOxygenValue;
                            DeviceStartStatus[EnumDevice.Oxygen] = true;
                            break;
                        case ParameterName.Flow:
                            SetFlowValue(number);
                            Task.Delay(1000);
                            SetFlowValue(number);////Request From Company For Twice Send
                            actionType = ActionType.ChangeFlowValue;
                            DeviceStartStatus[EnumDevice.Flow] = true;
                            break;
                        default:
                            break;
                    }
                    SaveUserActivity(actionType, $"Set {parameter} Value To {number}");
                    SaveAsync(parameter, number.ToString());
                }
                catch (Exception ex)
                {
                    //Error in SetValueAsync parameter:{parameter} value:{number}\r\n
                    SaveUserActivity(ActionType.Exception, $"{ex.Message}\r\nParameter:{parameter} Value:{number}");
                }
            });
        }
        public void Start(EnumDevice device)
        {
            DeviceStartStatus[device] = true;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    switch (device)
                    {
                        case EnumDevice.Oximeter:
                            await SendStartCommand(device);
                            await Task.Delay(1000);
                            await SendStartCommand(device);////Request From Company For Twice Send
                            break;
                        case EnumDevice.Flow:
                            //Start
                            break;
                        case EnumDevice.Oxygen:
                            //Start
                            break;
                        case EnumDevice.Fan:
                            await SendStartStopFanCommand(true);
                            await Task.Delay(1000);
                            await SendStartStopFanCommand(true);////Request From Company For Twice Send
                            break;
                        default:
                            break;
                    }
                    SaveUserActivity(ActionType.StartDevice, $"Send start command to module {device}");
                }
                catch (Exception ex)
                {
                    SaveUserActivity(ActionType.Exception, $"Error On Start {device}\r\n{ex.Message}");
                }
            });
        }
        public void Stop(EnumDevice device)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    DeviceStartStatus[device] = false;
                    await SendStopCommand(device);
                    await Task.Delay(1000);
                    await SendStopCommand(device);////Request From Company For Twice Send
                    switch (device)
                    {
                        case EnumDevice.Oximeter:
                            //Stop
                            break;
                        case EnumDevice.Flow:
                            //Stop
                            break;
                        case EnumDevice.Oxygen:
                            //Stop
                            break;
                        case EnumDevice.Fan:
                            await SendStartStopFanCommand(false);
                            await Task.Delay(1000);
                            await SendStartStopFanCommand(false);////Request From Company For Twice Send
                            break;
                        default:
                            break;
                    }
                    //DeviceStartStatus[device] = false;
                    SaveUserActivity(ActionType.StartDevice, $"Send stop command to module {device}");
                }
                catch (Exception ex)
                {
                    SaveUserActivity(ActionType.Exception, $"Error On stop {device}\r\n{ex.Message}");
                }
            });
        }
        DateTime lastInsert;
        private void SaveData(BlenderReceiveDataViewModel data)
        {
            dataList.Add(data);
            if (dataList.Count > 1000 || DateTime.Now > lastInsert.AddSeconds(5))
            //if (dataList.Count > 500)
            {
                var tmp = dataList.Where(p => p.OximeterValue == 0).ToList();
                db.InsertData(new List<BlenderReceiveDataViewModel>(dataList));
                dataList.Clear();
                lastInsert = DateTime.Now;
            }
        }
        private bool GetBit(byte b, int bitNumber)
        {
            var bit = (b & (1 << bitNumber)) != 0;
            return bit;
        }
        private void CheckByteForError(int byteNumber, byte b)
        {
            try
            {
                switch (byteNumber)
                {
                    case 29:
                        for (int i = 0; i < 8; i++)
                            AddOrUpdateError((KeyMessage)i + 4, GetBit(b, i));
                        break;
                    case 31:
                        for (int i = 0; i < 8; i++)
                            AddOrUpdateError((KeyMessage)i + 12, GetBit(b, i));
                        break;
                    case 33:
                        for (int i = 0; i < 2; i++)
                            AddOrUpdateError((KeyMessage)i + 20, GetBit(b, i));
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void AddOrUpdateError(KeyMessage errorType, bool value)
        {
            var isDeviceStart = DeviceStartStatus.Any(p => p.Value && errorType.ToString().StartsWith(p.Key.ToString()));
            if (!isDeviceStart) return;
            if ((int)(errorType) == 23)
            {
                if (DeviceModuleStatus.ContainsKey(errorType))
                {
                    if (DeviceModuleStatus[errorType] != value)
                    {
                    }
                }
            }
            bool change = false;
            if (DeviceModuleStatus.ContainsKey(errorType))
            {
                if (DeviceModuleStatus[errorType] != value)
                {
                    DeviceModuleStatus[errorType] = value;
                    change = true;
                }
                else change = false;
            }
            else if (value)
            {
                DeviceModuleStatus.Add(errorType, value);
                change = true;
            }
            if (change)
            {
                var extraData = errorType.ToString() + (value ? " Occured" : " Fixed");
                var model = new BlenderReceiveDataViewModel { ExtraData = extraData, Device = EnumDevice.General };
                DeviceStatusChanged(model);
                SaveUserActivity(ActionType.DeviceModuleStatusChanged, $"{extraData}");
            }
            var t = DeviceStartStatus;
            var ff = 0;
        }
        public async Task<bool> StartO2Calibration()
        {
            return await Task.Factory.StartNew(() =>
            {
                if (flowSerialPort != null && flowSerialPort.IsOpen)
                {
                    flowSerialPort.Write("EOC");     // start o2 calibration
                    byte[] bytestosend = new byte[1] { 0x00 };
                    flowSerialPort.Write(bytestosend, 0, 1);
                    flowSerialPort.Write(bytestosend, 0, 1);
                    return true;
                }
                return false;
            });
        }
        async Task<bool> SendStopCommand(EnumDevice enumDevice)
        {
            return await Task.Factory.StartNew(() =>
            {
                var cmd = enumDevice == EnumDevice.Flow ? "JFS" : enumDevice == EnumDevice.Oxygen ? "KOS" : (enumDevice == EnumDevice.Oximeter ? "PSP" : "");
                var port = flowSerialPort;// enumDevice == EnumDevice.Flow ? flowSerialPort : spo2SerialPort;
                if (port != null && port.IsOpen)
                {
                    port.Write(cmd);
                    byte[] bytestosend = new byte[1] { 0x00 };
                    port.Write(bytestosend, 0, 1);
                    port.Write(bytestosend, 0, 1);
                    return true;
                }
                return false;
            });
        }
        async Task<bool> SendStartCommand(EnumDevice enumDevice)
        {
            return await Task.Factory.StartNew(() =>
            {
                var cmd = enumDevice == EnumDevice.Flow ? "" : enumDevice == EnumDevice.Oxygen ? "" : (enumDevice == EnumDevice.Oximeter ? "OST" : "");
                var port = flowSerialPort;// enumDevice == EnumDevice.Flow ? flowSerialPort : spo2SerialPort;
                if (port != null && port.IsOpen)
                {
                    port.Write(cmd);
                    byte[] bytestosend = new byte[1] { 0x00 };
                    port.Write(bytestosend, 0, 1);
                    port.Write(bytestosend, 0, 1);
                    return true;
                }
                return false;
            });
        }
        async Task<bool> SendStartStopFanCommand(bool isStart)
        {
            return await Task.Factory.StartNew(() =>
            {
                var cmd = isStart ? "MFS" : "NFT";
                if (flowSerialPort != null && flowSerialPort.IsOpen)
                {
                    flowSerialPort.Write(cmd);
                    byte[] bytestosend = new byte[1] { 0x00 };
                    flowSerialPort.Write(bytestosend, 0, 1);
                    flowSerialPort.Write(bytestosend, 0, 1);
                    return true;
                }
                return false;
            });
        }
        public async Task<bool> SendCommand(EnumDevice enumDevice, string cmd)
        {
            return await Task.Factory.StartNew(() =>
            {
                var port = enumDevice == EnumDevice.Flow ? flowSerialPort : spo2SerialPort;
                if (port != null && port.IsOpen)
                {
                    port.Write(cmd);
                    byte[] bytestosend = new byte[1] { 0x00 };
                    port.Write(bytestosend, 0, 1);
                    port.Write(bytestosend, 0, 1);
                    return true;
                }
                return false;
            });
        }

        #region AutoStart
        Timer waitingTimeAutoTimer;
        int preSpo2;
        float preOxygen;
        DateTime preCheckTime;
        public void StartAuto(AutoPageViewModel model)
        {
            AutoModel = model;
            waitingTimeAutoTimer = new Timer(AutoModel.WaitingTime * 1000);
            waitingTimeAutoTimer.Elapsed += WaitingTimeAutoTimer_Elapsed;
            waitingTimeAutoTimer.Start();
            preSpo2 = spo2;
            preOxygen = Oxygen;
            preCheckTime = DateTime.Now;
            SaveUserActivity(ActionType.StartAuto, $"Start Auto");
        }
        public void StopAuto()
        {
            waitingTimeAutoTimer.Stop();
            SaveUserActivity(ActionType.StopAuto, $"Stop Auto");
        }

        private void WaitingTimeAutoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DoAuto();
        }
        const int normalOxygenValue = 21;
        /// <summary>
        /// This method auto set oxygen for patient
        /// </summary>
        private void DoAuto()
        {
            try
            {
                if (Oxygen <= normalOxygenValue) return;
                var model = new AutoStartDataViewModel
                {
                    Spo2 = spo2,
                    Oxygen = Oxygen,
                    CheckTime = DateTime.Now,
                    PreSpo2 = preSpo2,
                    PreOxygen = preOxygen,
                    PreCheckTime = preCheckTime
                };
                var threshold = preSpo2 * Convert.ToDouble(AutoModel.Dif / 100f);
                var up = preSpo2 + threshold;
                var down = preSpo2 - threshold;
                if (spo2 > up || (spo2 < up && spo2 > down))
                {
                    /// Send Down Oxygen
                    var newValue = Convert.ToInt32(Oxygen - AutoModel.StepDown);
                    model.NewValue = newValue;
                    if (Oxygen < (normalOxygenValue + AutoModel.StepDown)) newValue = normalOxygenValue;
                    //SetOxygenValue(newValue);
                    SaveUserActivity(ActionType.AutoStartDown, $"Auto Change Oxygen From {preOxygen} To {newValue} (DOWN)");
                    AutoStartStatusChangedEvent?.Invoke("DOWN", model);
                }
                else if (spo2 < down)
                {
                    /// Send Up Oxygen
                    var newValue = Convert.ToInt32(Oxygen + AutoModel.StepUp);
                    model.NewValue = newValue;
                    //SetOxygenValue(newValue);
                    SaveUserActivity(ActionType.AutoStartDown, $"Auto Change Oxygen From {preOxygen} To {newValue} (UP)");
                    AutoStartStatusChangedEvent?.Invoke("UP", model);
                }

                preSpo2 = spo2;
                preOxygen = Oxygen;
                preCheckTime = DateTime.Now;
            }
            catch
            {

            }
        }
        public void OnCloseErrorDialog()
        {
            AutoStartStatusChangedEvent?.Invoke("CLOSEERRORDIALOG", new AutoStartDataViewModel { });
        }
        #endregion
    }
}
