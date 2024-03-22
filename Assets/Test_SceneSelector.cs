using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_SceneSelector : MonoBehaviour
{
    public void LoadTestScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
