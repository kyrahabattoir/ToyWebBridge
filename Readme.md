# Buttplug Web Bridge

# Running

In the project root folder use the command:
```dotnet run```

By default, it will run on locahost:5000, to change the ip/port:
```dotnet run --urls "http://ip:port"```

You can also edit & use Start.bat, it will bind to all interfaces and listen on port 27015

You will most likely have to setup port-forwarding if you want the web bridge to be accessed remotely.

Ctrl+C to exit.

## API

Examples will use localhost:5000 as the Web Bridge address.

### Devices

The device APIs have general structure of `/Device/{device}/{function}/{argument}`
and is defined in `Controllers/DevicesController.cs` and also `Controllers/Devices/`

#### Get a list of devices
```
http://localhost:5000/Device/
```

#### Querying a specific device
```
http://localhost:5000/Device/Lovense%20Hush
```

#### Set vibrator level (all motors)
```
http://localhost:5000/Device/Lovense%20Hush/VibrateCmd/50
```

#### Shut down the device
```
http://localhost:5000/Device/Lovense%20Hush/StopDeviceCmd
```

#### Status Codes
Errors are indicated with HTTP response status codes.

##### 200 Ok
The command/query has been executed successfully.

##### 400 Bad Request
The requested device doesn't support this function.

##### 404 Not Found
The requested device doesn't exist.
