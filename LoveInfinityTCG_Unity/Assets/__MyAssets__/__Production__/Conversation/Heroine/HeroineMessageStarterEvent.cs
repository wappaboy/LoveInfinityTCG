using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroineMessageStarterEvent : EventBlock
{
    [SerializeField] bool _activeBgm = true;
    
    public override async UniTask EventBeforeOnNext(CancellationToken token)
    {
        //MessageBoxを徐々に表示
        await DOTween.To(CanvasManager.Instance.SetMessageBoxCanvasAlpha, 0, 1, 1).ToUniTask(cancellationToken: token);
        if (_activeBgm)
        {
            BGMPlayer.Instance.PlayBGM();
        }
        //ヒロインのプロフィールプロンプトを取得
        var profile = HeroineStatusManager.Instance.GetInitializePrompt();
        var response = await ChatGPTResponseTuner.Instance.GetResponseAsync(profile);
        //responseに：が入っていたらもう一度生成する
        int count = 0;
        while (response.Contains("："))
        {
            //countが10を超えたら強制終了
            if (count > 10)
            {
                Debug.LogError("生成が10回を超えたので再生成を諦めました.シーンをもう一度読み込みます.");
                //今のシーンを再読み込みする
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            }
            count++;
            Debug.Log($"[Count{count}]: エラー文字を含んでいたため再生成\nresponse: {response}");
            ChatGPTSaveChatLog.Instance.ResetChatLog();
            response = await ChatGPTResponseTuner.Instance.GetResponseAsync(profile);
        }
        //responseが「」を含んでいたら除外する
        string replace1 = response.Replace("「", "");
        string replace2 = replace1.Replace("」", "");
        //RoleをAssistantに変更
        ChatGPTRole.ChangeRole(ChatGPTRole.Role.Assistant);
        //ヒロインの発言をMessageBoxに表示
        CanvasManager.Instance.SetHeroineMessage(replace2);
        OnNext(1);
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
}
