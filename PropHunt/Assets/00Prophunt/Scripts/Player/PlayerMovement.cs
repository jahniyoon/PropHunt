using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using static Models; // �÷��̾� �� ���� Ŭ����
using System.ComponentModel;


public class PlayerMovement : MonoBehaviourPun
{
    private CharacterController characterController;
    public Animator animator;
    private PlayerAction playerInput;                    // ��ǲ �ý��� Ȱ��� �̸� �����ؾ���


    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    public Transform cameraHolder;

    [Header("Player")]
    public Vector2 inputMoveVec;
    public Vector2 inputViewVec;

    private float jumpVec;      // ���� ���� ��

    private float playerSpeed;  // �÷��̾��� ���� �ӵ�
    public float walkSpeed;     // �÷��̾� �ȴ� �ӵ�
    public float dashSpeed;     // �÷��̾� ��� �ӵ�
    public float rotaionSpeed;  // ȸ�� �ӵ�
    public float jumpForce;     // ���� ��
    public float gravity;       // �߷°�

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70f;
    public float viewClampYMax = 80f;

    private float dash;         // ��ù�ư �Է� Ȯ��
    private bool isDash;        // ��� �����ΰ�
    public bool isDie;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        //newCameraRotation = cameraHolder.localRotation.eulerAngles;
        //newCharacterRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.
        Cursor.lockState = CursorLockMode.Locked;

        if (!isDie)
        {
            CalculateMovement();    // �÷��̾� �̵� ���
            CalculateView();        // �÷��̾� ���� ���
            ActiveAnimation();      // �ִϸ��̼� ����
        }
    }

    private void CalculateMovement()
    {        
        if(isDash && inputMoveVec.y == 1)   // ��� �����̰� ������ �� ����  ���
        {
            playerSpeed = dashSpeed;
        }
        else { playerSpeed = walkSpeed; }
        
         Vector3 move = Quaternion.Euler(0, cameraHolder.rotation.eulerAngles.y, 0) * new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);

        //Vector3 move = new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);
        characterController.Move(move * Time.deltaTime * playerSpeed);
        jumpVec += gravity * Time.deltaTime;                             // �߷� ����
    }
    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.viewYSensitivity * (playerSettings.viewXInverted ? -inputViewVec.x : inputViewVec.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.viewYSensitivity * (playerSettings.viewYInverted ? inputViewVec.y : -inputViewVec.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    // �̵� �Է�
    public void OnMove(InputValue value)
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        inputMoveVec = value.Get<Vector2>();
    }
    // �÷��̾� �� ���콺 �Է�
    public void OnView(InputValue value)
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.
        Cursor.lockState = CursorLockMode.Locked;
        inputViewVec = value.Get<Vector2>();
    }
    // ���� �Է�
    public void OnJump()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        if (characterController.isGrounded)
        {
            jumpVec = jumpForce;
        }
    }
    // ��� �Է�
    public void OnDash(InputValue value)
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        dash = value.Get<float>();
        if (dash != 0)
        {
            isDash = true;
        }
        else
            isDash = false;
    }

    // �ִϸ��̼�
    public void ActiveAnimation()
    {
        // �ȱ� �ִϸ��̼� ����
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
