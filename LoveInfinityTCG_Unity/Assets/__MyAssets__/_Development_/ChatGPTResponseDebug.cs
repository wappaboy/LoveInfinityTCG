using UnityEngine;

public class ChatGPTResponseDebug : MonoBehaviour
{
	//[SerializeField] ChatGPTResponseTuner _responseTuner;
	[SerializeField, TextArea(3, 10)] string _message;
	bool _gettingResponse = false;

#if UNITY_EDITOR
	void Update()
	{
		//Mキーを押したら_messageをChatGPTに投げてDebugLogに結果を出力
		if (!_gettingResponse && Input.GetKeyDown(KeyCode.M))
		{
			GetResponse();
		}
	}
#endif

	async void GetResponse()
	{
		Debug.Log("Waiting response...");
		_gettingResponse = true;
		var response = await ChatGPTResponseTuner.Instance.GetResponseAsync(_message);
		Debug.Log(response);
		_gettingResponse = false;

		//ヒロインの発現を表示し終えたらRoleをUserに戻す
		ChatGPTRole.ChangeRole(ChatGPTRole.Role.User);
	}
}