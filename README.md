# HCommAir
Hantas air tool device communication library

## How to use

### 1. Common
1. Install using nuget.(https://www.nuget.org/packages/HCommAir/)
2. Add 'using' HComm and HCommAir reference.
```c#
using HComm.Common;
using HCommAir;
using HCommAir.Manager;
using HCommAir.Tools;
```
3. Create the HCommInterface object like this.
```c#
private HCommAirInterface HCommAir { get; } = new HCommAirInterface();
```
4. Set event and implement event
```c#
HCommAir.ChangedConnect += OnChangedConnect;
HCommAir.ReceivedMsg += OnReceivedMsg;
...
private void OnChangedConnect(HcToolInfo info, ConnectionState state){...}
private void OnReceivedMsg(HcToolInfo info, Command cmd, int addr, int[] values){...}
```
5. Start
```c#
HCommAir.Start();
```

### 2. Register / UnRegister tools

1. Get scanned/register tool list
```c#
// get all scanned tools
var scanTools = HCommAir.GetScannedTools();
// get all registered tools
var registerTools = HCommAir.GetRegisteredTools();
```

2. Register
```c#
// get scan tool first item
var item = scanTools[0];
// register item
HCommAir.RegisterTool(item);
// save file (path = user custom)
HCommAir.SaveRegisterTools(path);
```

3. Un-Register
```c#
// get register tool first item
var item = registerTools[0];
// register item
HCommAir.UnRegisterTool(item);
// save file (path = user custom)
HCommAir.SaveRegisterTools(path);
```

4. Load register tool
```c#
// load file (path = user custom)
HCommAir.LoadRegisterTools(path);
```

### 3. Session

1. Get session
```c#
// get all sessions
var allSession = HCommAir.GetAllSessions();
// get session (item = tool information item)
var session = HCommAir.GetSession(item);
```

2. Used command
```c#
session.GetParam(1, 10);          // GET parameter values. Start address = 1, Count = 10
session.SetParam(1, 0);           // SET parameter value. Set address = 1, value = 0
session.GetInfo();                // GET device information (Automatically called when a command is not transmitted for a certain period of time while connected to the device.)
session.SetRealTime(4002, 1);     // SET event real-time monitoring event value = 0 (stop), value = 1 (start)
session.SetGraph(4100, 1);        // SET event graph monitoring event value = 0 (stop), value = 1 (start)
session.GetState(3300, 14);       // GET current tool status
session.GetGraph(4200, 1);        // GET graph monitoring data (AD Only)
```

3. Received message implement
```c#
private void OnReceivedMsg(HcToolInfo info, Command cmd, int addr, int[] values)
{
    // tool information
    Console.WriteLine($@"IP address: {info.Ip}");
    Console.WriteLine($@"MAC: {info.Mac}");
    // check command
    switch( cmd )
    {
        case Command.Read:
            // parameter READ acknowledge
            break;
        case Command.Mor:
            // device monitoring data
            break;
        case Command.Write:
            // parameter WRITE acknowledge
            break;
        case Command.Info:
            // device INFORMATION acknowledge
            break;
        case Command.Graph:
            // graph monitoring data
            break;
        case Command.GraphRes:      // MDTC only
            // graph monitoring result data
            break;
        case Command.GraphAd:       // AD only
            // graph monitoring data
            break;
        case Command.Error:
            // error
            break;
        default:
            break;
    }
}
```

## History

v1.0.7
- HComm library version update (v1.2.7)
- Save register file bug fixed
- Network interface change function added

v1.0.6
- HComm library version update (v1.2.6)

v1.0.5
- Session USB serial port connect function added

v1.0.4
- Session queue count property added

v1.0.3
- Tool session max queue size and max block size bug fixed

v1.0.2
- Tool session remove bug fixed

v1.0.1
- Get session list function added

v1.0.0
- Release Hantas communication library
