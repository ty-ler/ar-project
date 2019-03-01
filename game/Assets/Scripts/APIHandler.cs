using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class APIHandler: MonoBehaviour
{

    private string endpoint;
    private string access_token;

    // Awake is the first method called in the lifecylce of a script.
    // This code will be run before anything else.
    void Awake()
    {
        endpoint = "localhost:1337/api";
        access_token = LoadAcessToken();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string LoadAcessToken()
    {
        string access_token = null;
        try {
            using (StreamReader reader = new StreamReader("../webserver/auth.json"))
            {
                string json = reader.ReadToEnd();
                Auth auth = JsonUtility.FromJson<Auth>(json);
                access_token = auth.access_token;
            }
        } catch(FileLoadException e)
        {
            Debug.Log(e.Message);
        }

        return access_token;
        
    }
}


public class Auth
{
    public string access_token;
}