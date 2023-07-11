using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

/// <summary>
/// 「返事を入力」ボタンを押したらプレイヤーの返答を入力させるテキストボックスを出す
/// </summary>
public class AwaitPlayerReactionEvent : EventBlock
{
    IDisposable _disposable;
    public override UniTask EventBeforeOnNext(CancellationToken token)
    {
        //「返答を入力」ボタンを表示し入力を待ち受ける
        var button = CanvasManager.Instance.AwaitPlayerResponse();
        _disposable = button.OnClickAsObservable().Subscribe(_ =>
        {
            //RoleをUserに変更
            ChatGPTRole.ChangeRole(ChatGPTRole.Role.User);
            OnNext(1);
        }).AddTo(gameObject);
        
        return UniTask.CompletedTask;
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        _disposable?.Dispose();
        return UniTask.CompletedTask;
    }
}
