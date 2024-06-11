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
        BoxCollider2D boxCollider;
        int collideLayers;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            fallable = GetComponent<IFallable>();
            boxCollider = GetComponent<BoxCollider2D>();
            collideLayers = LayerMask.GetMask("Floor", "GimmickCollision");
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
            Vector2 moveDir = Vector2.right;

            if (fallable.IsGrounded)
            {
                // 着地時は、床の方向に移動を試みる
                moveDir = Vector3.Cross(fallable.FloorNormal, Vector3.forward);
            }

            // 移動量を求める
            Vector2 tickMove = Time.deltaTime * walkSpeed * walkDirection * moveDir;

            // 水平方向の接触の判定
            tickMove = CheckTickMove(tickMove);

            // 確定した移動を反映
            rb.position += tickMove;
        }

        /// <summary>
        /// 予定の方向へ移動できるかを確認して、
        /// 移動できる移動量を返す。
        /// </summary>
        /// <param name="tickMove">移動したいベクトル</param>
        /// <returns>移動可能なベクトル</returns>
        Vector2 CheckTickMove(Vector2 tickMove)
        {
            if (Mathf.Approximately(tickMove.magnitude, 0))
            {
                return tickMove;
            }

            int count = CharacterCaster.Cast(
                rb.position + boxCollider.offset,
                boxCollider, tickMove,
                collideLayers);

            // 接触がなければ、下方向の段差チェック
            if (count == 0)
            {
                return CheckDownStep(tickMove);
            }
            // 接触があれば、上方向の段差チェック
            return CheckUpStep(tickMove);
        }

        /// <summary>
        /// 下りの段差チェック
        /// </summary>
        /// <param name="tickMove">移動したいベクトル</param>
        /// <returns>移動させるベクトル</returns>
        Vector2 CheckDownStep(Vector2 tickMove)
        {
            // 移動後の座標から、下方向の段差を確認
            Vector2 nextPos = rb.position + tickMove;

            // 下方向に段差チェック
            int count = CharacterCaster.Cast(
                rb.position+boxCollider.offset,
                boxCollider,
                step * Vector2.down,
                collideLayers);

            if (count == 0)
            {
                // 足が下せない段差なので、そのまま飛ぶ
                return tickMove;
            }

            // 足が届くので、移動量を調整する
            tickMove.y = -CharacterCaster.GetNearestRaycastHit2D().Value.distance;
            return tickMove;
        }

        /// <summary>
        /// 登りの段差チェック
        /// </summary>
        /// <param name="tickMove">移動したいベクトル</param>
        /// <returns>移動させるベクトル</returns>
        Vector2 CheckUpStep(Vector2 tickMove)
        {
            // 移動量を調整
            float nextStep = CharacterCaster.GetStepHeight();
            if (nextStep <= step)
            {
                // 乗り越えられる
                tickMove.y = nextStep;
                return tickMove;
            }

            // 乗り越えられない
            tickMove.x = Mathf.Sign(tickMove.x) * CharacterCaster.GetNearestRaycastHit2D().Value.distance;
            return tickMove;
        }
    }
}