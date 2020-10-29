# Buttplug Web Bridge

# Running
In the project root folder use the command:
```dotnet run```

By default, it will run on locahost:5000, to change the ip/port:
```dotnet run --urls "http://ip:port"```

You can also edit & use Start.bat, it will bind to all interfaces and listen on port 27015

You will most likely have to setup port-forwarding if you want the web bridge to be accessed remotely.

Ctrl+C to exit.

# Safety
I realize that this might be important to outline:

Using this web bridge **requires revealing your computer's public IP** to the program that will interface with it.

I made this program to control toys from "Second Life" scripts which effectively act as a 3rd party "anonymity buffer", this solution is acceptable for me, but might not be for you, please be safe.

## Security
* You can set a **SecretKey** in **appsettings.json**.
* If no **SecretKey** is set, a temporary key will be generated (and displayed in the console window).
* To connect to the Web bridge, apps have to supply a header parameter named "SecretKey" with the proper value.

## Privacy
You can hide the actual model of each registered device in **appsettings.json**.
* If no **DeviceNameCloaking** is set, entries in **NameCloakingTable** will be replaced by their respective value.
* Values in **NameCloakingTable** don't have to be unique (duplicates will be given a unique number).

## API
Examples will use localhost:5000 as the Web Bridge address.

### Devices
The device APIs have general structure of `/api/Device/{Action}/{Devicename}/{argument}`
and is defined in `Controllers/DevicesController.cs`

#### Get a list of devices
```
http://localhost:5000/api/Device/List
```

#### Querying a specific device status
```
http://localhost:5000/api/Device/Info/Lovense%20Hush
```
Currently returns supported features for the queried device

#### Set global vibration level (all vibration motors)
```
http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/0
http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/50
http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/100
```

#### Set vibration level on each vibrator independently
Note: You have to set the speed of all vibrators at once, the number of supplied values MUST match VibrateCmd Feature.

Example for VibrateCmd = 1:
```
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/0
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/50
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/100
```

Example for VibrateCmd = 2:
```
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/0,100
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/50,50
http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/100,0
```

#### Shut down the device
```
http://localhost:5000/api/Device/StopDeviceCmd/Lovense%20Hush
```

#### Status Codes
Errors are indicated with HTTP response status codes.

##### 200 Ok
The command/query has been executed successfully.

##### 401 Ok
Access denied, make sure that you supplied a valid access key.

##### 400 Bad Request
The requested device doesn't support this function.

##### 404 Not Found
The requested device doesn't exist (or the URL is invalid).