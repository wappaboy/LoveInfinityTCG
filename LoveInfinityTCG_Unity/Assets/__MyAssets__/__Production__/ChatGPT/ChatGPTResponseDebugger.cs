using UnityEngine;

public class ChatGPTResponseDebugger : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool _debugEnable;
    [SerializeField, TextArea(5, 10)] string _chatLogs;
    [SerializeField, TextArea(5, 10)] string _summary;
    [SerializeField, TextArea(5, 10)] string _promptSample;
    bool _creating;
    
    async void Update()
    {
        //chatLogから要約文を生成
        if (_debugEnable && !_creating && Input.GetKeyDown(KeyCode.S))
        {
            _creating = true;
            _summary = await ChatGPTResponseTuner.Instance.GetSummaryAsync(_chatLogs);
            _creating = false;
        }

        //要約文からPromptを生成
        if (_debugEnable && !_creating && Input.GetKeyDown(KeyCode.P))
        {
            _creating = true;
            var prompt = await ChatGPTResponseTuner.Instance.GetPromptForStableDiffusionAsync(_summary, _promptSample);
            _creating = false;
        }
    }
}
