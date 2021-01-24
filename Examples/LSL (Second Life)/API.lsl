/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

//Public ip/domain and port to access the Toy Web Bridge.
string address = "somedomain.someip";
integer port = 5000;

//The access key expected by the Toy Web Bridge
string access_key = "CHANGEME";


string CMD_LIST         = "List";
string CMD_INFO         = "Info";
string CMD_STOP         = "StopDeviceCmd";
string CMD_VIBRATE      = "VibrateCmd";
string CMD_VIBRATE_MULTI= "SingleMotorVibrateCmd";

integer Clamp(integer v)
{
    if(v > 100) v = 100;
    else if(v < 0) v = 0;
    return v;
}

//Lists all devices currently available
key ListDevices()
{
    return SendQuery(CMD_LIST,"","");
}
//Returns detail informations about <device>
key Info(string device)
{
    return SendQuery(CMD_INFO,device,"");
}
//Stops <device>
key StopDeviceCmd(string device)
{
    return SendQuery(CMD_STOP,device,"");
}
//Set <speed> (0-100) on all vibration motors of <device>
key VibrateCmd(string device, integer speed)
{
    return SendQuery(CMD_VIBRATE,device,(string)Clamp(speed));
}

//Set <speed> (0-100) on all vibration motors, independently.
//<i_speeds> has to have an entry for each available motor.
key SingleMotorVibrateCmd(string device, list i_speeds)
{
    integer i = llGetListLength(i_speeds);
    list o_speeds;
    while(i--)
        o_speeds+=Clamp(llList2Integer(i_speeds,i));

    return SendQuery(CMD_VIBRATE_MULTI,device,(string)llDumpList2String(o_speeds,","));
}

key SendQuery(string action, string device, string argument)
{
    list url = [port,"api","Device",action];
    if(device) url += device;
    if(argument) url += argument;

    key query_id = llHTTPRequest("http://"+address+":"+llDumpList2String(url,"/"),
                                [HTTP_METHOD,"GET",HTTP_MIMETYPE,"text/x-www-form-urlencoded",
                                 HTTP_CUSTOM_HEADER,"SecretKey",access_key,
                                 HTTP_VERBOSE_THROTTLE,FALSE],"");

    //This is not strictly required but there is a maximum rate
    //of 25 requests in 20 seconds per object (1 query every 1.25s).
    llSleep(1.3);
    return query_id;
}

default
{
    state_entry()
    {
        ListDevices();
        Info("Device Name");
        VibrateCmd("Device Name",20);
        SingleMotorVibrateCmd("Device Name",[10,5]);
        StopDeviceCmd("Device Name");
    }
    http_response(key id, integer status, list meta, string body)
    {
        if(llJsonValueType(body,["Action"]) == JSON_INVALID)
        {
            if(status == 404)
                llOwnerSay("Web Bridge not found.");
            else if(status == 401)
                llOwnerSay("Access key rejected.");
            return;
        }

        string command = llJsonGetValue(body,["Action"]);
        string device = llJsonGetValue(body,["Device"]);
        if(command == CMD_LIST)
        {
            string devices = llJsonGetValue(body,["Devices"]);
            return;
        }

        if(status == 404)
        {
            llOwnerSay("Device '"+device+"' does not exist.");
            return;
        }

        if(command == CMD_INFO)
        {
            string message = "Features supported by device '"+device+"' :\n";

            list features = llJson2List(llJsonGetValue(body,["Features"]));
            if(features == [])
            {
                llOwnerSay(message + "none!");
                return;
            }

            integer i;
            list features_key;
            list features_value;
            for(;i<llGetListLength(features);i+=2)
            {
                features_key += llList2String(features,i);
                features_value += llList2String(features,i+1);
            }
            message += llList2CSV(features_key);

            integer vc_index = llListFindList(features_key,["VibrateCmd"]);
            if(vc_index !=-1)
            {
                integer motor_count = llList2Integer(features_value,vc_index);
                message += "\nMotor count: "+ (string)motor_count;
            }

            llOwnerSay(message);
            return;
        }

        if(status != 200)
        {
            llOwnerSay("Device '"+device+"' does not support the command '"+command+"'.");
            return;
        }

        if(command == CMD_STOP)
        {
            llOwnerSay("Device '"+device+"' stopped.");
            return;
        }

        string speed = llJsonGetValue(body,["Speed"]);

        if(command == CMD_VIBRATE)
        {
            llOwnerSay("Device '"+device+"' is vibrating at: "+speed+"%");
        }
        else if(command == CMD_VIBRATE_MULTI)
        {
            list speeds = llJson2List(speed);
            llOwnerSay("Device '"+device+"' motors are vibrating at the following speeds: "+llDumpList2String(speeds,"%,")+"%");
        }
    }
}
