using System.Net;
using System.IO;
using UnityEngine;
using System.Text;

public class APIHandler: MonoBehaviour
{

    private string endpoint;
    private string access_token;

    // Awake is the first method called in the lifecylce of a script.
    // This code will be run before anything else.
    public void Awake()
    {
        endpoint = "http://localhost:1337/api";
        access_token = LoadAcessToken();

        string call = endpoint + "/problems?access_token=" + access_token;


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(call);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            Debug.Log(reader.ReadToEnd());
        }
    }

    public int Login(string username, string password)
    {
        endpoint = "http://localhost:1337/api";
        access_token = LoadAcessToken();
    
        string call = endpoint + "/student_accounts?access_token=" + access_token + "&user=" + username + "&pass=" +password ;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(call);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            Debug.Log(reader.ReadToEnd());
            return (int)response.StatusCode;
        }
    }
    public int SignUp(string studenID,string fullname, string username,string password, string email,string teacherID)
    {
        endpoint = "http://localhost:1337/api";
        access_token = LoadAcessToken();

        string call = endpoint + "/student_accounts?access_token=" + access_token;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(call);
        request.Method = "POST";
        string postData = "&studentID=" + studenID + "&Password=" + password 
                          + "&Username=" + username + "&Password=" + password
                          + "&Email=" + email + "&TeacherID=" + teacherID;
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream dataStream = request.GetRequestStream();
        // Write the data to the request stream.  
        dataStream.Write(byteArray, 0, byteArray.Length);
        // Close the Stream object.  
        dataStream.Close();
        dataStream = response.GetResponseStream();
        using (StreamReader reader = new StreamReader(dataStream)) {
            Debug.Log(reader.ReadToEnd());
            return (int)response.StatusCode;
        }
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