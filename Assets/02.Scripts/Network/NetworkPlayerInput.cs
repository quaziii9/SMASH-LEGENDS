using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class NetworkPlayerInput : NetworkBehaviour
{
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (isLocalPlayer)
        {
            // 로컬 플레이어의 경우 InputSystem을 활성화
            playerInput.enabled = true;
            playerInput.ActivateInput();
        }
        else
        {
            // 로컬 플레이어가 아닌 경우 InputSystem을 비활성화
            playerInput.enabled = false;
            playerInput.DeactivateInput();
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // 로컬 플레이어일 때 추가 설정
        playerInput.enabled = true;
    }
}
