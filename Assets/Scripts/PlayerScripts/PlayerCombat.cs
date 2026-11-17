using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : NetworkBehaviour
{
    private Controls controls;

    void Awake()
    {
        controls = new Controls();
    }

    void OnEnable()
    {
        controls.PlayerMap.Enable();

        controls.PlayerMap.PlaceBomb.performed += OnPlaceBombPerformed;
        controls.PlayerMap.ThrowBomb.performed += OnThrowBombPerformed;
    }

    private void OnDisable()
    {
        controls.PlayerMap.PlaceBomb.performed -= OnPlaceBombPerformed;
        controls.PlayerMap.ThrowBomb.performed -= OnThrowBombPerformed;

        controls.PlayerMap.Disable();
    }

    private (Vector2 velocity, float strength) GetStrengthAndVelocity()
    {
        // (If the camera or a mouse doesn't exist, return default values.
        if (Camera.main == null || Mouse.current == null) { return (Vector2.zero, 0f); }

        // Take the current mouse position, and convert the coordinates into tangible vector.
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition3D = Camera.main.ScreenToWorldPoint(new Vector3(
            mousePos.x,
            mousePos.y,
            Camera.main.nearClipPlane
        ));

        Vector2 mouseWorldPos = new Vector2(mouseWorldPosition3D.x, mouseWorldPosition3D.y);
        Vector2 offset = (mouseWorldPos - (Vector2)transform.position);

        // Calculate the direction through normalization, and calculate the strength by taking the magnitude.
        Vector2 direction = offset.normalized;
        float distance = offset.magnitude;

        // Define the limits of the vector and apply them.
        float minStrength = 2.0f;
        float maxStrength = 10.0f;

        float finalStrength = Mathf.Clamp(distance, minStrength, maxStrength);

        Vector2 finalVelocity = direction * finalStrength;

        // Return the final velocity (which has the direction), and the final strength.
        return (finalVelocity, finalStrength);
    }

    private void OnPlaceBombPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("Bomb Placed!");

        if (!IsOwner) return;

        PlaceBombServerRpc();
    }

    private void OnThrowBombPerformed(InputAction.CallbackContext context)
    {
        // Debug.Log("Bomb Thrown!");

        if (!IsOwner) return;

        ThrowBombServerRpc();
    }

    [ServerRpc]
    private void PlaceBombServerRpc()
    {
        // Locate and use the bomb prefab in the Resources folder.*
        //*(May need to be changed later.)
        GameObject bombPrefab = Resources.Load<GameObject>("Bomb");

        // If there's no existing bomb prefab, stop the script and return an error.
        if (bombPrefab == null)
        {
            Debug.LogError("There's no available bomb prefab.");
            return;
        }

        // Ask the server to spawn a bomb at the player's location after getting confirmation.
        Vector2 spawnPosition = transform.position;
        GameObject bombInstance = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        Bomb bombComponent = bombInstance.GetComponent<Bomb>();

        if (bombComponent != null)
        {
            bombComponent.InitializeAndThrow(Vector2.zero);
        }

        // Only spawn a bomb network object if the bomb has a network object component.
        NetworkObject bombNetworkObject = bombInstance.GetComponent<NetworkObject>();
        if (bombNetworkObject != null)
        {
            bombNetworkObject.Spawn(true);
        }
        else
        {
            Debug.LogError("The bomb is missing a network object component.");
            Destroy(bombInstance);
        }
    }

    [ServerRpc]
    private void ThrowBombServerRpc()
    {
        // Locate and use the bomb prefab in the Resources folder.*
        //*(May need to be changed later.)
        GameObject bombPrefab = Resources.Load<GameObject>("Bomb");

        // If there's no existing bomb prefab, stop the script and return an error.
        if (bombPrefab == null)
        {
            Debug.LogError("There's no available bomb prefab.");
            return;
        }

        // Track the player's coordinates, and obtain the strength/velocity based on cursor position.
        Vector2 spawnPosition = transform.position;
        var (throwVelocity, throwStrength) = GetStrengthAndVelocity();
        GameObject bombInstance = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

        Bomb bombComponent = bombInstance.GetComponent<Bomb>();
        if (bombComponent != null)
        {
            bombComponent.InitializeAndThrow(throwVelocity);
        }
        // Only spawn a bomb network object if the bomb has a network object component.
        NetworkObject bombNetworkObject = bombInstance.GetComponent<NetworkObject>();
        if (bombNetworkObject != null)
        {
            bombNetworkObject.Spawn(true);
        }
        else
        {
            Debug.LogError("The bomb is missing a network object component.");
            Destroy(bombInstance);
        }
    }
}
