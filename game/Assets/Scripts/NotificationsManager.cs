using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{

    void Start()
    {
        ScheduleNotifcation("Your pet is getting hungry!", "Tap to start answering questions to feed them!", 5, 1440); // 1440 mintues = 24 hours
    }

    void ScheduleNotifcation(string title, string message, int id, int delay)
    {
        NativeToolkit.ScheduleLocalNotification(title, message, id, delay, "default_sound", true, "ic_notification", "ic_notification_large");
    }


}
