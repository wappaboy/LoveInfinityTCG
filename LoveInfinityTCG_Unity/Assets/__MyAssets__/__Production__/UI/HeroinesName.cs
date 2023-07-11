using TMPro;
using UnityEngine;

public class HeroinesName : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = HeroineStatusManager.Instance.GetHeroinesName();
    }
}
