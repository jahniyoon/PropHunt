using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WebSocketSharp;

public class PlayerShoot : MonoBehaviourPunCallbacks
{
    private PlayerAction playerInput;                    // ��ǲ �ý��� Ȱ��� �̸� �����ؾ���
    public CinemachineVirtualCamera playerCam;

    public GameObject prop;         // Ŭ���� ������Ʈ
    public GameObject[] propPrefab;   // Ŭ���� ������Ʈ�� ������
    public GameObject propObject;   // ������ ������Ʈ
    public GameObject playerBody;   // �÷��̾� �ٵ�


    public float range = 100f;      // ��Ÿ�

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // �÷��̾� �� Ŭ�� �Է�
    public void OnShoot()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.
        
        if (playerCam != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);
                Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * range, Color.red);

                if (hit.collider.gameObject != null)
                {
                    prop = hit.collider.gameObject;
                    Debug.Log(photonView.ViewID + "�� Master����" + prop + "���� ���� ��û");
                    photonView.RPC("Shoot", RpcTarget.MasterClient, prop.name);  // �����Ϳ��� ���� ��û
                }
            }
        }
    }

    // �����Ϳ��� ������ ��û�Ѵ�.
    [PunRPC]
    public void Shoot(string prop)
    {
        photonView.RPC("SyncProp", RpcTarget.AllBufferedViaServer, prop);
    }

    // �����Ͱ� ��ü���� ��û
    [PunRPC]
    public void SyncProp(string prop)
    {
        Debug.Log(photonView.ViewID + "�� Master����" + prop + "���� ���� ��û������ �� ���� �������.");

        Object resource = Resources.Load(prop);

        if (resource != null)
        {
            playerBody.SetActive(false);
            Destroy(propObject);

            PlayerMovement playerPos = GetComponent<PlayerMovement>();
            //GameObject newProp = Instantiate(prop, playerPos.transform.position, Quaternion.identity);
            GameObject newProp = PhotonNetwork.Instantiate(prop, playerPos.transform.position, Quaternion.identity);
            newProp.transform.parent = playerPos.transform;
            newProp.layer = LayerMask.NameToLayer("Ignore Raycast");
            propObject = newProp;
        }
        
    }

    // ��� �Է�
    public void OnCancle()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        if (prop != null)
        {
            photonView.RPC("Cancle", RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    public void Cancle()
    {
        photonView.RPC("SyncCancle", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void SyncCancle()
    {
        prop = null;
        playerBody.SetActive(true);
        propObject.SetActive(false);
    }
}
