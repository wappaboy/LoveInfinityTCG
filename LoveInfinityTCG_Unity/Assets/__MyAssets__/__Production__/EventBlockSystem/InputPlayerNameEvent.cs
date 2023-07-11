using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class InputPlayerNameEvent : EventBlock
{
    public override async UniTask EventBeforeOnNext(CancellationToken token)
    {
        //プレイヤーが名前を入力するまで待つ
        var playerName = await CanvasManager.Instance.AwaitPlayerNameInput(token);
        PlayerSettingsManager.Instance._playerName = playerName;
        //効果音
        SEPlayer.Instance.PlaySEOneShot(SEPlayer.SEType.playerResponse);
        await DOTween.To(CanvasManager.Instance.SetPlayerSettingsCanvasAlpha, 1, 0, 1).ToUniTask(cancellationToken: token);
        OnNext(1);
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
}
