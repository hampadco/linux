using DataAccess;
using DataAccess.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ViewModel;
using ViewModel.Enum;
using Timer = System.Timers.Timer;

namespace SerialPortReader
{
    public class SimulateBlenderDevice : BaseDevice, IBlenderDevice
    {
        public event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataOximeter;
        public event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataFlow;
        public event EventHandler<BlenderReceiveDataViewModel> DeviceStatusChangedEvent;
        public event EventHandler<EnumDevice> OnStart;
        public event EventHandler<EnumDevice> OnStop;
        public event EventHandler<AutoStartDataViewModel> AutoStartStatusChangedEvent;

        Timer oximeterTimer;
        public float oxygen;
        public float flow;
        public float oximeter;
        public float spo2;
        public float hr;

        public float temp;
        public float o2pre;
        public float o2mv;

        private static volatile SimulateBlenderDevice instance;
        private static object syncRoot = new Object();
        public IDictionary<EnumDevice, bool> DeviceStartStatus { get; set; }
        public IDictionary<KeyMessage, bool> DeviceModuleStatus { get; set; }

        public static SimulateBlenderDevice Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SimulateBlenderDevice();
                    }
                }

                return instance;
            }
        }

        private SimulateBlenderDevice()
        {
            InitDeviceStartStatus();
            DeviceModuleStatus = new Dictionary<KeyMessage, bool>();
            oximeterTimer = new Timer(100);
            oximeterTimer.Elapsed += OximeterTimer_Elapsed; ;
            oximeterTimer.Start();

            spo2 = (int)(new Random().NextDouble() * 100);
            hr = new Random().Next(60, 65);
        }
        private void InitDeviceStartStatus()
        {
            DeviceStartStatus = new Dictionary<EnumDevice, bool>();
            foreach (EnumDevice device in (EnumDevice[])Enum.GetValues(typeof(EnumDevice)))
                DeviceStartStatus.Add(device, false);
        }
        private void OximeterTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            oximeter = (int)(new Random().Next(60, 190));

            temp = (new Random().Next(60, 190));
            o2pre = (new Random().Next(1, 40));
            o2mv = (new Random().Next(300, 700));

            if (OnReceivedDataOximeter != null)
                OnReceivedDataOximeter(sender, new BlenderReceiveDataViewModel
                {
                    SPo2 = (int)spo2,
                    Device = EnumDevice.Flow,
                    CreateDateTime = DateTime.Now,
                    FlowValue = (int)flow,
                    HR = (int)hr,
                    Oxygen = (int)oxygen,
                    OximeterValue = (int)oximeter,
                    Temp = temp,
                    O2_pr_Sensor = o2pre,
                    O2MV = o2mv,
                    Air_pr_value = (new Random().Next(1, 68)),
                    DeviceSerialNumber = (new Random().Next(10, 30)).ToString(),
                });
        }

        public void SetValueAsync(ParameterName parameter, int number)
        {
            if (number == -10)
            {
                DeviceStatusChangedEvent?.Invoke(null, new BlenderReceiveDataViewModel
                {
                    SPo2 = (int)0,
                    Device = EnumDevice.Oximeter,
                    CreateDateTime = DateTime.Now,
                    FlowValue = (int)0,
                    HR = (int)0,
                    Oxygen = (int)0,
                    OximeterValue = (int)0,
                    ExtraData = $"Error occured in {DateTime.Now.ToString("HH:mm:ss")}"
                });
                return;
            }
            if (number == -11)
            {
                AddOrUpdateError(KeyMessage.Flow_Sensor_Failed, true);
                AddOrUpdateError(KeyMessage.Flow_High_Air_pressure, true);
            }
            else if (number == -12)
            {
                AddOrUpdateError(KeyMessage.Flow_Sensor_Failed, false);
                AddOrUpdateError(KeyMessage.Flow_High_Air_pressure, false);
            }
            if (parameter == ParameterName.Flow)
                _ = Task.Factory.StartNew(async () =>
                {
                    DeviceStartStatus[EnumDevice.Flow] = true;
                    while (true)
                    {
                        if (number < flow)
                            flow--;
                        else if (number > flow)
                            flow++;
                        else
                            break;
                        var data = new BlenderReceiveDataViewModel
                        {
                            SPo2 = (int)spo2,
                            Device = EnumDevice.Flow,
                            CreateDateTime = DateTime.Now,
                            FlowValue = (int)flow,
                            HR = (int)hr,
                            Oxygen = (int)oxygen,
                            OximeterValue = (int)oximeter,
                            ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds()
                    };
                        OnReceivedDataFlow(null, data);
                        db.InsertData(new List<BlenderReceiveDataViewModel> { data });
                        await Task.Delay(1000);
                    }
                });
            if (parameter == ParameterName.Oxygen)
                _ = Task.Factory.StartNew(async () =>
                {
                    DeviceStartStatus[EnumDevice.Oxygen] = true;
                    while (true)
                    {
                        if (number < oxygen)
                            oxygen--;
                        else if (number > oxygen)
                            oxygen++;
                        else
                            break;
                        var data2 = new BlenderReceiveDataViewModel
                        {
                            SPo2 = (int)spo2,
                            Device = EnumDevice.Flow,
                            CreateDateTime = DateTime.Now,
                            FlowValue = (int)flow,
                            HR = (int)hr,
                            Oxygen = (int)oxygen,
                            OximeterValue = (int)oximeter,
                            ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds()
                    };
                        OnReceivedDataFlow(null, data2);
                        db.InsertData(new List<BlenderReceiveDataViewModel> { data2 });
                        await Task.Delay(1000);
                    }
                });
            SaveAsync(parameter, number.ToString());
        }
        public void Start(EnumDevice device)
        {
            DeviceStartStatus[device] = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (device)
                    {
                        case EnumDevice.Oximeter:
                            hr = 0;
                            spo2 = 0;
                            spo2 = (int)(new Random().NextDouble() * 100);
                            hr = new Random().Next(60, 65);
                            OnReceivedDataOximeter?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
                            break;
                        case EnumDevice.Flow:
                            flow = 0;
                            OnReceivedDataFlow?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
                            break;
                        case EnumDevice.Oxygen:
                            oxygen = 0;
                            OnReceivedDataFlow?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
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
            DeviceStartStatus[device] = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (device)
                    {
                        case EnumDevice.Oximeter:
                            hr = 0;
                            spo2 = 0;
                            OnReceivedDataOximeter?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
                            break;
                        case EnumDevice.Flow:
                            flow = 0;
                            OnReceivedDataFlow?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
                            break;
                        case EnumDevice.Oxygen:
                            oxygen = 0;
                            OnReceivedDataFlow?.Invoke(device, new BlenderReceiveDataViewModel
                            {
                                SPo2 = (int)spo2,
                                Device = EnumDevice.Flow,
                                CreateDateTime = DateTime.Now,
                                FlowValue = (int)flow,
                                HR = (int)hr,
                                Oxygen = (int)oxygen,
                                OximeterValue = (int)oximeter
                            });
                            break;
                        default:
                            break;
                    }
                    SaveUserActivity(ActionType.StartDevice, $"Send stop command to module {device}");
                }
                catch (Exception ex)
                {
                    SaveUserActivity(ActionType.Exception, $"Error On stop {device}\r\n{ex.Message}");
                }
            });
        }
        private List<BlenderReceiveDataViewModel> dataList { get; set; }
        private void SaveData(BlenderReceiveDataViewModel data)
        {
            dataList.Add(data);
            if (dataList.Count > 500)
            {
                db.InsertData(dataList);
                dataList.Clear();
            }
        }
        private void AddOrUpdateError(KeyMessage errorType, bool value)
        {
            bool change = true;
            if (DeviceModuleStatus.ContainsKey(errorType))
            {
                if (DeviceModuleStatus[errorType] != value)
                    DeviceModuleStatus[errorType] = value;
                else change = false;
            }
            else
                DeviceModuleStatus.Add(errorType, value);
            if (change)
            {
                var extraData = errorType.ToString() + (value ? " Occured" : " Fixed");
                var model = new BlenderReceiveDataViewModel { ExtraData = extraData, Device = EnumDevice.General };
                DeviceStatusChanged(model);
                SaveUserActivity(ActionType.DeviceModuleStatusChanged, $"{extraData}");
            }
        }
        private void DeviceStatusChanged(BlenderReceiveDataViewModel data)
        {
            OnDeviceStatusChanged(data);
        }
        protected virtual void OnDeviceStatusChanged(BlenderReceiveDataViewModel e)
        {
            DeviceStatusChangedEvent?.Invoke(this, e);
        }
        public async Task<bool> StartO2Calibration()
        {
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);
            });
            return true;
        }
        public void Test()
        {
            var data = new BlenderReceiveDataViewModel();
            var flow_value = 2.56f;
            var air_pr_value = 4.56f;
            var o2_value = 6.56f;
            var temp_value = 16.56f;
            var o2_pr_value = 12.56f;
            var o2_value_mv = 11.56f;
            data.FlowValue = flow_value;
            
            data.Air_pr_value = air_pr_value;
            data.Oxygen = o2_value;
            
            ///New
            data.Temp = temp_value;
            data.O2_pr_Sensor = o2_pr_value;
            data.O2MV = o2_value_mv;
            data.DeviceSerialNumber = "1.4.32";
            /////
            //data.ExtraData = $"flow_value:{flow_value} o2_pr_value:{o2_pr_value} o2_value:{o2_value} o2_value_mv:{o2_value_mv}";
            data.Device = EnumDevice.Flow;
            
            data.ElapsedMilliseconds = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            
            
            
        }

        public void StartAuto(AutoPageViewModel model)
        {
            throw new NotImplementedException();
        }

        public void StopAuto()
        {
            throw new NotImplementedException();
        }
        private Dictionary<DateTime, int> SampleData()
        {
            Dictionary<DateTime, int> list = new Dictionary<DateTime, int>();
            Random rnd = new Random();
            DateTime nowDate = DateTime.Now;
            for (int i = 0; i < 12000000; i++)
            {
                var X = nowDate.AddSeconds(i);
                int Y = rnd.Next(0, 200);
                list.Add(X, Y);
            }
            return list;
        }

        public Task<bool> SendCommand(EnumDevice enumDevice, string cmd)
        {
            throw new NotImplementedException();
        }

        public void OnCloseErrorDialog()
        {
            AutoStartStatusChangedEvent?.Invoke("CLOSEERRORDIALOG", new AutoStartDataViewModel { });
        }
    }
}
