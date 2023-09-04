using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviourPunCallbacks
{
    private PlayerAction playerInput;                    // 인풋 시스템 활용시 이름 동일해야함
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
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        animator.SetTrigger("onPunch");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Damage"))
        {
            photonView.RPC("DamageSync", RpcTarget.MasterClient, curHP);  // 주먹에 맞으면 마스터에게 데미지 계산 요청
        }
    }

    // 마스터가 모든 ID에게 데미지 계산 요청
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


            // 지정된 랜덤 위치로 이동
            transform.position = SpawnPos;
        }

        // 컴포넌트들을 리셋하기 위해 게임 오브젝트를 잠시 껐다가 다시 켜기
        // 컴포넌트들의 OnDisable(), OnEnable() 메서드가 실행됨
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        curHP = originHP;
        playerHpSlider.maxValue = maxHP;
        playerHpSlider.value = curHP;
    }

  
}
