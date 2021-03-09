using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Match _matchPrefab = null;

    private void Awake()
    {
        if(_matchPrefab == null)
        {
            Debug.LogError("MatchPrefab Reference");
            return;
        }
        PhotonNetwork.InstantiateRoomObject(_matchPrefab.gameObject.name, Vector3.zero, Quaternion.identity);
    }
}