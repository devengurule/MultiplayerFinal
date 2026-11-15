using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    private BasicPlayerControls controls;

    void Awake()
    {
        controls = new BasicPlayerControls();
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

    private void OnPlaceBombPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Bomb Placed!");
        //TODO: Bomb Placing Logic
    }

    private void OnThrowBombPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Bomb Thrown!");
        //TODO: Bomb Throwing Logic
    }
}
