using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RestartButtonEvent : EventBlock
{
    [SerializeField] GameObject _resultEventDirector;
    
    public override UniTask EventBeforeOnNext(CancellationToken token)
    {
        //RestartPivotCanvasを表示
        CanvasManager.Instance.SetActiveRestartCanvas(true);
        //ResultEventDirectorを表示
        _resultEventDirector.SetActive(true);
        OnNext(1);
        return UniTask.CompletedTask;
    }

    public override UniTask EventAfterOnNext(CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
}
