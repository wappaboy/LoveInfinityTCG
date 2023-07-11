using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// 初期化イベント
/// </summary>
public class InitializeEvent : EventBlock
{
    public override async UniTask EventBeforeOnNext(CancellationToken token)
    {
        FadePanelCanvas.Instance.SetAlpha(1);
        await UniTask.DelayFrame(1, cancellationToken:token);
        await FadePanelCanvas.Instance.FadeIn(2, token);
        OnNext(1);
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
}
