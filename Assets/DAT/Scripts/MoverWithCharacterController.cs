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

            MoveHorizontal();
            fallable?.Fall(Time.deltaTime);
        }

        /// <summary>
        /// 水平移動処理。
        /// </summary>
        void MoveHorizontal()
        {
            if (fallable == null)
            {
                Debug.LogError("コンポーネントに、IFallableのクラスをアタッチしてください。");
                return;
            }

            // 移動方向を求める
            Vector2 moveDir = walkDirection * Vector2.right;

            if (fallable.IsGrounded)
            {
                // 着地時は、床の方向に移動を試みる
                moveDir = Vector3.Cross(fallable.FloorNormal, Vector3.forward);
                Debug.Log($"{fallable.FloorNormal}={moveDir}");
            }


            var v = rb.velocity;
            v.x = walkSpeed * walkDirection;
            rb.velocity = v;

        }
    }
}