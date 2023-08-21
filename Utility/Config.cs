using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.Enum;

namespace Utility
{
    public class Config
    {
        private static volatile Config instance;
        private static object syncRoot = new Object();

        private Config()
        {
            InitStatusDictionary();
        }
        private void InitStatusDictionary()
        {
            try
            {
                foreach (KeyMessage item in (KeyMessage[])Enum.GetValues(typeof(KeyMessage)))
                {
                    if (item.ToString().StartsWith(ConstantsStrings.PreOximeter))
                        DeviceError.Add(item, EnumDevice.Oximeter);
                    else if (item.ToString().StartsWith(ConstantsStrings.PreFlowMain))
                        DeviceError.Add(item, EnumDevice.Flow);
                    else if (item.ToString().StartsWith(ConstantsStrings.PreOxygen))
                        DeviceError.Add(item, EnumDevice.Oxygen);
                    else
                        DeviceError.Add(item, EnumDevice.General);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Config Instance
        {
            get
            {
                ////test32
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Config();
                    }
                }

                return instance;
            }
        }
        public ParameterSettingsViewModel ParameterSettings { get; set; }
        public bool IsLockMode { get; set; }
        public bool IsSilentMode { get; set; }
        public string CurrentUserId { get; set; } = Environment.UserDomainName;
        public string CurrentUserName { get; set; } = Environment.UserName;
        public string Password => "";//ConfigurationManager.AppSettings["Password"];
        public int FlowMinRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["FlowMinRange"]);
        public int FlowMaxRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["FlowMaxRange"]);
        public int OxygenMinRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["OxygenMinRange"]);
        public int OxygenMaxRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["OxygenMaxRange"]);
        public int HRMinRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["HRMinRange"]);
        public int HRMaxRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["HRMaxRange"]);
        public int Spo2MinRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["Spo2MinRange"]);
        public int Spo2MaxRange => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["Spo2MaxRange"]);
        public int FlowThresholdPercentage =>1;// Convert.ToInt32(ConfigurationManager.AppSettings["FlowThresholdPercentage"]);
        public int OxygenThresholdPercentage =>1;// Convert.ToInt32(ConfigurationManager.AppSettings["OxygenThresholdPercentage"]);
        public bool SliderGuageViewWithRange => true;//Convert.ToBoolean(ConfigurationManager.AppSettings["SliderGuageViewWithRange"]);
        public bool ShowTrends =>true;// Convert.ToBoolean(ConfigurationManager.AppSettings["ShowTrends"]);
        public int BlurPageValue => 5;
        public string PatientId { get; set; }
        public bool IsKeyboardListen { get; set; }
        public int TimeDelayAlarmOxygen =>1;// Convert.ToInt32(ConfigurationManager.AppSettings["TimeDelayAlarmOxygen"]) * 1000;
        public int TimeDelayAlarmFlow =>1;// Convert.ToInt32(ConfigurationManager.AppSettings["TimeDelayAlarmFlow"]) * 1000;
        public int TimeDelayAlarmHR => 1;//Convert.ToInt32(ConfigurationManager.AppSettings["TimeDelayAlarmHR"]) * 1000;
        public int TimeDelayAlarmSpo2 =>1;// Convert.ToInt32(ConfigurationManager.AppSettings["TimeDelayAlarmSpo2"]) * 1000;
        public bool ActiveAlarmFlow { get; set; }
        public bool ActiveAlarmOxygen { get; set; }
        public bool ActiveAlarmHR { get; set; }
        public bool ActiveAlarmSpo2 { get; set; }
        public bool AutoRemoveOldData => true;//Convert.ToBoolean(ConfigurationManager.AppSettings["AutoRemoveOldData"]);
        public byte CameraIndex =>1;// Convert.ToByte(ConfigurationManager.AppSettings["CameraIndex"]);
        public readonly KeyMessage[] DangerErrors ;// ConfigurationManager.AppSettings["DangerErrors"].ToString().Split(',').Select(p => (KeyMessage)(Convert.ToByte(p))).ToArray();// new KeyMessage[] { KeyMessage.Flow_Low_Air_pressure, KeyMessage.Oxygen_Low_O2_pressure };
        public string HospitalName { get; set; }

        #region Auto System
        public byte StepUp { get; set; }
        public byte StepDown { get; set; }
        public byte WaitingTime { get; set; }
        public byte Dif { get; set; }
        public bool IsAutoStart { get; set; }
        public bool IsWorkingApplication { get; set; } = false;
        public bool SaveMode =>true;// Convert.ToBoolean(ConfigurationManager.AppSettings["SaveMode"]);
        #endregion

        //public string ClientName => ConfigurationManager.AppSettings["ClientName"];
        //public string MoviePath => ConfigurationManager.AppSettings["MoviePath"];
        //public string BellIconPath => "/TrainInteractiveSystem;component/Resources/Icons/bell.png";
        //public string InfoIconPath => "/TrainInteractiveSystem;component/Resources/Icons/InfoIcon.png";
        //public string ErrorIconPath => "/TrainInteractiveSystem;component/Resources/Icons/errorIcon.png";
        //public string UNCUserName => ConfigurationManager.AppSettings["UNCUserName"];
        //public string UNCPassword => ConfigurationManager.AppSettings["UNCPassword"];
        //public string UNCShareName => ConfigurationManager.AppSettings["ShareName"];
        //public string RootMediaPath => ConfigurationManager.AppSettings["RootMediaPath"];
        //public string MusicPath => ConfigurationManager.AppSettings["MusicPath"];
        //public string ImagePath => ConfigurationManager.AppSettings["ImagePath"];
        //public string DocPath => ConfigurationManager.AppSettings["DocPath"];
        //public string AdvertisePath => ConfigurationManager.AppSettings["AdvertisePath"];
        //public string HelpDocFileName => ConfigurationManager.AppSettings["HelpDocFileName"];
        //public string HelpServerDocFileName => ConfigurationManager.AppSettings["HelpServerDocFileName"];
        //public string NVRIP => ConfigurationManager.AppSettings["NVRIP"];
        //public int CheckAliveInterval => Convert.ToInt32(ConfigurationManager.AppSettings["CheckAliveInterval"]);

        //public string[] ValidMovieExtensions => new[] { ".mpg", ".avi", ".mkv", ".mp4" };
        //public Dictionary<EWindowsCommand, string> DicWindowsCommand = new Dictionary<EWindowsCommand, string> {
        //    {EWindowsCommand.StartNotepad,"notepad.exe"},
        //    {EWindowsCommand.KillClient,"taskkill /im traininteractivesystem.exe /f /t"}
        //};
        //public int MaxReportPage => 20;
        //public string CustomCommands => ConfigurationManager.AppSettings["CustomCommands"];

        //public SourceDestinationInfoViewModel travelInfo;
        //public string DVRUrl => $"http://{this.NVRIP}/doc/page/login.asp?_1573940422293&page=preview";//"http://192.168.0.1/login_security.html";//
        //public string RtspNVRUrl => $"rtsp://{this.NVRIP}:554/Streaming/Channels/";
        //public string RtspNVRUrlAihua => $"rtsp://{CameraUserName}:{this.CameraPassword}@{this.NVRIP}:554/cam/realmonitor?channel=";
        //public string RtspGeneralFormat => ConfigurationManager.AppSettings["RtspGeneralFormat"];
        //private object SavedElement { get; set; }
        //public void SaveElement<T>(T obj)
        //{
        //    SavedElement = new object();
        //    SavedElement = obj;
        //}
        //public T GetElement<T>()
        //{
        //    return (T)SavedElement;
        //}
        //public string[] FlipchartIP => ConfigurationManager.AppSettings["FlipchartIP"].Split(';');
        //public int FlipchartPort => Convert.ToInt32(ConfigurationManager.AppSettings["FlipchartPort"]);
        //public string MicShortutKey => ConfigurationManager.AppSettings["MicShortutKey"];
        //public string FlipchartResetMessage => ConfigurationManager.AppSettings["FlipchartResetMessage"];
        //public string Users => ConfigurationManager.AppSettings["Users"];
        //public string CurrentUserName { get; set; }
        //public string Company => ConfigurationManager.AppSettings["Company"];
        //public string ResourcePath => ConfigurationManager.AppSettings["ResourcePath"];
        //public string DownloadType => ConfigurationManager.AppSettings["DownloadType"];
        //public string Tools => ConfigurationManager.AppSettings["Tools"];
        //public int AdvertisePeriodTime => Convert.ToInt32(ConfigurationManager.AppSettings["AdvertisePeriodTime"]) * 1000 * 60;
        //public DateTime NextAdvertisePeriodTime { get; set; }
        //public string CameraUserName => ConfigurationManager.AppSettings["CameraUserName"];
        //public string CameraPassword => ConfigurationManager.AppSettings["CameraPassword"];
        //public EnumNVRType NVRType => (EnumNVRType)Convert.ToByte(ConfigurationManager.AppSettings["NVRType"]);
        //public EnumGPSType GPSType => (EnumGPSType)Convert.ToByte(ConfigurationManager.AppSettings["GPSType"]);
        //public string GPSPort => ConfigurationManager.AppSettings["GPSPort"];
        //public EnumServerApplicationType ServerApplicationType => (EnumServerApplicationType)Convert.ToByte(ConfigurationManager.AppSettings["ServerApplicationType"]);
        //public string WCIraniIP => "10.0.47.15";
        //public string WCFarangiIP => "10.0.47.16";
        //public IPAddress ServerIP => IPAddress.Parse(ConfigurationManager.AppSettings["ServerIP"]);
        //public bool FlipchartSentA { get; set; } = false;
        //public bool FlipchartSentB { get; set; } = false;

        //public string FtpSourcePath => ConfigurationManager.AppSettings["FtpSourcePath"];
        //public string FtpDestinationPath => ConfigurationManager.AppSettings["FtpDestinationPath"];
        //public string AdvertiseDirectoryName => ConfigurationManager.AppSettings["AdvertiseDirectoryName"];
        //public string TravelInfo => ConfigurationManager.AppSettings["TravelInfo"];
        //public bool AutoLogin => Convert.ToBoolean(ConfigurationManager.AppSettings["AutoLogin"]);
        //public bool IsLoggedin { get; set; } = false;
        //public bool HasMicrophone => Convert.ToBoolean(ConfigurationManager.AppSettings["HasMicrophone"]);
        //public int ClientKeepAliveInterval => Convert.ToInt32(ConfigurationManager.AppSettings["ClientKeepAliveInterval"]);
        //public string AspectRatio => ConfigurationManager.AppSettings["AspectRatio"];

        public IDictionary<KeyMessage, EnumDevice> DeviceError = new Dictionary<KeyMessage, EnumDevice>
        {
            //{ KeyMessage.No_sensor_connected,EnumDevice.Oximeter },
            //{ KeyMessage.No_finger_in_probe,EnumDevice.Oximeter },
            //{ KeyMessage.Low_perfusion,EnumDevice.Oximeter },

            //{ KeyMessage.Low_O2_pressure,EnumDevice.Flow },
            //{ KeyMessage.Low_Air_pressure,EnumDevice.Flow }
        };

        public void SaveSettings(string key, string value)
        {
            // Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // config.AppSettings.Settings[key].Value = value;
            // config.Save(ConfigurationSaveMode.Full);
            // ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
