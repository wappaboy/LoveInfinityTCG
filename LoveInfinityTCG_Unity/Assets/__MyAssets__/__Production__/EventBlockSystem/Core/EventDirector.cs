using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// EventBlockを順番に実行していく
/// </summary>
public class EventDirector : MonoBehaviour
{
	[SerializeField] EventBlock[] _eventBlockObjects;
	public bool _dontStartOnAwake = false;
	int _eventNum; //現在のイベント番号
	CancellationTokenSource _token;
	int _onNextNum;
	readonly List<bool> _onNextList = new List<bool>();
	int _currentEventNum;

	void Awake()
	{
		_token = new CancellationTokenSource();
		SetUpBlocks();

		if (_dontStartOnAwake) return;
		StartEvents(_token.Token).Forget();
	}

	void SetUpBlocks()
	{
		foreach (var obj in _eventBlockObjects)
		{
			obj.gameObject.SetActive(false);
			_onNextList.Add(false);
		}

		_eventNum = 0; //最初からスタート
	}

	//EventBlockを順番にアクティブにして実行する
	//イベントが終了したらオブジェクトを非アクティブにして次のイベントに進む
	public async UniTask StartEvents(CancellationToken token)
	{
		//イベントがすべて終了するまでイベント進行と退行を任意に繰り返す
		while (_eventNum < _eventBlockObjects.Length && !token.IsCancellationRequested)
		{
			_currentEventNum = _eventNum;
			var eventBlock = _eventBlockObjects[_currentEventNum];
			_eventBlockObjects[_currentEventNum].gameObject.SetActive(true);
			eventBlock.StartEvent(this);

			await eventBlock.EventBeforeOnNext(token);
			await UniTask.WaitUntil(() => _onNextList[_currentEventNum], cancellationToken: token);
			await eventBlock.EventAfterOnNext(token);

			_eventBlockObjects[_currentEventNum].gameObject.SetActive(false);

			if (_onNextList.All(value => value))
			{
				Debug.Log("<color=yellow>All event block has done!</color>");
			}
		}
	}

	public void OnNext(int onNextNum)
	{
		if (_eventNum >= _eventBlockObjects.Length) return;
		_onNextNum = onNextNum;
		_currentEventNum = _eventNum;

		if (_onNextNum == 1) //1 だったら次のイベントに進む
		{
			_eventNum++;

			//移動しようとしているブロックのonNextListがtrueになってしまっている場合、falseにする。それ以上ないときはデバッグログ。
			if (!(_eventNum > _onNextList.Count - 1) && _onNextList[_eventNum])
			{
				_onNextList[_eventNum] = false;
			}
		}
		else if (_onNextNum == 0) //0だったら繰り返し
		{
			_onNextList[_currentEventNum] = false;
		}
		else if (_onNextNum < 0) //負の数なら前のイベントに戻る
		{
			//現在のイベント番号からマイナスの _onNextNumを足す
			_eventNum = _currentEventNum + _onNextNum;

			if (0 > _eventNum) //_eventNumがリストの数より少ない場合
			{
				Debug.Log("<color=red>指定した onNextNum にイベントが存在しません。(_eventNumがリストの数より少ない)</color>");
			}
			else
			{
				_onNextList[_eventNum] = false;
			}
		}
		else //２以上のとき
		{
			//現在のイベント番号からプラスの_onNextNumを足す
			_eventNum = _currentEventNum + _onNextNum;
			if (_eventBlockObjects.Length < _eventNum)
			{
				//_eventNumがリストの数より多い場合
				Debug.Log("<color=red>指定した onNextNum にイベントが存在しません。(_eventNumがリストの数より多い)</color>");
			}
			else
			{
				_onNextList[_eventNum] = false;
			}
		}

		_onNextList[_currentEventNum] = true;
	}

	public void Restart()
	{
		_token?.Dispose();
		_token = new CancellationTokenSource();
		SetUpBlocks();
		StartEvents(_token.Token).Forget();
	}

	void OnDestroy()
	{
		_token.Cancel();
		_token.Dispose();
	}
}