using System.Net;
using System.IO;
using UnityEngine;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class APIHandler
{

    private static string endpoint = "http://192.168.0.6:1337/api";
    private string access_token;

    public APIHandler()
    {
        LoadAcessToken();
    }

    public int Login(string username, string password)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            {"user", username},
            {"pass", password}
        };

        try
        {
            JObject result = get("/student_accounts", parameters);
            return (int)result["status"];
        } catch(Exception e)
        {
            Debug.Log(e);
            return 401;
        }
    }

    public async Task<int> SignUpAsync(string studentID, string fullname, string username, string password, string email, string teacherID)
    {
        HttpClient client = new HttpClient();
        string call = endpoint + "/student_accounts";
        var values = new Dictionary<string, string>
        {
            {"StudentID", studentID },
            {"Fullname", fullname },
            {"Username",username },
            {"Password",password },
            {"Email",email },
            {"TeacherID",teacherID },
            {"access_token",access_token}
        };
        FormUrlEncodedContent content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(call, content);
        var responseString = await response.Content.ReadAsStringAsync();

        return (int)response.StatusCode;


    }

    public JObject get(string path, Dictionary<string, string> parameters)
    {
        string call = endpoint + path + "?access_token=" + access_token;

        if (parameters != null && parameters.Count > 0)
        {
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                call += $"&{entry.Key}={entry.Value}";
            }
        }

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(call);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            string jsonStr = reader.ReadToEnd();
            JObject res = new JObject();
            res.Add("status", 200);
            res.Add("res", JArray.Parse(jsonStr));

            return res;
        }
    }

    private void LoadAcessToken()
    {
        try
        {
            TextAsset authFile = Resources.Load<TextAsset>("auth");
            string json = authFile.text;
            Auth auth = JsonUtility.FromJson<Auth>(json);
            access_token = auth.access_token;
        }
        catch (FileLoadException e)
        {
            Debug.Log(e.Message);
        }
    }
}


public class Auth
{
    public string access_token;
}