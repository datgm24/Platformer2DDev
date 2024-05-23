using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public class MoverWithCharacterController : MonoBehaviour, ICharacterMoveable
    {
        [SerializeField, Tooltip("歩く速度")]
        float walkSpeed = 4f;

        Rigidbody2D rb;

        float walkDirection = 0;
        bool isJumped = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Jump()
        {
            isJumped = true;
        }

        public void Walk(float moveX)
        {
            walkDirection = moveX;
        }

        void FixedUpdate()
        {
            // TODO ジャンプ開始

            // TODO 重力落下

            var v = rb.velocity;
            v.x = walkSpeed * walkDirection;
            rb.velocity = v;
        }
    }
}