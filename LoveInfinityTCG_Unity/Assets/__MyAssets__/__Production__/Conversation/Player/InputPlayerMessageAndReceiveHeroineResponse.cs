using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

/// <summary>
/// プレイヤーの入力を送信し、ヒロインの返事を受け取る
/// </summary>
public class InputPlayerMessageAndReceiveHeroineResponse : EventBlock
{
    IDisposable _disposable;
    public override UniTask EventBeforeOnNext(CancellationToken token)
    {
        var button = CanvasManager.Instance.SendPlayerResponseButton();
        _disposable = button.OnClickAsObservable().Subscribe(_ =>
        {
            SendMessageAndReceiveResponse();
        }).AddTo(gameObject);
        return UniTask.CompletedTask;
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        _disposable?.Dispose();
        return UniTask.CompletedTask;
    }
    
    async void SendMessageAndReceiveResponse()
    {
        var inputText = CanvasManager.Instance.GetPlayerResponse();
        var response = await ChatGPTResponseTuner.Instance.GetResponseAsync(inputText);
        //RoleをAssistantに変更
        ChatGPTRole.ChangeRole(ChatGPTRole.Role.Assistant);
        //ヒロインの発言をMessageBoxに表示
        CanvasManager.Instance.SetHeroineMessage(response);
        OnNext(1);
    }
}
