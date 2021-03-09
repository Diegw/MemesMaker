using UnityEngine;

public class ExecutionManager : MonoBehaviour
{
    public static ExecutionManager Instance => _instance;

    private static ExecutionManager _instance = null;
    private ScenesManager _scenesManager = null;
    private ObjectSelect _objectSelect = null;
    private NetworkManager _networkManager = null;
    private GameManager _gameManager = null;

    private void Awake()
    {
        Singleton();
        GetReferences();
        if(CheckReferences() == false)
        {
            return;
        }
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
        _networkManager = GetReference(_networkManager);
        _gameManager = GetReference(_gameManager);
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
    
    private bool CheckReferences()
    {
        if(CheckReference(_scenesManager) == false) return false;
        if(CheckReference(_objectSelect) == false) return false;
        if(CheckReference(_networkManager) == false) return false;
        if(CheckReference(_gameManager) == false) return false;
        return true;
    }

    private bool CheckReference<T>(T reference) where T : Object
    {
        if(reference == null)
        {
            return false;
        }
        return true;
    }

    private void Construct()
    {
        _scenesManager.Construct();
        _objectSelect.Construct();
        _networkManager.Construct();
        _gameManager.Construct();
    }
}