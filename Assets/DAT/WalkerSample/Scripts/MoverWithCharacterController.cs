//#define DEBUG_LOG

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
        float slope = 46f;

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
            JumpProcess();
            Log($"1: {rb.position.y} {fallable.VelocityY} {fallable.IsGrounded}");
            MoveHorizontal();
            Log($"2: {rb.position.y} {fallable.VelocityY} {fallable.IsGrounded}");
            OnGround();
            Log($"3: {rb.position.y} {fallable.VelocityY} {fallable.IsGrounded}");
            fallable?.Fall(Time.deltaTime);
            Log($"4: {rb.position.y} {fallable.VelocityY} {fallable.IsGrounded}");
        }

        /// <summary>
        /// 着地時、斜め床などで宙に浮かないように、段差分落下させてみて、
        /// 地面から距離があったら接地させる。
        /// </summary>
        void OnGround()
        {
            // 空中の時は処理不要
            if (!fallable.IsGrounded)
            {
                return;
            }

            int count = CharacterCaster.Cast(rb.position, boxCollider, step * Vector2.down, collideLayers);
            // 落差分、地面がなければ落下するので処理は不要
            if (count == 0)
            {
                return;
            }

            float distance = CharacterCaster.GetNearestRaycastHit2D().Value.distance;
            if (distance > Physics2D.defaultContactOffset)
            {
                // 移動距離が残っているので、移動させて接地させる
                rb.position += (distance - Physics2D.defaultContactOffset) * Vector2.down;
            }
        }

        /// <summary>
        /// ジャンプの処理
        /// </summary>
        void JumpProcess()
        {
            if (!isJumped || !fallable.IsGrounded)
            {
                isJumped = false;
                return;
            }

            isJumped = false;

            // ジャンプの初速を設定
            // 初速を求める:jumpH = t * t * Physics2D.gravity.y / 2
            // t^2 = jumpH / (Physics2D.gravity.y * 0.5f)
            // 時間に負の値は不要なので、プラスのみ。
            // t = sqrt(jumpH / (Phyics2D.gravity.y * 0.5f))
            // vf = t * Physics2D.gravity.y
            // テストプレイ時に、動的にジャンプの高さを変更できるように、毎回算出する
            fallable.SetVelocityY(-Physics2D.gravity.y * Mathf.Sqrt(jumpHeight / (-Physics2D.gravity.y * 0.5f)));
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
                moveDir = Vector3.Cross(fallable.FloorNormal, Vector3.forward).normalized;
            }

            // 移動量を求める
            Vector2 tickMove = Time.deltaTime * walkSpeed * walkDirection * moveDir;

            // 水平方向の接触の判定
            tickMove = CheckTickMove(tickMove);

            Log($"  Adjusted Tick Move ={tickMove}");

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

            Log($"Check Tick Move {count} tick={tickMove}");

            // 接触がなければ、下方向の段差チェック
            if ((count == 0) && (fallable.VelocityY <= 0))
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
                nextPos + boxCollider.offset,
                boxCollider,
                step * Vector2.down,
                collideLayers);

            if (count == 0)
            {
                // 足が下せない段差なので、そのまま飛ぶ
                return tickMove;
            }

            // 足が届くので、移動量を調整する
            tickMove.y = -CharacterCaster.CanMoveDistance();
            tickMove.y += Physics2D.defaultContactOffset;   // 接触オフセット分、上昇させる
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
            Vector2 floorNormal = CharacterCaster.GetNormal();
            floorNormal.x = -Mathf.Abs(floorNormal.x);
            float nextStep = CharacterCaster.GetStepHeight();
            if ((nextStep <= step) || (Vector2.Angle(Vector2.up, floorNormal) < slope))
            {
                // 乗り越えられる
                tickMove.y = nextStep + Physics2D.defaultContactOffset;
                return tickMove;
            }

            // 乗り越えられない
            tickMove.x = Mathf.Sign(tickMove.x) * CharacterCaster.CanMoveDistance();
            tickMove.x -= Mathf.Sign(tickMove.x) * Physics2D.defaultContactOffset;
            return tickMove;
        }

        [System.Diagnostics.Conditional("DEBUG_LOG")]
        void Log(object mes)
        {
            Debug.Log(mes);
        }
    }
}