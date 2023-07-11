using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// プレイヤーとヒロインの会話を指定回数ループする
/// </summary>
public class ConversationLoopEvent : EventBlock
{
    public int _loopCount = 10;
    //残り会話数
    int _remainingConversationCount;
    
    public override async UniTask EventBeforeOnNext(CancellationToken token)
    {
        _remainingConversationCount = _loopCount;
        await LoopConversation(token);
        OnNext(1);
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
    
    //loopCount回数分だけ会話を繰り返す
    async UniTask LoopConversation(CancellationToken token)
    {
        for (int i = 0; i < _loopCount; i++)
        {
            //残り会話数を更新
            CanvasManager.Instance.SetRemainingConversationCount(_remainingConversationCount, _loopCount);
            //「返答を入力」ボタンを表示しボタン入力かEnterキー入力を待ち受ける
            var inputButton = CanvasManager.Instance.AwaitPlayerResponse();
            var inputButtonClicked = inputButton.OnClickAsObservable().First().ToUniTask(cancellationToken: token);
            var getDownEnter = UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return), cancellationToken: token);
            await UniTask.WhenAny(inputButtonClicked, getDownEnter);
            SEPlayer.Instance.PlaySEOneShot(SEPlayer.SEType.startInput);
            //RoleをUserに変更
            ChatGPTRole.ChangeRole(ChatGPTRole.Role.User);
            //文字入力カーソルをPlayerのMessageInputBoxに設定
            CanvasManager.Instance.PlayerInputFieldSelect();
            //プレイヤーの返答を入力させ、ボタン入力かPキー入力を待ち受ける
            var sendPlayerResponseButton = CanvasManager.Instance.SendPlayerResponseButton();
            var sendPlayerResponseButtonClicked = sendPlayerResponseButton.OnClickAsObservable().First().ToUniTask(cancellationToken: token);
            var getDownCtrlEnter = UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.LeftControl), cancellationToken: token);
            await UniTask.WhenAny(sendPlayerResponseButtonClicked, getDownCtrlEnter);
            SEPlayer.Instance.PlaySEOneShot(SEPlayer.SEType.playerResponse);
            //Playerの入力フォーカスを外す
            EventSystem.current.SetSelectedGameObject(null);
            await SendMessageAndReceiveResponse();
            //残り会話数を減らす
            _remainingConversationCount--;
        }
    }
    
    async UniTask SendMessageAndReceiveResponse()
    {
        var inputText = CanvasManager.Instance.GetPlayerResponse();
        var response = await ChatGPTResponseTuner.Instance.GetResponseAsync(inputText);
        //responseが「」を含んでいたら除外する
        string replace1 = response.Replace("「", "");
        string replace2 = replace1.Replace("」", "");
        //RoleをAssistantに変更
        ChatGPTRole.ChangeRole(ChatGPTRole.Role.Assistant);
        //ヒロインの発言をMessageBoxに表示
        CanvasManager.Instance.SetHeroineMessage(replace2);
    }
}
