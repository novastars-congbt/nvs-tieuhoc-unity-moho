using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;
using System.Text;
public class ListLessonController : UIProperties
{
    //public Transform specialButtons;
    [SerializeField]
     Button btnBackClass;
    [SerializeField]
     TextMeshProUGUI textMesh;
    //public Button btnBackLesson;
    //public List<GameObject> titles = new List<GameObject>();
    [SerializeField]
     List<LessonSelector> lessons = new List<LessonSelector>(); 

    private List<bool> availableLessons;
    string authUrl = "https://novastars.trannguyenhan.id.vn/api/v1/valid-token";
    Data data;
    MenuController menuController;

    private void Awake()
    {
        data = DataController.instance.data;
        menuController = MenuController.instance;
    }

    public async void Display()
    {
        //if (MenuController.currentClass < 3)
        //{
        //    foreach (var title in titles)
        //    {
        //        title.SetActive(false);
        //    }

        //    titles[MenuController.currentClass].SetActive(true);
        //}

        if (Application.internetReachability != NetworkReachability.NotReachable) { 
            if (PlayerPrefs.HasKey("Token")) {
                string token = PlayerPrefs.GetString("Token");
                Debug.Log(token);
                await Auth(token);
            }
        }

        int lessonLength = lessons.Count;
        availableLessons =  new List<bool> (new bool[lessonLength]);

        string lessonString = "";
        if (PlayerPrefs.HasKey("LessonList")) {
            lessonString = PlayerPrefs.GetString("LessonList");
        }
        if (!string.IsNullOrEmpty(lessonString)) {
                List<string> numbers = lessonString.Split(',')
                        .ToList();
                for (int i = 0; i < numbers.Count; i ++) {
                    string[] parts = numbers[i].Split('.');
                    int beforeComma = int.Parse(parts[0]);
                    int afterComma = int.Parse(parts[1]);
                    if (beforeComma == MenuController.currentClass + 1) {
                        Debug.Log(afterComma - 1);
                        availableLessons[afterComma - 1] = true;
                    }
                }
        }

        textMesh.text = "LỚP " + (MenuController.currentClass + 1);
        foreach (var lesson in menuController.lessons)
        {
            lesson.gameObject.SetActive(false);
        }
        menuController.lessons.Clear();
        for (int i = 0; i < data.typeClasses[MenuController.currentClass].typeLessons.Length; i++)
        {
            lessons[i].lesson = i;
            lessons[i].gameObject.SetActive(true);
            Button button = lessons[i].GetComponent<Button>();
            if (button != null)
            {
                if (availableLessons[i] == false && DataParam.isLogin) {
                    button.interactable = false;
                } else {
                    button.interactable = true;
                }
            }
            menuController.lessons.Add(lessons[i]);
        }
    }

    public void BtnTopic()
    {
        SetCurrentClass();
        //specialButtons.gameObject.SetActive(false);
        btnBackClass.gameObject.SetActive(false);
        //btnBackLesson.gameObject.SetActive(true);
        Display();
    }

    public void BtnBackLesson()
    {
        SetCurrentClass();
        //specialButtons.gameObject.SetActive(true);
        btnBackClass.gameObject.SetActive(true);
        //btnBackLesson.gameObject.SetActive(false);
        Display();
    }

    void SetCurrentClass()
    {
        switch (MenuController.currentClass)
        {
            case 0:
                MenuController.currentClass = 3;
                break;
            case 1:
                MenuController.currentClass = 4;
                break;
            case 2:
                MenuController.currentClass = 5;
                break;
            case 3:
                MenuController.currentClass = 0;
                break;
            case 4:
                MenuController.currentClass = 1;
                break;
            case 5:
                MenuController.currentClass = 2;
                break;
        }
    }

    public async Task<bool> Auth(string code)
    {

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
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
                return false;
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                ResponseData errorResponse = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                Debug.Log(errorResponse.account_type);
                Debug.Log(errorResponse.account_type.lesson_list);

                // Access the error field
                bool isError = errorResponse.error;
                if (!isError) {
                    string lessonList = errorResponse.account_type.lesson_list;
                    DateTime expiredDate = DateTime.Parse(errorResponse.time_expired);
                    long timestamp = expiredDate.Ticks;
                    PlayerPrefs.SetString("ExpiredTime", timestamp.ToString());
                    PlayerPrefs.SetString("LessonList", lessonList);
                    PlayerPrefs.Save();
                    Debug.Log("ExpiredTime");
                    Debug.Log(timestamp.ToString());
                    return true;
                } else {
                    if (PlayerPrefs.HasKey("ExpiredTime"))
                    {
                        long lastimeLogin = Convert.ToInt64(PlayerPrefs.GetString("ExpiredTime"));
                        DateTime savedDateTime = new DateTime(lastimeLogin);
                        DateTime currentDateTime = DateTime.Now;
                        int hadGraced = PlayerPrefs.GetInt("Graced", 0);

                        if (!(currentDateTime < savedDateTime && hadGraced == 1)) {
                            ShowExpiredPopUp();
                        }
                    }
                    return false;
                }
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
