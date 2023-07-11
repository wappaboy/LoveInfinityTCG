using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BGMPlayer : SingletonMonoBehaviour<BGMPlayer>
{
    AudioSource _audio;
    float _startVolume;
    readonly bool _dontDestroyOnLoad = false;
    protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
        _startVolume = _audio.volume;
    }

    public void PlayBGM()
    {
        _audio.volume = _startVolume;
        _audio.Play();
    }
    
    public void PlayBGMWithNewClip(AudioClip clip)
    {
        _audio.clip = clip;
        _audio.Play();
    }

    public async UniTask FadeOut(CancellationToken token)
    {
        await _audio.DOFade(0, 2f).WithCancellation(token);
        _audio.Stop();
    }
}
