using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if !UNITY_EDITOR
using System.IO;
#endif

/// <summary>
/// GoogleCollaboratoryでStableDiffusion画像を生成しUnityで表示する
/// </summary>
public class CreateStableDiffusionImage : SingletonMonoBehaviour<CreateStableDiffusionImage>
{
	[Header("StableDiffusion")]
	[SerializeField] bool _debugActive;

	string _url;
	[SerializeField] RawImage _image;
	[SerializeField, TextArea(5, 10)] string _prompt = "warehouse entrance";
	[SerializeField, TextArea(5, 10)] string _negativePrompt = "";
	public Texture _generatedImage { get; private set; }

	[Header("GoogleDrive")] [SerializeField]
	string _folderName;

	bool _creating;
	public byte[] _savedRawFileData { get; private set; } //作成した画像のrawFileDataを保持しておく
	readonly bool _dontDestroyOnLoad = false;
	protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

	void Start()
	{
		//exeではアプリの実行ファイルの同階層の「GameData」フォルダ内のurl.txtを読み込みstabledifussionのurlとする
#if !UNITY_EDITOR
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData", "url.txt");
        _url = File.ReadAllText(filePath);
#else
		//UnityEditor上ではgitignoreされているAssets/GameDataフォルダのurl.txtを読み込みstabledifussionのurlとする
		var filePath = Path.Combine(Application.dataPath, "GameData", "url.txt");
		_url = File.ReadAllText(filePath);
#endif
	}

#if UNITY_EDITOR
	void Update()
	{
		//Iキーを押したら_promptから画像を生成し_imageに表示する
		if (_debugActive && Input.GetKeyDown(KeyCode.I) && !_creating)
		{
			CreateImage(_prompt, _negativePrompt).Forget();
		}
	}
#endif

	//会話の要約文から生成したpromptをもとに画像を生成する
	public async UniTask CreateImageFromSummary(string prompt)
	{
		await CreateImage(prompt, _negativePrompt);
	}

	//promptを引数としてGetImageAsyncを呼び出し、返り値を_imageに表示する
	async UniTask CreateImage(string prompt, string negativePrompt)
	{
		Debug.Log("Creating image...");
		_creating = true;
		try
		{
			var texture = await GetImageAsync(prompt, negativePrompt);
			_generatedImage = texture;
			_image.texture = texture;
			Debug.Log("<color=yellow>Success to create image!</color>");
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}

		_creating = false;
	}

	//promptを引数としてJsonDataを整形しPostJsonを呼び出す
	//返り値はawaitで待機しResponseJsonDataとして展開
	async UniTask<Texture> GetImageAsync(string prompt, string negativePrompt)
	{
		//JsonDataを整形
		var json = new JsonData();
		json.prompt = prompt;
		json.negative_prompt = negativePrompt;
		//PostJsonを呼び出しawaitで待機
		var url = _url + "/sdapi/v1/txt2img";
		var response = await PostJson(url, json);
		//返り値をResponseJsonDataとして展開
		var responseJson = JsonUtility.FromJson<ResponseJsonData>(response.downloadHandler.text);
		Debug.Log(response.downloadHandler.text);
		response.Dispose(); //UnityWebRequestを破棄
		//responseJsonからimageを取得しbase64をTexture型に変換
		var base64image = responseJson.images[0];
		Debug.Log(base64image);
		var imageData = Convert.FromBase64String(base64image);
		var texture = new Texture2D(1, 1);
		texture.LoadImage(imageData);

		//textureを画像ファイルとしてストレージに保存する
		//ファイル名は日付と時刻から作成する
		var bytes = texture.EncodeToPNG();
		_savedRawFileData = bytes;
		var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
		var filePath = Path.Combine(Application.persistentDataPath, fileName);
		await File.WriteAllBytesAsync(filePath, bytes);
		Debug.Log($"Save image to {filePath}");

		//テスト:GoogleDriveに画像をアップロード
		//UploadImageToGoogleDrive.UploadImage(bytes, _folderName).Forget();

		return texture;
	}

	//UnityWebrequestでjsonをPOSTする
	async UniTask<UnityWebRequest> PostJson(string url, JsonData json)
	{
		//jsonをJsonUtliityでstringに変換
		var postData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(json));
		//UnityWebRequestを送信しawaitで待機
		var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
		{
			uploadHandler = new UploadHandlerRaw(postData),
			downloadHandler = new DownloadHandlerBuffer()
		};
		request.SetRequestHeader("Content-Type", "application/json");
		return await request.SendWebRequest();
	}

	//JsonUtilityでjsonとしてつかう構造体を定義
	// prompt: "warehouse entrance",
	// model: "model",
	// length: 256,
	// temperature: 0.7,
	// top_k: 40,
	// top_p: 0.9,
	// repetition_penalty: 1.0,
	// repetition_penalty_range: 512,
	// repetition_penalty_slope: 3.33,
	[Serializable]
	public class JsonData
	{
		public string prompt;
		public string negative_prompt;
		public string model = "model";
		public bool restore_faces = true;
		public int width = 1024;

		public int length = 576;
		//public float temperature = 0.7f;
		//public int top_k = 40;
		//public float top_p = 0.9f;
		//public float repetition_penalty = 1.0f;
		//public int repetition_penalty_range = 512;
		//public float repetition_penalty_slope = 3.33f;
	}

	//PostJsonで返ってきたJsonの構造体
	[Serializable]
	public class ResponseJsonData
	{
		public string[] images;
	}
}