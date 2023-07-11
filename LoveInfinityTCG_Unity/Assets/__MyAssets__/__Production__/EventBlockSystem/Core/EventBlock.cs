using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventBlockの抽象クラス
/// この抽象クラス継承する形で任意のEventBlockオブジェクトを自作する
/// </summary>
public abstract class EventBlock : MonoBehaviour
{
	public UnityEvent _startEvent;
	EventDirector _eventDirector;
	bool _isPushedOnce;

	protected void OnNext(int onNextNum)
	{
		if (!_isPushedOnce)
		{
			_isPushedOnce = true;
			_eventDirector.OnNext(onNextNum);
		}
	}

	//各イベントブロックに切り替わった時にUnityEventを発動する
	public void StartEvent(EventDirector eventDirector)
	{
		_eventDirector = eventDirector;
		_isPushedOnce = false;
		_startEvent.Invoke();
	}

	//Eventを進行させるプレイヤーのアクション待機の前の動作を記述
	public abstract UniTask EventBeforeOnNext(CancellationToken token);

	//プレイヤーのアクションによってイベントが進行した直後の動作を記述
	public abstract UniTask EventAfterOnNext(CancellationToken token);
}