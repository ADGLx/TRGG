using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingMap : MonoBehaviour {

    public Text Percentage;

    private void Start()
    {
        LoadTheLevel(1);
    }

    void LoadTheLevel(int index)
    {
        StartCoroutine(LoadAsy(index));
    }

    IEnumerator LoadAsy (int index)
    {
        AsyncOperation Op = SceneManager.LoadSceneAsync(index);

        while (!Op.isDone)
        {
            float Progress = Mathf.Clamp01(Op.progress / .9f);
            Percentage.text = (Progress * 100).ToString() + "%"; 
            yield return null;
        }
    }
}
