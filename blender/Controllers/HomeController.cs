using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.IO.Ports;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using System.Text;
using SerialPortReader;
using ViewModel;

namespace blender.Controllers;


public class HomeController : Controller
{
    private readonly IBlenderDevice _device;
    private readonly IHubContext<DataHub> _hubContext;

    public HomeController(IHubContext<DataHub> hubContext)
    {
        _hubContext = hubContext;
        _device = BlenderDevice.Instance;
        _device.OnReceivedDataFlow += Device_OnReceivedDataFlow;
        _device.DeviceStatusChangedEvent += Device_DeviceStatusChanged;
        _device.AutoStartStatusChangedEvent += Device_AutoStartStatusChangedEvent;
    }

    public IActionResult Index()
    {
        //Check();


        return View();
    }

    public IActionResult privacy()
    {
        return View();
    }


    public IActionResult Gauge()
    {
        // TODO: Your code here
        return View();
    }
    public IActionResult Gaugetwo()
    {
        // TODO: Your code here
        return View();
    }


    private Timer _dataUpdateTimer;
    string Pulse = "0";
    string So = "0";
    string Oxigen = "0";
    string Flow = "0";

    public async Task StartReading()
    {
        //_device.Start(ViewModel.Enum.EnumDevice.Flow);
        //_device.Start(ViewModel.Enum.EnumDevice.Oxygen);
        _device.Start(ViewModel.Enum.EnumDevice.Oximeter);
        _device.OnReceivedDataOximeter += Device_OnReceivedDataOximeter;

    }

    private async Task SendDataToClients()
    {


        await _hubContext.Clients.All.SendAsync("ReceiveData", Pulse, So, Oxigen, Flow);
    }





    public IActionResult Check()
    {

        Console.WriteLine("Start Check");

        try
        {
            //device = SimulateBlenderDevice.Instance;
            //device.Stop();
            //device.Start();
            //device = SimulateBlenderDevice.Instance;
            //var device2 = ECGDevice.Instance;
        }
        catch (Exception ex)
        {

        }





        // TODO: Your code here
        return View();
    }
    private async void Device_OnReceivedDataOximeter(object sender, BlenderReceiveDataViewModel e)
    {
        Pulse=e.OximeterValue.ToString(); So=e.SPo2.ToString(); Oxigen=e.Oxygen.ToString(); Flow=e.FlowValue.ToString();
        await SendDataToClients();
        System.Console.WriteLine("InDebug Control: {0}", e.OximeterValue);
        var data = e;

        // if (isLive)
        // {
        //     Values.Add(new MeasureModel
        //     {
        //         ElapsedMilliseconds = UnixTime,
        //         Value = data.OximeterValue
        //     });
        //     if (Values.Count > 200)
        //         Values.RemoveAt(0);
        //     HR = data.HR;
        //     SPo2 = data.SPo2;
        // }
        return;
    }
    private async void Device_OnReceivedDataFlow(object sender, BlenderReceiveDataViewModel e)
    {
        Pulse=e.OximeterValue.ToString(); So=e.SPo2.ToString(); Oxigen=e.Oxygen.ToString(); Flow=e.FlowValue.ToString();
        await SendDataToClients();
        //cw convert convert.jasonconvert.ser
        System.Console.WriteLine("InDebug Control: {0}", e.Temp);

        var data = e;
        // if (BtnSetOxygen.IsPlaying)
        //     Oxygen = (int)data.Oxygen;
        // if (BtnSetFlow.IsPlaying)
        //     Flow = data.FlowValue < 5 ? (float)Math.Round(data.FlowValue, 1) : (float)Math.Round(data.FlowValue, 0);
    }
    private void Device_DeviceStatusChanged(object sender, BlenderReceiveDataViewModel e)
    {
        Console.WriteLine("InDebug Control: {0}", e.ExtraData);
        // if (e.ExtraData.Contains(KeyMessage.Oximeter_No_finger_in_probe.ToString()))
        // {
        //     SPo2 = 0;
        //     HR = 0;
        // }
        // if (e.Device == EnumDevice.Oximeter)
        // {
        //     if (e.ExtraData != "OK" && e.ExtraData != ActionType.StartDevice.ToString())
        //     {
        //         IsHeartBeating = false;
        //         LastError = $"{e.Device.ToString()}: {e.ExtraData}";
        //     }
        // }
        // else if (e.Device == EnumDevice.Flow)
        // {
        //     if (e.ExtraData != "OK" && e.ExtraData != ActionType.StartDevice.ToString())
        //         LastError = $"{e.Device.ToString()}: {e.ExtraData}";
        // }
        // else if (e.Device == EnumDevice.General)
        // {
        //     MakeErrors();
        // }
    }

    private void Device_AutoStartStatusChangedEvent(object sender, AutoStartDataViewModel e)
    {
        // if (sender.ToString() == "CLOSEERRORDIALOG")
        // {
        //     ShowErrorDialogTimer.Enabled = true;
        //     ShowErrorDialogTimer.Start();
        //     return;
        // }
        // if (e.NewValue > 0)
        //     SetDeviceValue(ParameterName.Oxygen, e.NewValue);
    }


public IActionResult test()
{
  //TODO: Implement Realistic Implementation
  return View();
}








}