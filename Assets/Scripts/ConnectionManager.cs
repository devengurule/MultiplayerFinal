using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Tutorials.Editor;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance {  get; private set; }

    private const int MAX_PLAYERS = 20;
    public event Action OnSessionJoined;
    public event Action OnSessionLeft;

    // Tracks initalizing unity services task
    // We do not want to perform this task multiple times
    private Task initalizationTask;
    public ISession CurrentSession { get; private set; }
    private bool isBusy;

    private void Awake()
    {
        if (instance != null && instance == this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        // Beging initalizing unity services without waiting for completion
        _ = InitalizeAsync();
    }

    public async Task HostSession()
    {
        // Do not continue if the player is already atempting to host or if the network manager is running
        if (isBusy || CurrentSession != null) return;

        isBusy = true;

        try
        {
            // Ensure Unity Services are set up
            await InitalizeAsync();

            // Setup the session settings and specify to utilize unity relay
            var options = new SessionOptions
            {
                MaxPlayers = MAX_PLAYERS,
                IsPrivate = true
            }.WithRelayNetwork();

            // Request to create a relay session and store the session is successfull
            CurrentSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            // Subscribe to session deletion/removal events
            CurrentSession.Deleted += OnSessionDeleted;
            CurrentSession.RemovedFromSession += OnSessionDeleted;

            Debug.Log(CurrentSession.Code);

            // Notify when the session is joined
            OnSessionJoined?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.Log($"[NETWORK] Failed to create relay session {ex}");
        }
        finally
        {
            // Allow future session operations
            isBusy = false;
        }
    }

    public async Task JoinSession(string sessionCode)
    {
        if (sessionCode.IsNullOrWhiteSpace()) return;

        // Do not continue if the player is already atempting to host or if the network manager is running
        if (isBusy || CurrentSession != null) return;

        isBusy = true;

        try
        {
            // Ensure Unity Services are set up
            await InitalizeAsync();

            CurrentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);

            // Subscribe to session deletion/removal events
            CurrentSession.Deleted += OnSessionDeleted;
            CurrentSession.RemovedFromSession += OnSessionDeleted;
            
            // Notify when the session is joined
            OnSessionJoined?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.Log($"[NETWORK] Failed to join relay session {ex}");
        }
        finally
        {
            // Allow future session operations
            isBusy = false;
        }
    }

    public async Task LeaveSession()
    {
        // Do not continue if the player is already atempting a task or if the current session is null
        if (isBusy || CurrentSession == null) return;

        isBusy = true;

        try
        {
            // If player is host
            if (CurrentSession.IsHost)
            {
                // Delete the session as a host
                await CurrentSession.AsHost().DeleteAsync();
            }
            // If player is not host, probably client
            else
            {
                // Leave the session without deleting
                await CurrentSession.LeaveAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"[NETWORK] Failed to leave session {ex}");
        }
        finally
        {
            ClearSessionData();
            // Allow future session operations
            isBusy = false;
        }
    }

    private void OnSessionDeleted()
    {
        ClearSessionData();
    }

    private void ClearSessionData()
    {
        // Already cleaned up
        if (CurrentSession == null) return;

        CurrentSession.Deleted -= OnSessionDeleted;
        CurrentSession.RemovedFromSession -= OnSessionDeleted;

        CurrentSession = null;

        OnSessionLeft?.Invoke();
    }

    private Task InitalizeAsync()
    {
        // if task is null, run method to create a task and initalize unity services
        return initalizationTask ??= InitalizeServicesAsync();
    }

    private async Task InitalizeServicesAsync()
    {
        try
        {
            // Initalize Unity Services if they are NOT initalized
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"[NETWORK] Failed to initalize Unity Services {ex}");
            initalizationTask = null;
            throw;
        }

        try
        {
            // If we are not currently signed into Unity Services
            // sign in as anonymous user
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.Log($"[NETWORK] Failed Unity Services SignIn {e}");
            initalizationTask = null;
            throw;
        }
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}