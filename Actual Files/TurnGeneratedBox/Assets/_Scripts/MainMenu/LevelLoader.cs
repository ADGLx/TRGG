using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public void LoadLoadScreenNewMap(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);

        StaticMapConf.NewMap = true;
    }

    public void LoadLoadScreenOldMap(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);

        StaticMapConf.NewMap = false;
    }
}
