using Cinemachine; // �ó׸ӽ� ���� �ڵ�
using Photon.Pun; // PUN ���� �ڵ�
using UnityEngine;

// �ó׸ӽ� ī�޶� ���� �÷��̾ �����ϵ��� ����
public class CameraSetup : MonoBehaviourPun
{
    GameObject fpsCam;
    GameObject tpsCam;
    CinemachineVirtualCamera followCam;
    bool isFPS;
    GameObject playerBody;

    void Awake()
    {

        // ���� �ڽ��� ���� �÷��̾���
        if (photonView.IsMine)
        {
            // ���� �ִ� �ó� �ӽ� ���� ī�޶� ã��
            //CinemachineVirtualCamera followCam =
            //    FindObjectOfType<CinemachineVirtualCamera>();


            // �÷��̾� ������ �ֱ�
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
            if(isFPS) // 1��Ī�̸�
            {
                fpsCam.SetActive(false);
                tpsCam.SetActive(true);

                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = false;

                ChangeCamera(followCam);
            }
            else if (!isFPS) // 3��Ī�̸�
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

            // ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
            _followCam.Follow = transform;
            //followCam.LookAt = transform;
            _followCam.transform.parent = this.transform;

            // �÷��̾�� �޾��ֱ�
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();   
            playerMovement.cameraHolder = _followCam.transform;

            PlayerShoot playerShoot = GetComponent<PlayerShoot>();
            playerShoot.playerCam = _followCam;
    }
}