using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FadePanelCanvas : SingletonMonoBehaviour<FadePanelCanvas>
{
    CanvasGroup _canvasGroup;
    readonly bool _dontDestroyOnLoad = false;
    protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

    //フェードイン
    public UniTask FadeIn(float duration, CancellationToken token)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;
        return DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0, duration)
                      .SetEase(Ease.InQuad)
                      .ToUniTask(cancellationToken: token);
    }
    
    //フェードアウト
    public UniTask FadeOut(float duration, CancellationToken token)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        return DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1, duration)
                      .SetEase(Ease.InQuad)
                      .ToUniTask(cancellationToken: token);
    }
    
    public void SetAlpha(float alpha)
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_canvasGroup.alpha = alpha;
	}
}
