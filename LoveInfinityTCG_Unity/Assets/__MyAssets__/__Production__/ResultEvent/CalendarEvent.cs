using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class CalendarEvent : MonoBehaviour
{
	[Header("Debug")]
	[SerializeField] bool _debugEnable;
	[SerializeField] Texture _debugTexture;
	[SerializeField] int _debugYear;
	[SerializeField] int _debugMonth;
	[SerializeField] int _debugDay;

	void Start()
	{
		//Calendarボタンが押されるのを購読する
		CanvasManager.Instance.CalendarButtonObservable().Subscribe(_ =>
			{
				var texture = CreateStableDiffusionImage.Instance._generatedImage;
				DisplayCalendar(texture);
			}
		).AddTo(gameObject);
	}

	async void DisplayCalendar(Texture texture)
	{
		var token = this.GetCancellationTokenOnDestroy();
		//カレンダーを徐々に表示する
		SEPlayer.Instance.PlaySEOneShot(SEPlayer.SEType.openCalendar);
		await DOTween.To(CanvasManager.Instance.SetCalendarCanvasAlpha, 0, 1, 1).WithCancellation(token);
		await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
		SEPlayer.Instance.PlaySEOneShot(SEPlayer.SEType.stampImage);
		var targetTransform = CalendarDatePivots.Instance.GetDatePivot(CalendarDatePivots.Instance._promiseDate);
		CanvasManager.Instance.SetCalendarImage(texture, targetTransform);
	}

	void Update()
	{
		//Cキーでデバッグ表示
		if (_debugEnable && Input.GetKeyDown(KeyCode.C))
		{
			CalendarDatePivots.Instance._promiseDate = new DateTime(_debugYear, _debugMonth, _debugDay);
			DisplayCalendar(_debugTexture);
		}
	}
}