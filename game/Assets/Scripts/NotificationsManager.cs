using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleAppOpened()
    {
        NativeToolkit.ScheduleLocalNotification("Test Noti", "testing!", 5, 1, "default_sound", true, "ic_notification", "ic_notification_large");
    }
}
