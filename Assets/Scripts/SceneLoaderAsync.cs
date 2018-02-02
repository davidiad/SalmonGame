using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderAsync : MonoBehaviour {

    private float _loadingProgress;
    public float LoadingProgress { get { return _loadingProgress; } }

	public void LoadScenes () {
        StartCoroutine(LoadScenesInOrder());
	}

    private IEnumerator LoadScenesInOrder()
    {
        // should have a check whether scene is already loaded
       // yield return SceneManager.LoadSceneAsync(0);
        yield return StartCoroutine(LoadScene(1));
        yield return StartCoroutine(LoadScene(2));
        yield return StartCoroutine(LoadScene(3));
    }

    private IEnumerator LoadScene (int index)
    {
        var asyncScene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        asyncScene.allowSceneActivation = false;

        while (!asyncScene.isDone)
        {
            _loadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;

            // At 0.9, the scene has loaded as much as possible, as the last 10% can't be multi-threaded
            if (asyncScene.progress >= 0.9f)
            {
                asyncScene.allowSceneActivation = true;
            }
            yield return null;
        }
    }
	
}
