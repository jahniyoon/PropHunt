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
    private PlayerAction playerInput;                    // 인풋 시스템 활용시 이름 동일해야함
    public CinemachineVirtualCamera playerCam;

    public GameObject prop;         // 클릭한 오브젝트
    public GameObject[] propPrefab;   // 클릭한 오브젝트의 프리팹
    public GameObject propObject;   // 변신한 오브젝트
    public GameObject playerBody;   // 플레이어 바디


    public float range = 100f;      // 사거리

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 플레이어 우 클릭 입력
    public void OnShoot()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        
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
                    Debug.Log(photonView.ViewID + "가 Master에게" + prop + "으로 변신 요청");
                    photonView.RPC("Shoot", RpcTarget.MasterClient, prop.name);  // 마스터에게 변신 요청
                }
            }
        }
    }

    // 마스터에게 변신을 요청한다.
    [PunRPC]
    public void Shoot(string prop)
    {
        photonView.RPC("SyncProp", RpcTarget.AllBufferedViaServer, prop);
    }

    // 마스터가 전체에게 요청
    [PunRPC]
    public void SyncProp(string prop)
    {
        Debug.Log(photonView.ViewID + "가 Master에게" + prop + "으로 변신 요청했으니 다 변신 시켜줘라.");

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

    // 취소 입력
    public void OnCancle()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

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
