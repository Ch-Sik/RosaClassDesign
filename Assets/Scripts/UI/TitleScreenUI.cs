using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 화면 UI 담당
/// </summary>
public class TitleScreenUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnClickStartButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickOptionButton()
    {
        
    }
}
