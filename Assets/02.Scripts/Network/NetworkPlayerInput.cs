using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class NetworkPlayerInput : NetworkBehaviour
{
    private PlayerInput playerInput;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing from the player object.");
            return;
        }

        // 로컬 플레이어일 때 InputSystem 활성화
        playerInput.enabled = true;
        playerInput.ActivateInput();
    }

    private void Start()
    {
        // Start 메서드에서 로컬 플레이어만 InputSystem 활성화
        if (isLocalPlayer)
        {
            playerInput = GetComponent<PlayerInput>();

            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component is missing from the player object.");
                return;
            }

            playerInput.enabled = true;
            playerInput.ActivateInput();
        }
        else
        {
            // 로컬 플레이어가 아닌 경우 InputSystem 비활성화
            if (playerInput != null)
            {
                playerInput.enabled = false;
                playerInput.DeactivateInput();
            }
        }
    }
}
