using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// For Add This Enum Must be To End of fileds
    /// </summary>
    public enum KeyMessage
    {
        /// <summary>
        /// Alarms
        /// </summary>
        Alarm_OverFlowValue,//Alarm
        Alarm_OverOxygenValue,//Alarm
        Alarm_OverHRValue,//Alarm
        Alarm_OverSpo2Value,//Alarm
        ///Byte 29
        Oxygen_High_O2_pressure,//Flow O2
        Oxygen_Low_O2_pressure,//Flow O2
        Flow_High_Air_pressure,//Flow
        Flow_Low_Air_pressure,//Flow
        General_High_Temperature,//Flow Sys
        General_Low_Temperature,//Flow Sys
        Oxygen_O2_Sensor_Failed,//Flow O2
        Flow_Sensor_Failed,//Flow
        ///Byte 31
        General_Internal_Error,//Flow Sys
        General_Connection_Failed,//Flow Sys
        Flow_Air_pressure_Sensor_Failed,//Flow
        Oxygen_O2_Pressure_Sensor_Failed,//Flow O2
        General__Temperature_Sensor_Failed,//Flow Sys
        Oxygen_O2_Valve_Failed,//Flow O2
        Flow_Air_Valve_Failed,//Flow
        Oxygen_ADC_Failed,//Flow O2
        ///Byte 33
        Flow_Air_leak_detected,//Flow
        Oxygen_O2_leak_detected,//Flow O2

        //USB Connection
        Oximeter_No_sensor_connected,//Oximeter
        Oximeter_No_finger_in_probe,//Oximeter
        Oximeter_Low_perfusion//Oximeter

    }
}
