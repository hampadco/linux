using DataAccess.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.Enum;

namespace SerialPortReader.Interface
{
    public interface IECGDevice
    {
        //event EventHandler<ECGReceiveDataViewModel> OnReceivedDataOximeter;
        //event EventHandler<ECGReceiveDataViewModel> OnReceivedDataFlow;
        //event EventHandler<ECGReceiveDataViewModel> DeviceStatusChangedEvent;
        //event EventHandler<AutoStartDataViewModel> AutoStartStatusChangedEvent;
        //event EventHandler OnStart;
        //event EventHandler OnStop;
        //void SetValueAsync(ParameterName parameter, int number);
        //void Start(EnumDevice device);
        //void Stop(EnumDevice device);
        //IDictionary<KeyMessage, bool> DeviceModuleStatus { get; set; }
        //IDictionary<EnumDevice, bool> DeviceStartStatus { get; set; }
        //Task<bool> StartO2Calibration();
        //void Test();
        //void StartAuto(AutoPageViewModel model);
        //void StopAuto();
        //Task<bool> SendCommand(EnumDevice enumDevice, string cmd);
        Task<bool> ChangeMode(ECGMode mode);

    }
}
