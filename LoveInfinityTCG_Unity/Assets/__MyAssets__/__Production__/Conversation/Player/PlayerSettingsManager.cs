/// <summary>
/// プレイヤーの設定を管理する
/// </summary>
public class PlayerSettingsManager : SingletonMonoBehaviour<PlayerSettingsManager>
{
    public string _playerName { get; set; }
    
    readonly bool _dontDestroyOnLoad = false;
    protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;
}
