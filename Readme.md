# Toy Web Bridge

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

## Using multiple identical devices
Using multiple identical devices is supported and duplicates will show as "Devicename 2" , "Devicename 3" and so on.
There is currently no way to uniquely identify each device under the ButtPlug API, so they are registered on a first come, first served basis.

## API
Examples use localhost:5000 as the Toy Web Bridge address and "Lovense Hush" as the device.

### Devices
The device APIs have general structure of `/api/Device/{Action}/{Devicename}/{argument}`
and is defined in `Controllers/DevicesController.cs`

#### Get a list of devices
```
GET http://localhost:5000/api/Device/List
```

#### Querying a specific device status
```
GET http://localhost:5000/api/Device/Info/Lovense%20Hush
```
Currently returns supported features for the queried device

#### Set global vibration level (all vibration motors)
```
GET http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/0
GET http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/50
GET http://localhost:5000/api/Device/VibrateCmd/Lovense%20Hush/100
```

#### Set vibration level on each vibrator independently
Note: You have to set the speed of all vibrators at once, the number of supplied values MUST match VibrateCmd Feature.

Example for VibrateCmd = 1:
```
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/0
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/50
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Hush/100
```

Example for VibrateCmd = 2:
```
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/0,100
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/50,50
GET http://localhost:5000/api/Device/SingleMotorVibrateCmd/Lovense%20Edge/100,0
```

#### Play a vibration sequence
Devices that support VibrateCmd can play vibration sequences with "SequenceVibrateCmd".
Unlike the other commands, this one requires to use a JSON encoded POST query.

* A "Time" list indicates how long each sequence step takes.
* You can control each motor independently, if only only one motor instruction list is sent,
it is assumed that you want all motors to be controlled at the same time.
* For devices with more than 2 independent motors (is there any?), ommited entries are treated as 0.

Example:
```
POST http://localhost:5000/api/Device/SequenceVibrateCmd/Lovense%20Edge
```
Payload example (single/all motors): ( raw/JSON )
```
{
   "Loop":false,
   "Time":[500,500,500,500],
   "Speeds":[
      [10,0,50,0]
   ]
}
```
Payload example (two independent motors): ( raw/JSON )
```
{
   "Loop":true,
   "Time":[500,500,500,500],
   "Speeds":[
      [10,0,50,0],
      [0,10,0,50]
   ]
}
```
* If "Loop" is true, the pattern will repeat until a new instruction is received.
* When a sequence ends, the motors will keep running at the last "Speed" value.
* Missing "Speed"values will be considered 0, "Time" determinates the sequence length.

#### Shut down the device
```
GET http://localhost:5000/api/Device/StopDeviceCmd/Lovense%20Hush
```

#### Status Codes
Errors are indicated with HTTP response status codes.

##### 200 Ok
The command/query has been executed successfully.

##### 400 Bad Request
The requested device doesn't support this function.

##### 401 Unauthorized
Access denied, make sure that you supplied a valid access key.

##### 404 Not Found
The requested device doesn't exist (or the URL is invalid).

## License

Toy Web Bridge is licensed under [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/)

![CC BY-NC-SA 4.0](https://i.imgur.com/BlZ8chD.png)
