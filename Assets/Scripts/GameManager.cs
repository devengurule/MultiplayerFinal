using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TMP_InputField roomCodeInput;

    private string roomCode => roomCodeInput.text;

    private ConnectionManager session;

    private GameObject player;

    private void Start()
    {
        session = ConnectionManager.instance;

        session.OnSessionJoined += EnterRoom;
        session.OnSessionLeft += SessionLeft;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public async void Host()
    {
        Debug.Log("Making Game...");
        try
        {
            await session.HostSession();
        }
        catch
        {
            Debug.Log("[GAMEMANAGER] Failed to host session");
        }
    }

    public async void Join()
    {
        Debug.Log("Joining Game...");
        try
        {
            await session.JoinSession(roomCode);
        }
        catch
        {
            Debug.Log("[GAMEMANAGER] Failed to join session");
        }
    }

    public async void Leave()
    {
        Debug.Log("Leaving Game...");
        try
        {
            await session.LeaveSession();
        }
        catch
        {
            Debug.Log("[GAMEMANAGER] Failed to leave session");
        }
    }

    private void EnterRoom()
    {
        Debug.Log("Entered Room");
    }
    private void SessionLeft()
    {
        Debug.Log("Leaving");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        SpawnPlayer(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        DespawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private void DespawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log($"Client {clientId} disconnected.");
    }
}
