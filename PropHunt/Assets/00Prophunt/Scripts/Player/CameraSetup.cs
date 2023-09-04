using Cinemachine; // 시네머신 관련 코드
using Photon.Pun; // PUN 관련 코드
using UnityEngine;

// 시네머신 카메라가 로컬 플레이어를 추적하도록 설정
public class CameraSetup : MonoBehaviourPun
{
    GameObject fpsCam;
    GameObject tpsCam;
    CinemachineVirtualCamera followCam;
    bool isFPS;
    GameObject playerBody;

    void Awake()
    {

        // 만약 자신이 로컬 플레이어라면
        if (photonView.IsMine)
        {
            // 씬에 있는 시네 머신 가상 카메라를 찾고
            //CinemachineVirtualCamera followCam =
            //    FindObjectOfType<CinemachineVirtualCamera>();


            // 플레이어 하위에 넣기
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            tpsCam.SetActive(false);
            fpsCam = GameObject.FindWithTag("FPS CAM");
            fpsCam.transform.parent = this.transform;

            PlayerShoot body = GetComponent<PlayerShoot>();
            playerBody = body.playerBody;


            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
            isFPS = true;

            ChangeCamera(followCam);
        }
    }
    public void OnChangeCamera()
    {
        if (photonView.IsMine)
        {
            if(isFPS) // 1인칭이면
            {
                fpsCam.SetActive(false);
                tpsCam.SetActive(true);

                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = false;

                ChangeCamera(followCam);
            }
            else if (!isFPS) // 3인칭이면
            {
                tpsCam.SetActive(false);
                fpsCam.SetActive(true);

                followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = true;

                ChangeCamera(followCam);
            }

        }
    }
    public void ChangeCamera(CinemachineVirtualCamera _followCam)
    {

            // 가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
            _followCam.Follow = transform;
            //followCam.LookAt = transform;
            _followCam.transform.parent = this.transform;

            // 플레이어에게 달아주기
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();   
            playerMovement.cameraHolder = _followCam.transform;

            PlayerShoot playerShoot = GetComponent<PlayerShoot>();
            playerShoot.playerCam = _followCam;
    }
}