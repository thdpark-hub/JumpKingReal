using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Vector2 move;
    [SerializeField]private float movespeed = 5f;
    [SerializeField]private float jumpSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
        rigid.linearVelocity = new Vector2(move.x * movespeed, rigid.linearVelocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
