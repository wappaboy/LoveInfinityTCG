using TMPro;
using UnityEngine;

public class DeveloperConsoleVisualizer : MonoBehaviour
{
    public GameObject _debugLogCanvas;
    public TextMeshProUGUI consoleText;

    void Start()
    {
        _debugLogCanvas.SetActive(false);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(type == LogType.Warning) return;
        consoleText.text += logString + '\n';
    }
    
    void Update()
    {
        //Ctrl + Shift +D でDeveloperConsoleを表示
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            _debugLogCanvas.SetActive(!_debugLogCanvas.activeInHierarchy);
        }
    }
}
