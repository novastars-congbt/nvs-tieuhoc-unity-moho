using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Globalization;
public class LoginController : UIProperties
{
    [SerializeField]
     bool isLogin;
    //public bool isWebGL;
     string authUrl = "https://novastars.trannguyenhan.id.vn/api/v1/valid-token";
     int currentProgress = 0;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Slider progressSlider;
    [SerializeField] GameObject loaderUI;
    [SerializeField] GameObject errorText;
    [SerializeField] TMP_Text tmpText;

    async void Start()
    {
        DataParam.isLogin = isLogin;
        //DataParam.isWebGL = isWebGL;
         if (PlayerPrefs.HasKey("ExpiredTime"))
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                long lastimeLogin = Convert.ToInt64(PlayerPrefs.GetString("ExpiredTime"));
                DateTime savedDateTime = new DateTime(lastimeLogin);
                DateTime currentDateTime = DateTime.Now;

                if (currentDateTime < savedDateTime) {
                    SceneManager.LoadScene("Menu");
                }
            } else {
                if (PlayerPrefs.HasKey("Token")) {
                    string token = PlayerPrefs.GetString("Token");
                    Debug.Log(token);
                    bool isAuth =  await Auth(token);
                    if (isAuth) {
                        SceneManager.LoadScene("Menu");
                    }
                }

            }
        }
        
    }


    public void BtnClose()
    {
        ShowCloseAppPanel();
    }

    public async void BtnLogin()
    {
        if (!isLogin) SceneManager.LoadScene("Menu");
        else
        {
            errorText.SetActive(false);
            Debug.Log(inputField.text);
            currentProgress = 0;

            bool isAuth = await Auth(inputField.text);
            if (isAuth)
            {
                PlayerPrefs.SetString("Token", inputField.text);
                DateTime now = DateTime.Now;
                long timestamp = now.Ticks;
                PlayerPrefs.SetString("LoginTime", timestamp.ToString());
                PlayerPrefs.Save();
                SceneManager.LoadScene("Menu");
            }
            else
            {
                errorText.SetActive(true);
            }
        }
    }

    public async Task<bool> Auth(string code)
    {
        loaderUI.SetActive(true);

        string IP = await GetPublicIPAddress();
        string cpu_computer = SystemInfo.processorType;
        string ram_computer = SystemInfo.systemMemorySize.ToString();
        string vendor_computer = SystemInfo.deviceModel;

        ApiRequest requestData = new ApiRequest
        {
            token = code,
            ip = IP,
            ram_computer = ram_computer,
            cpu_computer = cpu_computer,
            vendor_computer = vendor_computer,
            software_type = 1,
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log(jsonData);
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(authUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            var operation =  webRequest.SendWebRequest();

            while (!operation.isDone) {
                await Task.Yield();
                if (currentProgress < 100) {
                    currentProgress = currentProgress + 1;
                    progressSlider.value = currentProgress;
                }
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                loaderUI.SetActive(false);
                Debug.LogError(webRequest.error);
                return false;
            }
            else
            {
                loaderUI.SetActive(false);
                Debug.Log(webRequest.downloadHandler.text);
                ResponseData errorResponse = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                Debug.Log(errorResponse.account_type);
                Debug.Log(errorResponse.account_type.lesson_list);

                // Access the error field
                bool isError = errorResponse.error;
                tmpText.text = errorResponse.message; 
                string lessonList = errorResponse.account_type.lesson_list;
                if (!isError) {
                    DateTime expiredDate = DateTime.Parse(errorResponse.time_expired);
                    long timestamp = expiredDate.Ticks;
                    PlayerPrefs.SetString("ExpiredTime", timestamp.ToString());
                    PlayerPrefs.SetString("LessonList", lessonList);
                    PlayerPrefs.SetInt("Graced", 0);
                    PlayerPrefs.Save();
                    Debug.Log("ExpiredTime");
                    Debug.Log(timestamp.ToString());
                }
                return !isError;
            }
        }
    }

    public async Task<string> GetPublicIPAddress()
    {
        string apiUrl = "https://api.ipify.org";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone) {
                await Task.Yield();
                if (currentProgress < 100) {
                    currentProgress = currentProgress + 1;
                    progressSlider.value = currentProgress;
                }
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                return "192.168.0.1";
            }
            else
            {
                string publicIP = webRequest.downloadHandler.text;
                Debug.Log("Public IP: " + publicIP);
                return publicIP;
            }
        }
    }
}


public class ApiRequest
{
    public string token;
    public string ip;
    public string ram_computer;
    public string cpu_computer;
    public string vendor_computer;
    public int software_type;
}

public class ResponseData
{
    public bool error;
    public string message;
    public string time_expired;
    public AccountType account_type;
}

[System.Serializable]
public class AccountType
{
    public int id;
    public string name;
    public string description;
    public DateTime created_at;
    public DateTime updated_at;
    public string lesson_list;
    public int software_type;
}