using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static GameManager m_instance; // �̱����� �Ҵ�� static ����

    public GameObject playerPrefab; // ������ �÷��̾� ĳ���� ������
    public bool isGameover { get; private set; } // ���� ���� ����

    // �ֱ������� �ڵ� ����Ǵ�, ����ȭ �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      
    }

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // ������ ��ġ ����
        Vector3 SpawnPos = new Vector3 (1174, 106, 1003);

        // ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ���� ����
        // ��, �ش� ���� ������Ʈ�� �ֵ�����, ���� �޼��带 ���� ������ Ŭ���̾�Ʈ���� ����
        PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // ���� ������ �ڵ� ����Ǵ� �޼���
    public override void OnLeftRoom()
    {
        // ���� ������ �κ� ������ ���ư�
        SceneManager.LoadScene("LobbyScene");
    }
  
}
