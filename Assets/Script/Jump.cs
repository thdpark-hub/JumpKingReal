using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    private Rigidbody2D rigid;
    
    // [요청하신 대로 걷기 관련 설정과 Update 내의 이동 로직은 비웠습니다]
    [Header("Jump Settings")]
    [SerializeField] private float jumpForwardForceMin = 2f;  // 살짝 눌렀을 때 최소 앞으로 뛰는 힘
    [SerializeField] private float jumpForwardForceMax = 5f;  // 끝까지 모았을 때 최대 앞으로 뛰는 힘
    
    [SerializeField] private float jumpUpwardForceMin = 5f;   // 살짝 눌렀을 때 최소 위로 뛰는 힘
    [SerializeField] private float jumpUpwardForceMax = 12f;  // 끝까지 모았을 때 최대 위로 뛰는 힘
    
    [SerializeField]private float movespeed =3f;

    [SerializeField] private float maxChargeTime = 1.5f;      // 풀차지까지 걸리는 시간 (초 단위) - 조절하기 훨씬 편함!
    
    // 상태 확인용 변수들
    public bool charging = false;
    public bool jumping;
    private float currentChargeTime = 0f;   // 누르고 있는 시간 타이머
    private float currentUpwardForce = 0f;  
    private float currentForwardForce = 0f; 

    [Header("Ground Check")]
    public bool isGrounded = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField]private LayerMask sliderLayer;
    [Header("Input Actions")]
    public InputActionReference moveAction; 
    public InputActionReference jumpAction; 

    private Vector2 moveInput;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = 2;
    }

    void OnEnable()
    {
        jumpAction.action.started += OnJumpStarted;
        jumpAction.action.canceled += OnJumpReleased;
    }

    void OnDisable()
    {
        jumpAction.action.started -= OnJumpStarted;
        jumpAction.action.canceled -= OnJumpReleased;
    }

    void Update()
    {
        bool isNowGrounded = false;
        bool isSlider = false;
        // 1. 바닥 체크
        if (groundCheck != null)
        {
            // 🌟 수정됨: 검사 결과를 임시 변수인 'isNowGrounded'에 먼저 저장합니다.
            isNowGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }
        if(groundCheck != null)
        {
            isSlider = Physics2D.OverlapCircle(groundCheck.position, checkRadius, sliderLayer);
        }


        // 🌟 2. 착지 판정 (점프 중이고, 바닥에 닿았으며, 아래로 떨어지거나 멈췄을 때)
        if (jumping && isNowGrounded && rigid.linearVelocity.y <= 0.1f)
        {
            jumping = false; // 착지 완료!
        }

        // 🌟 3. 판정이 다 끝난 후, 최종 결과를 진짜 변수에 넘겨줍니다.
        isGrounded = isNowGrounded;

        // ----------------------------------------------------

        // 4. 이동 입력값 매 프레임 읽기 (방향 파악용)
        moveInput = moveAction.action.ReadValue<Vector2>();

        // 5. 땅에 있을 때 좌우 이동
        if (isGrounded && !charging)
        {
            rigid.linearVelocity = new Vector2(moveInput.x * movespeed, rigid.linearVelocity.y);
        }
        
        // 6. 스페이스바를 누르고 있는 동안 점프력 충전
        if (charging && isGrounded)
        {
            currentChargeTime += Time.deltaTime;
            float chargeRatio = Mathf.Clamp01(currentChargeTime / maxChargeTime);

            currentUpwardForce = Mathf.Lerp(jumpUpwardForceMin, jumpUpwardForceMax, chargeRatio);
            currentForwardForce = Mathf.Lerp(jumpForwardForceMin, jumpForwardForceMax, chargeRatio);
        }
    }

    // 스페이스바를 '누르기 시작'할 때
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            currentChargeTime = 0f; // 누른 시간 0으로 초기화
            currentUpwardForce = jumpUpwardForceMin;   // 시작하자마자 떼도 뛸 수 있게 기본(최소) 점프력 장전
            currentForwardForce = jumpForwardForceMin; // 기본 앞쪽 점프력 장전
            charging = true;       
        }
    }

    // 스페이스바를 '뗐을 때'
    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        if (isGrounded && charging)
        {
            jumping = true;
            charging = false;

            float dirX = moveInput.x;

            Vector2 jumpForce = new Vector2(dirX * currentForwardForce, currentUpwardForce);
            
            rigid.linearVelocity = Vector2.zero; 
            rigid.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}