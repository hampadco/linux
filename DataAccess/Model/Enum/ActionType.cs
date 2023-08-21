using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.Enum
{
    public enum ActionType : int
    {
        StartApp,
        StartDevice,
        LoggedInUser,
        ChangeFlowValue,
        ChangeOxygenValue,
        OpenPortError = 100,
        DeviceStatusChanged,
        Exception,
        DeviceModuleStatusChanged,
        AutoStartUp,
        AutoStartDown,
        StartAuto,
        StopAuto
    }
}
