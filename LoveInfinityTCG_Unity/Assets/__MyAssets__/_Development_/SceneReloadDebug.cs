using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloadDebug : MonoBehaviour
{
    void Update()
    {
        //Ctrl + Shift + Rでシーンをリロード
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            SceneReload();
        }
    }
    
    //シーンをリロード
    void SceneReload()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
