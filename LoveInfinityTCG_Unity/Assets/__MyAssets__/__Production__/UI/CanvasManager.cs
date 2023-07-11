using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
{
	[Header("Player Settings")] [SerializeField]
	CanvasGroup _playerSettingsCanvas;

	[SerializeField] Button _playerSettingsEnterButton;
	[SerializeField] TMP_InputField _playerNameInputField;

	[Header("Conversation")] [SerializeField]
	CanvasGroup _messageBoxCanvas;

	[SerializeField] GameObject _playerBoxPivot;
	[SerializeField] GameObject _heroineBoxPivot;
	[SerializeField] TextMeshProUGUI _heroineText;
	[SerializeField] Button _startPlayerResponseButton;
	[SerializeField] Button _sendPlayerResponseButton;
	[SerializeField] TMP_InputField _playerResponseInputField;
	[SerializeField] TextMeshProUGUI _remainingConversationCountText;

	[Header("Generated Image")] [SerializeField]
	CanvasGroup _generatedImagePivot;


	[Header("Restart")] [SerializeField] CanvasGroup _restartPivot;
	[SerializeField] Button _mintNFTButton;
	[SerializeField] Button _restartButton;
	[SerializeField] Button _calendarButton;

	[Header("Mint NFT")] [SerializeField] CanvasGroup _mintNFTPivot;
	[SerializeField] TMP_InputField _walletAddressInputField;
	[SerializeField] Button _mintButton;
	[SerializeField] Button _backButton;

	[Header("Calendar")] [SerializeField] CanvasGroup _calendarPivot;
	[SerializeField] RawImage _generatedImageForCalendar;

	readonly bool _dontDestroyOnLoad = false;
	protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

	void Start()
	{
		//最初はすべてのBoxとButtonを非表示に
		_generatedImagePivot.alpha = 0;
		_playerBoxPivot.SetActive(false);
		_heroineBoxPivot.SetActive(false);
		_startPlayerResponseButton.gameObject.SetActive(false);
		_sendPlayerResponseButton.gameObject.SetActive(false);
		_generatedImageForCalendar.gameObject.SetActive(false);
		SetActiveRestartCanvas(false);
		SetActiveMintNFTCanvas(false);

		//ChatGptRoleを購読してRoleごとに表示するMessageBoxを変える
		ChatGPTRole.CurrentRole.SkipLatestValueOnSubscribe().Subscribe(role =>
		{
			if (role == ChatGPTRole.Role.Assistant)
			{
				//_heroineMessageBoxを表示
				_startPlayerResponseButton.gameObject.SetActive(false);
				_sendPlayerResponseButton.gameObject.SetActive(false);
				_playerBoxPivot.SetActive(false);
				_heroineBoxPivot.SetActive(true);
			}
			else if (role == ChatGPTRole.Role.User)
			{
				//_userMessageBoxを表示
				_startPlayerResponseButton.gameObject.SetActive(false);
				_sendPlayerResponseButton.gameObject.SetActive(true);
				_playerBoxPivot.SetActive(true);
				_heroineBoxPivot.SetActive(false);
			}
			else
			{
				Debug.LogError("Roleが不正です");
			}
		}).AddTo(gameObject);
	}

	//プレイヤーの名前入力のボタン入力を待ち受け、ボタン入力があるかCtrl + Enterキーが押されたら
	//InputFieldの入力を返すUniTask
	//ボタン入力があったらInputFieldとボタンを押せないようにする
	public async UniTask<string> AwaitPlayerNameInput(CancellationToken token)
	{
		var buttonClicked = _playerSettingsEnterButton.OnClickAsObservable().First().ToUniTask(cancellationToken: token);
		var getDOwnCtrlEnter = UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.LeftControl), cancellationToken: token);
		await UniTask.WhenAny(buttonClicked, getDOwnCtrlEnter);
		_playerSettingsEnterButton.interactable = false;
		_playerNameInputField.interactable = false;
		return _playerNameInputField.text;
	}

	public void SetPlayerSettingsCanvasAlpha(float alpha)
	{
		_playerSettingsCanvas.alpha = alpha;
	}

	public void SetGeneratedImageCanvasAlpha(float alpha)
	{
		_generatedImagePivot.alpha = alpha;
	}

	public void SetMessageBoxCanvasAlpha(float alpha)
	{
		_messageBoxCanvas.alpha = alpha;
	}

	public void SetCalendarCanvasAlpha(float alpha)
	{
		_calendarPivot.alpha = alpha;
	}

	public void SetHeroineMessage(string message)
	{
		//前回のプレイヤーの入力内容を削除
		_playerResponseInputField.text = "";
		_heroineText.text = message;
	}

	//「返答を入力」ボタンを表示
	public Button AwaitPlayerResponse()
	{
		_startPlayerResponseButton.gameObject.SetActive(true);
		return _startPlayerResponseButton;
	}

	//「送信」ボタン
	public Button SendPlayerResponseButton()
	{
		return _sendPlayerResponseButton;
	}

	//ボタンの状態を変更しプレイヤーの入力を返す
	public string GetPlayerResponse()
	{
		_sendPlayerResponseButton.gameObject.SetActive(false);
		return _playerResponseInputField.text;
	}

	public void SetRemainingConversationCount(int count, int maxCount)
	{
		_remainingConversationCountText.text = $"残り会話数 {count}/{maxCount}";
	}

	//文字入力カーソルをPlayerのMessageInputBoxに設定
	public void PlayerInputFieldSelect()
	{
		_playerResponseInputField.Select();
	}

	//RestartCanvasを表示
	public void SetActiveRestartCanvas(bool active)
	{
		if (active)
		{
			_restartPivot.alpha = 1;
			_restartPivot.blocksRaycasts = true;
		}
		else
		{
			_restartPivot.alpha = 0;
			_restartPivot.blocksRaycasts = false;
		}
	}

	//Restartボタンが押されるのを待ち受けるObservableを返す
	public IObservable<Unit> RestartButtonObservable()
	{
		return _restartButton.OnClickAsObservable()
		                     .First()
		                     .Do(_ => _restartButton.interactable = false);
	}

	//Calendarボタンが押されるのを待ち受けるObservableを返す
	public IObservable<Unit> CalendarButtonObservable()
	{
		return _calendarButton.OnClickAsObservable()
		                      .First()
		                      .Do(_ => _calendarButton.interactable = false);
	}

	//MintNFTCanvasを表示
	public void SetActiveMintNFTCanvas(bool active)
	{
		if (active)
		{
			_mintNFTPivot.alpha = 1;
			_mintNFTPivot.blocksRaycasts = true;
		}
		else
		{
			_mintNFTPivot.alpha = 0;
			_mintNFTPivot.blocksRaycasts = false;
		}
	}

	//MintNFTボタンが押されるのを待ち受けるObservableを返す
	public IObservable<Unit> MintNFTButtonObservable()
	{
		return _mintNFTButton.OnClickAsObservable()
		                     .First()
		                     .Do(_ => _mintNFTButton.interactable = false);
	}

	public IObservable<Unit> BackButtonObservable()
	{
		return _backButton.OnClickAsObservable()
		                  .First()
		                  .Do(_ => _backButton.interactable = false);
	}

	//MintNFTボタン入力があったらアドレスを返す
	public UniTask<string> AwaitMintNFTCanvas(CancellationToken token)
	{
		return _mintButton.OnClickAsObservable()
		                  .First()
		                  .Do(_ => _mintButton.interactable = false)
		                  .Select(_ => _walletAddressInputField.text)
		                  .ToUniTask(cancellationToken: token);
	}

	//カレンダー用の画像をセット
	public void SetCalendarImage(Texture texture, Transform pos)
	{
		_generatedImageForCalendar.texture = texture;
		_generatedImageForCalendar.transform.position = pos.position;
		_generatedImageForCalendar.gameObject.SetActive(true);
	}
}