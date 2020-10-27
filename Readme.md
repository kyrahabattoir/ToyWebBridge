# Buttplug Web Bridge

# Running

In the project root folder use the command:
```dotnet run```

By default, it will run on locahost:5000, to change the ip/port:
```dotnet run --urls "http://ip:port"```

You can also edit & use Start.bat, it will bind to all interfaces and listen on port 27015

You will most likely have to setup port-forwarding if you want the web bridge to be accessed remotely.

Ctrl+C to exit.

# Safety and privacy
I realize that this might be important to outline:

Using this web bridge **requires revealing your computer's public IP** to the program that will interface with it.

I made this program to control toys from "Second Life" scripts which effectively act as a 3rd party "anonymity buffer", this solution is acceptable for me, but might not be for you, please be safe.

## Security
The default configuration generates a random 20 characters password each time the Web Bridge is started, this password has to be supplied with all queries.

All the examples in this readme assume that the password is "mypassword".

Without the right password, any query should return a 401 Unauthorized error code.

* You can disable random password generation by setting **"UseRandomPasswords"** to false in appsettings.json
* You can set a custom static password in appsettings.json by turning off random password and setting **"Password"** to a suitable string.
* You can disable password entry entirely by turning oss random password generation and setting **"Password"** to a blank "" string.

If the password function is disabled (**THIS IS NOT RECOMMENDED**), the "pw" argument can be ommitted entirely.

## API

Examples will use localhost:5000 as the Web Bridge address.

### Devices

The device APIs have general structure of `/Device/{device}/{function}/{argument}`
and is defined in `Controllers/DevicesController.cs` and also `Controllers/Devices/`

#### Get a list of devices
```
http://localhost:5000/Device?pw=mypassword
```

#### Querying a specific device status
```
http://localhost:5000/Device/Lovense%20Hush?pw=mypassword
```
Currently returns supported features for the queried device

#### Set global vibration level (all vibration motors)
```
http://localhost:5000/Device/Lovense%20Hush/VibrateCmd?speed=0&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/VibrateCmd?speed=50&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/VibrateCmd?speed=100&pw=mypassword
```

#### Set vibration level on each vibrator independently
Note: You have to set the speed of all vibrators at once, the number of supplied values MUST match VibrateCmd Feature.

Example for VibrateCmd = 1:
```
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=0&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=50&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=100&pw=mypassword
```

Example for VibrateCmd = 2:
```
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=0,100&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=50,50&pw=mypassword
http://localhost:5000/Device/Lovense%20Hush/SingleMotorVibrateCmd?speed=100,0&pw=mypassword
```

#### Shut down the device
```
http://localhost:5000/Device/Lovense%20Hush/StopDeviceCmd?pw=mypassword
```

#### Status Codes
Errors are indicated with HTTP response status codes.

##### 200 Ok
The command/query has been executed successfully.

##### 400 Bad Request
The requested device doesn't support this function.

##### 404 Not Found
The requested device doesn't exist.
