using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance => _instance;
    public static Action OnSceneChanged;

    private static ScenesManager _instance = null;
    private string _currentSceneName = string.Empty;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);

        SetCurrentScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SetCurrentScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetCurrentScene;
    }

    private void SetCurrentScene(Scene scene, LoadSceneMode sceneMode)
    {
        _currentSceneName = scene.name;
        OnSceneChanged?.Invoke();
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    private void OnDestroy()
    {
        OnDisable();
    }
}