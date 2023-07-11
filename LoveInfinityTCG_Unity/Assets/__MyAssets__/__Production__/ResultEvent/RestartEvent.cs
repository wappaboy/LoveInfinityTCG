using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

//もう一度会話するボタンを押したときのイベント
public class RestartEvent : MonoBehaviour
{
	[SerializeField] bool _activeBgm = true;

	void Start()
	{
		//Restartボタンが押されるのを購読する
		CanvasManager.Instance.RestartButtonObservable().Subscribe(_ => Restart()).AddTo(gameObject);
	}

	async void Restart()
	{
		var token = this.GetCancellationTokenOnDestroy();
		//フェードアウト
		await FadePanelCanvas.Instance.FadeOut(2, token);
		CanvasManager.Instance.SetActiveRestartCanvas(false);
		CanvasManager.Instance.SetMessageBoxCanvasAlpha(1);
		CanvasManager.Instance.SetGeneratedImageCanvasAlpha(0);
		//フェードイン
		await FadePanelCanvas.Instance.FadeIn(3, token);
		if (_activeBgm)
		{
			BGMPlayer.Instance.PlayBGM();
		}
	}
}