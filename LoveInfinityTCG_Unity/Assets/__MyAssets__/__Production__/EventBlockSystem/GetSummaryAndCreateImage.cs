using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 会話ログから要約を作成し、その要約をStableDiffusion用のプロンプトに整形する。
/// 整形したプロンプトを用いて画像を生成する。
/// </summary>
public class GetSummaryAndCreateImage : EventBlock
{
	[SerializeField, TextArea(5, 10)] string _promptSample;

	[Header("Show image")] [SerializeField]
	CanvasGroup _showingImageText; //そして約束の日...

	public override async UniTask EventBeforeOnNext(CancellationToken token)
	{
		var prepareAsync = PrepareFadePanel(token);
		var chatLog = ChatGPTSaveChatLog.Instance.GetChatLogText();
		var summary = await ChatGPTResponseTuner.Instance.GetSummaryAsync(chatLog);
		var date = GetDateFromSummary(summary);
		Debug.Log($"約束の日：{date}");
		//CalendarDatePivotに約束の日をセット
		CalendarDatePivots.Instance._promiseDate = date;
		var prompt = await ChatGPTResponseTuner.Instance.GetPromptForStableDiffusionAsync(summary, _promptSample);
		var createImageAsync = CreateStableDiffusionImage.Instance.CreateImageFromSummary(prompt);
		//GeneratedImageCanvasを表示
		CanvasManager.Instance.SetGeneratedImageCanvasAlpha(1);
		//MessageBoxを非表示に
		CanvasManager.Instance.SetMessageBoxCanvasAlpha(0);
		//prepareAsyncとcreateImageAsyncがどちらも完了したら次へ
		await UniTask.WhenAll(prepareAsync, createImageAsync);
		//フェードイン
		await FadePanelCanvas.Instance.FadeIn(2f, token);
		OnNext(1);
		_showingImageText.alpha = 0f;
	}

	public override UniTask EventAfterOnNext(CancellationToken token)
	{
		return UniTask.CompletedTask;
	}

	//画面をフェードアウトし「そして約束の日へ...」を表示する
	//裏でプロンプト生成と画像生成を進める
	async UniTask PrepareFadePanel(CancellationToken token)
	{
		//最後の返答を読むために4秒待つ
		await UniTask.Delay(TimeSpan.FromSeconds(4), cancellationToken: token);
		_showingImageText.alpha = 0f;
		var bgmFade = BGMPlayer.Instance.FadeOut(token);
		var fadeIn = FadePanelCanvas.Instance.FadeOut(2f, token); //画面を暗転
		await UniTask.WhenAll(bgmFade, fadeIn);
		//3秒待つ
		await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
		//「そして約束の日...」を2秒でフェードイン表示
		await DOTween.To(() => _showingImageText.alpha, x => _showingImageText.alpha = x, 1f, 2f)
		             .SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
	}

	//summary文中のTIME:直後のyyyy-mm-dd形式を抜き出してTime.Date形式に変換する
	DateTime GetDateFromSummary(string summary)
	{
		//summary文中にTIME:を含んでいなかったら「未定」を返す
		if (!summary.Contains("TIME:", StringComparison.Ordinal))
		{
			throw new Exception("TIME: 形式が見つかりませんでした");
		}

		//summary文中の空白を削除する
		summary = summary.Replace(" ", "");
		//summary文中のTIME:直後のyyyy-mm-dd形式を抜き出す
		var startIndex = summary.IndexOf("TIME:", StringComparison.Ordinal) + 5;
		var date = summary.Substring(startIndex, 10);
		//yyyy-mm-dd形式の文字列をTime.Date形式に変換する
		var year = int.Parse(date.Substring(0, 4));
		var month = int.Parse(date.Substring(5, 2));
		var day = int.Parse(date.Substring(8, 2));
		//Debug.Log($"{year}, {month}, {day}");
		var time = new DateTime(year, month, day);
		return time;
	}

	// void Update()
	// {
	// 	if (Input.GetKeyDown(KeyCode.L))
	// 	{
	// 		Debug.Log($"{GetDateFromSummary("AssistantはUserの幼馴染で、16歳の女性。Userとの会話の中で、Assistantの好きなものや性格が明らかになった。UserとAssistantは来週金曜日に六本木のバーで会う予定があり、Assistantは赤いワンピースとメガネを着ることに決めた。また、夏が好きなAssistantは、しょうごと一緒に海に行ったり花火を見たりすることが楽しいと話した。次に会う日程は2023年7月7日（金曜日）である。TIME: 2023-07-07。")}");
	// 	}
	// }
}