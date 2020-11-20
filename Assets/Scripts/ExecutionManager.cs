using UnityEngine;

public class ExecutionManager : MonoBehaviour
{
    public static ExecutionManager Instance => _instance;
    private static ExecutionManager _instance = null;
    private ScenesManager _scenesManager = null;
    private ObjectSelect _objectSelect = null;

    private void Awake()
    {
        Singleton();
        GetReferences();
        Construct();
    }

    private void Singleton()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void GetReferences()
    {
        _scenesManager = GetReference(_scenesManager);
        _objectSelect = GetReference(_objectSelect);
    }

    private T GetReference<T>(T reference) where T : Object
    {
        if(reference != null)
        {
            return reference;
        }
        reference = GetComponent<T>();
        if(reference == null)
        {
            Debug.LogError($"{reference} is null. Couldnt find in ExecutionManager");
            return null;
        }
        return reference;
    }

    private void Construct()
    {
        _scenesManager.Construct();
        _objectSelect.Construct();
    }
}