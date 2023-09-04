using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using static Models; // 플레이어 모델 셋팅 클래스
using System.ComponentModel;


public class PlayerMovement : MonoBehaviourPun
{
    private CharacterController characterController;
    public Animator animator;
    private PlayerAction playerInput;                    // 인풋 시스템 활용시 이름 동일해야함


    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    public Transform cameraHolder;

    [Header("Player")]
    public Vector2 inputMoveVec;
    public Vector2 inputViewVec;

    private float jumpVec;      // 현재 점프 힘

    private float playerSpeed;  // 플레이어의 현재 속도
    public float walkSpeed;     // 플레이어 걷는 속도
    public float dashSpeed;     // 플레이어 대시 속도
    public float rotaionSpeed;  // 회전 속도
    public float jumpForce;     // 점프 힘
    public float gravity;       // 중력값

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70f;
    public float viewClampYMax = 80f;

    private float dash;         // 대시버튼 입력 확인
    private bool isDash;        // 대시 상태인가
    public bool isDie;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        //newCameraRotation = cameraHolder.localRotation.eulerAngles;
        //newCharacterRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        Cursor.lockState = CursorLockMode.Locked;

        if (!isDie)
        {
            CalculateMovement();    // 플레이어 이동 계산
            CalculateView();        // 플레이어 시점 계산
            ActiveAnimation();      // 애니메이션 적용
        }
    }

    private void CalculateMovement()
    {        
        if(isDash && inputMoveVec.y == 1)   // 대시 상태이고 앞으로 갈 때만  대시
        {
            playerSpeed = dashSpeed;
        }
        else { playerSpeed = walkSpeed; }
        
         Vector3 move = Quaternion.Euler(0, cameraHolder.rotation.eulerAngles.y, 0) * new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);

        //Vector3 move = new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);
        characterController.Move(move * Time.deltaTime * playerSpeed);
        jumpVec += gravity * Time.deltaTime;                             // 중력 구현
    }
    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.viewYSensitivity * (playerSettings.viewXInverted ? -inputViewVec.x : inputViewVec.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.viewYSensitivity * (playerSettings.viewYInverted ? inputViewVec.y : -inputViewVec.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    // 이동 입력
    public void OnMove(InputValue value)
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        inputMoveVec = value.Get<Vector2>();
    }
    // 플레이어 뷰 마우스 입력
    public void OnView(InputValue value)
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        Cursor.lockState = CursorLockMode.Locked;
        inputViewVec = value.Get<Vector2>();
    }
    // 점프 입력
    public void OnJump()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        if (characterController.isGrounded)
        {
            jumpVec = jumpForce;
        }
    }
    // 대시 입력
    public void OnDash(InputValue value)
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        dash = value.Get<float>();
        if (dash != 0)
        {
            isDash = true;
        }
        else
            isDash = false;
    }

    // 애니메이션
    public void ActiveAnimation()
    {
        // 걷기 애니메이션 셋팅
        if (inputMoveVec.x != 0 || inputMoveVec.y != 0)
        {
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        animator.SetBool("isRun", isDash);
        animator.SetBool("isJump", !characterController.isGrounded);
        animator.SetFloat("xDir", inputMoveVec.x);
        animator.SetFloat("yDir", inputMoveVec.y);
    }
}
