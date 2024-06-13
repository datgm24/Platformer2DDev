using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public class FallBehaviour : MonoBehaviour, IFallable
    {
        public float VelocityY { get; private set; }

        public bool IsGrounded { get; private set; }
        /// <summary>
        /// 着地時の足元の法線方向
        /// </summary>
        public Vector2 FloorNormal { get; private set; }

        Rigidbody2D rb;
        BoxCollider2D boxCollider;
        int collideLayers;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            collideLayers = LayerMask.GetMask("Floor", "GimmickCollision");
        }

        public void Fall(float delta)
        {
            // 速度を更新
            VelocityY += Physics2D.gravity.y * delta;
            if (Mathf.Approximately(VelocityY, 0))
            {
                return;
            }

            // 座標の更新
            var move = delta * VelocityY * Vector2.up;
            int count = CharacterCaster.Cast(
                rb.position + boxCollider.offset,
                boxCollider,
                move, collideLayers);

            if (count == 0)
            {
                // 落下継続
                IsGrounded = false;
            }
            else
            {
                // 接触したので、着地
                Vector2 vertical = Mathf.Sign(VelocityY) * Vector2.up;
                move = (CharacterCaster.CanMoveDistance() - Physics2D.defaultContactOffset) * vertical;
                IsGrounded = VelocityY < 0f;
                VelocityY = 0;

                // 接触面が、垂直に近い場合は、床方向は更新しない
                var floorNormal = CharacterCaster.GetNearestRaycastHit2D().Value.normal;
                if (Mathf.Abs(Vector2.Dot(floorNormal, Vector2.right)) < 0.9f)
                {
                    FloorNormal = floorNormal;
                }
            }

            rb.position += move;
        }

        public void SetVelocityY(float y)
        {
            VelocityY = y;
        }
    }
}