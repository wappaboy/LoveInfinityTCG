using UnityEngine;

/// <summary>
/// 効果音の管理と再生
/// </summary>
public class SEPlayer : SingletonMonoBehaviour<SEPlayer>
{
    AudioSource _audio;
    [SerializeField] AudioClip _startInput;
    [SerializeField] AudioClip _playerResponse;
    [SerializeField] AudioClip _openCalendar;
    [SerializeField] AudioClip _stampImage;
    readonly bool _dontDestroyOnLoad = false;
    protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void PlaySEOneShot(SEType seType)
    {
        switch (seType)
        {
            case SEType.startInput:
                _audio.PlayOneShot(_startInput);
                break;
            case SEType.playerResponse:
                _audio.PlayOneShot(_playerResponse);
                break;
            case SEType.openCalendar:
                _audio.PlayOneShot(_openCalendar);
                break;
            case SEType.stampImage:
                _audio.PlayOneShot(_stampImage);
                break;
        }
    }

    public enum SEType
    {
        startInput,
        playerResponse,
        openCalendar,
        stampImage
    }
}
