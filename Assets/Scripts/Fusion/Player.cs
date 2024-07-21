using Fusion;
using System;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController characterController;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private PhysicBullet _physicBulletPrefab;
    private Vector3 _forward = Vector3.forward;
    public TextMeshProUGUI messageText;

    [Networked] TickTimer delay { get; set; }

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
    }
    private void Update()
    {
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hello TMT");
        }
    }
    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority,HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_SendMessage(string message,RpcInfo rpcInfo = default)
    {
        RPC_RelayMessage(message,rpcInfo.Source);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_RelayMessage(string message,PlayerRef messageSource)
    {
        if (messageText == null)
        {
            messageText = FindObjectOfType<TextMeshProUGUI>();
        }   
        if (messageSource == Runner.LocalPlayer)
        {
            message = "You said : " + message + "\n";
        }
        else
        {
            message = "other said : " + message + "\n";
        }
        messageText.text += message;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

            characterController.Move(10 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0f)
            {
                _forward = data.direction;
            }
            if (HasInputAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    Runner.Spawn(_bulletPrefab, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority,
                    (Runner, O) =>
                    {
                        O.GetComponent<Bullet>().Initial();

                    });
                }
                else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    Runner.Spawn(_physicBulletPrefab, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority,
                    (Runner, O) =>
                    {
                        O.GetComponent<PhysicBullet>().Initial(10 * _forward);

                    });
                }
            }
        }
    }
}
