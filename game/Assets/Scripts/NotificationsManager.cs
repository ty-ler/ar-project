using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager
{
    public static void ScheduleNotifcation(string title, string message, int id, int delay)
    {
        // Schedule a notificaiton
        NativeToolkit.ScheduleLocalNotification(title, message, id, delay, "default_sound", true, "ic_notification", "ic_notification_large");
    }
}
