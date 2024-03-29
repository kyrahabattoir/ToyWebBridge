/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */

//Public ip/domain and port to access the Toy Web Bridge.
string address = "somedomain.someip";
integer port = 5000;

//Device settings.
string device = "Device Name";
integer speed = 25; //from 0 (off) to 100 (max vibration)

//The access key expected by the Toy Web Bridge
string access_key = "CHANGEME";

integer Clamp(integer v)
{
    if(v > 100) v = 100;
    else if(v < 0) v = 0;
    return v;
}

key SendQuery(string action, string device, string argument)
{
    list url = [port,"api/Device",action];
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

key queryId;
integer on = FALSE;
default
{
    touch_start(integer total)
    {
        if(llDetectedKey(0) != llGetOwner()) return; //Only the object onwer can use it.
        if(queryId) return;     //Ignores clicks until we processed the Web Bridge reply.

        on = !on;
        if(on)
            queryId = SendQuery("VibrateCmd",device,(string)Clamp(speed*on));
        else
            queryId = SendQuery("StopDeviceCmd",device,"");
    }
    http_response(key id, integer status, list meta, string body)
    {
        if(id != queryId) return;
        queryId = NULL_KEY;

        //If no action is set it is either an access or autorization issue.
        if(llJsonValueType(body,["Action"]) == JSON_INVALID)
        {
            if(status == 404)
                llOwnerSay("Web Bridge/Command not found.");
            else if(status == 401)
                llOwnerSay("Access key rejected.");
            return;
        }

        //404 at this stage means device not found.
        if(status == 404)
        {
            llOwnerSay("Device does not exist.");
            return;
        }
        if(status != 200)
        {
            llOwnerSay("Device does not support vibration commands.");
            return;
        }

        string command = llJsonGetValue(body,["Action"]);
        integer speed = (integer)llJsonGetValue(body,["Speed"]);

        if( (command == "VibrateCmd") && speed)
        {
            llOwnerSay("Vibrating at: "+(string)speed+"%");
            return;
        }

        else if(command == "StopDeviceCmd" || command == "VibrateCmd")
        {
            llOwnerSay("Stopped");
            return;
        }

        llOwnerSay("!Unparsed return data!");
        llOwnerSay("Status: " + (string)status);
        llOwnerSay("Body: " + body);
    }
}
