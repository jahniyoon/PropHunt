using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviourPunCallbacks
{
    private PlayerAction playerInput;                    // ��ǲ �ý��� Ȱ��� �̸� �����ؾ���
    private CharacterController characterController;
    public Animator animator;

    public Slider playerHpSlider;
    public float maxHP;
    public float curHP;
    float originHP;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerHpSlider.maxValue = maxHP;
        playerHpSlider.value = curHP;
        originHP = curHP;
    }



    public void OnPunch()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.
        animator.SetTrigger("onPunch");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Damage"))
        {
            photonView.RPC("DamageSync", RpcTarget.MasterClient, curHP);  // �ָԿ� ������ �����Ϳ��� ������ ��� ��û
        }
    }

    // �����Ͱ� ��� ID���� ������ ��� ��û
    [PunRPC]
    public void DamageSync(float _curHP)
    {
        photonView.RPC("OnDamage", RpcTarget.AllBufferedViaServer, _curHP);
    }

    [PunRPC]
    public void OnDamage(float playerHP)
    {
        animator.SetTrigger("onDamage");
        playerHP -= 5f;
        curHP = playerHP;
        playerHpSlider.value = curHP;
        if (playerHP <= 0)
        {
            animator.SetBool("isDie", true);

            Invoke("Respawn", 2f);

        }

    }
    public void Respawn()
    {
        if (photonView.IsMine)
        {
            Vector3 SpawnPos = new Vector3(1174, 106, 1003);


            // ������ ���� ��ġ�� �̵�
            transform.position = SpawnPos;
        }

        // ������Ʈ���� �����ϱ� ���� ���� ������Ʈ�� ��� ���ٰ� �ٽ� �ѱ�
        // ������Ʈ���� OnDisable(), OnEnable() �޼��尡 �����
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        curHP = originHP;
        playerHpSlider.maxValue = maxHP;
        playerHpSlider.value = curHP;
    }

  
}
