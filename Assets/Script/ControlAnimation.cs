using UnityEngine;
using UnityEngine.InputSystem;

public class ControlAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    private Jump jump;

    void Start()
    {
        anim = GetComponent<Animator>();
        jump = GetComponent<Jump>(); 
    }

    void OnMove(InputValue value)
    {
        // 🌟 기를 모으는 중(Read)이거나 점프 중일 때는 이동 애니메이션/방향전환 무시!
        if (jump.charging || jump.jumping) 
        {
            anim.SetBool("walking", false);
            return; 
        }

        Vector2 moveinput = value.Get<Vector2>();
        bool isMoving = (moveinput != Vector2.zero);
        anim.SetBool("walking", isMoving);

        // 방향 뒤집기
        if(moveinput.x < 0)
        {
            playerSprite.flipX = true;
        }
        else if(moveinput.x > 0)
        {
            playerSprite.flipX = false;
        }
    }

    // 🌟 OnJump 함수는 지워도 됩니다. 상태 체크는 Update에서 하는 게 훨씬 정확합니다.

    void Update()
    {
        // Jump 스크립트의 변수 상태를 매 프레임 애니메이터에 똑같이 복사(동기화)합니다.
        // 이렇게 하면 손을 뗄 때 jump.charging이 false가 되면서 알아서 Read도 false가 됩니다!
        
        anim.SetBool("Read", jump.charging);
        anim.SetBool("Jumping", jump.jumping);
    }
}