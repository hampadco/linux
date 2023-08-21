using DataAccess.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.Enum;

namespace SerialPortReader
{
    public interface IBlenderDevice
    {
        event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataOximeter;
        event EventHandler<BlenderReceiveDataViewModel> OnReceivedDataFlow;
        event EventHandler<BlenderReceiveDataViewModel> DeviceStatusChangedEvent;
        event EventHandler<AutoStartDataViewModel> AutoStartStatusChangedEvent;
        event EventHandler<EnumDevice> OnStart;
        event EventHandler<EnumDevice> OnStop;
        void SetValueAsync(ParameterName parameter, int number);
        void Start(EnumDevice device);
        void Stop(EnumDevice device);
        IDictionary<KeyMessage, bool> DeviceModuleStatus { get; set; }
        IDictionary<EnumDevice, bool> DeviceStartStatus { get; set; }
        Task<bool> StartO2Calibration();
        void Test();
        void StartAuto(AutoPageViewModel model);
        void StopAuto();
        Task<bool> SendCommand(EnumDevice enumDevice, string cmd);
        void OnCloseErrorDialog();

    }
}
