using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public class MoverWithCharacterController : MonoBehaviour, ICharacterMoveable
    {
        [SerializeField, Tooltip("歩く速度")]
        float walkSpeed = 4f;

        [SerializeField, Tooltip("ジャンプの高さ")]
        float jumpHeight = 2.5f;

        [SerializeField, Tooltip("登れる斜面の角度")]
        float slope = 50f;

        [SerializeField, Tooltip("登れる段差の高さ")]
        float step = 0.2f;

        Rigidbody2D rb;
        IFallable fallable;

        float walkDirection = 0;
        bool isJumped = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            fallable = GetComponent<IFallable>();
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

            // TODO 水平移動
            var v = rb.velocity;
            v.x = walkSpeed * walkDirection;
            rb.velocity = v;

            // 重力落下
            fallable?.Fall(Time.deltaTime);

        }
    }
}