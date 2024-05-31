using Codice.CM.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    /// <summary>
    /// キャラクターの当たり判定をキャストして、
    /// 接触状況を返す機能を提供するstaticクラス。
    /// </summary>
    public static class CharacterCaster
    {
        /// <summary>
        /// 接触する数を受け取る上限。足りなかったら増やす。
        /// </summary>
        public static int CollisionMax => 4;

        static RaycastHit2D[] castResults = new RaycastHit2D[CollisionMax];
        /// <summary>
        /// 前回のCastで取得した接触相手の配列。
        /// </summary>
        public static RaycastHit2D[] CastResults => castResults;

        /// <summary>
        /// 前回のCastで接触した数。
        /// </summary>
        public static int HitCount { get; private set; }

        /// <summary>
        /// Castで使った移動ベクトル
        /// </summary>
        static Vector2 moveVector;

        /// <summary>
        /// 一番近いオブジェクトのCastResultsのインデックス。-1のとき、未検出。
        /// </summary>
        static int nearestIndex = -1;

        /// <summary>
        /// チェックしたレイヤーマスク
        /// </summary>
        static int castedLayerMask;

        /// <summary>
        /// チェックした起点
        /// </summary>
        static Vector2 castedOrigin;

        /// <summary>
        /// チェックした当たり判定
        /// </summary>
        static BoxCollider2D castedBoxCollider;

        /// <summary>
        /// centerから、colliderで指定した当たり判定をキャストした結果を求める。
        /// </summary>
        /// <param name="center">確認を開始する中心座標</param>
        /// <param name="collider">当たり判定データ</param>
        /// <param name="move">移動ベクトル</param>
        /// <param name="layerMask">レイヤーマスク</param>
        /// <returns>接触数。HitCountと同じ</returns>
        public static int Cast(Vector2 center, BoxCollider2D collider, Vector2 move, int layerMask)
        {
            moveVector = move;
            nearestIndex = -1;
            if (Mathf.Approximately(move.magnitude, 0f))
            {
                HitCount = 0;
                return HitCount;
            }

            castedOrigin = center;
            castedBoxCollider = collider;
            castedLayerMask = layerMask;

            HitCount = Physics2D.BoxCastNonAlloc(
                center + collider.offset,
                collider.size - 2.0f * Physics2D.defaultContactOffset * Vector2.one,    // 接触の干渉距離を確保
                0f, move.normalized, castResults, 
                move.magnitude + Physics2D.defaultContactOffset,                        // 接触の干渉距離分、移動を伸ばす
                layerMask);

            return HitCount;
        }

        /// <summary>
        /// 前回Castした結果、moveに対して移動できる距離を返す。
        /// 斜面の場合、斜面に接触するまでの距離。
        /// </summary>
        /// <returns>moveVectorに対して、移動可能な距離を返す。</returns>
        public static float CanMoveDistance()
        {
            if (HitCount == 0)
            {
                return moveVector.magnitude;
            }

            var nearest = GetNearestRaycastHit2D();
            if (nearest == null)
            {
                return moveVector.magnitude;
            }
            return nearest.Value.fraction;
        }

        /// <summary>
        /// 前回Castした結果、一番近いオブジェクトを返す。
        /// </summary>
        /// <returns>一番近いオブジェクト。接触していなければnull</returns>
        public static GameObject GetNearestObject()
        {
            var nearest = GetNearestRaycastHit2D();
            if (nearest == null)
            {
                return null;
            }

            return nearest.Value.collider.gameObject;
        }

        /// <summary>
        /// 一番近いオブジェクトの当たり判定を返す。
        /// </summary>
        /// <returns>一番近いオブジェクトの当たり判定結果。なければnull</returns>
        public static RaycastHit2D? GetNearestRaycastHit2D()
        {
            if (HitCount == 0)
            {
                return null;
            }
            if (nearestIndex != -1)
            {
                return CastResults[nearestIndex];
            }

            float minDistance = CastResults[0].distance;
            nearestIndex = 0;
            for (int i = 1; i < HitCount; i++)
            {
                if (CastResults[i].distance < minDistance)
                {
                    nearestIndex = i;
                    minDistance = CastResults[i].distance;
                }
            }

            return CastResults[nearestIndex];
        }

        /// <summary>
        /// 接触面の法線ベクトルを返す。
        /// </summary>
        /// <returns>法線ベクトル。未接触のとき、上</returns>
        public static Vector2 GetNormal()
        {
            var nearest = GetNearestRaycastHit2D();

            // 接触していなければ上方向にしておく。
            if (nearest == null)
            {
                return Vector2.up;
            }

            return nearest.Value.normal;
        }

        /// <summary>
        /// 水平移動時の足元から、接触オブジェクトの上面への距離。
        /// </summary>
        /// <returns>Y方向の距離</returns>
        public static float GetStepHeight()
        {
            float l_or_r = Mathf.Sign(moveVector.x);

            var nearest = GetNearestRaycastHit2D();
            if ((nearest == null) || (Mathf.Abs(l_or_r) < 0.5f))
            {
                return 0;
            }

            // 頭の高さから、移動方向の起点を求める
            Vector2 from = castedOrigin + 0.5f * castedBoxCollider.size + castedBoxCollider.offset + Physics2D.defaultContactOffset * Vector2.down;
            from.x = nearest.Value.point.x - l_or_r * Physics2D.defaultContactOffset;
            var result = Physics2D.Raycast(from, l_or_r * Vector2.right, 2f * Physics2D.defaultContactOffset,
                castedLayerMask);
            if (result.collider != null)
            {
                // 頭をぶつけているなら、身長をそのまま返す
                return castedBoxCollider.size.y;
            }

            from.x += l_or_r * 2f * Physics2D.defaultContactOffset;
            result = Physics2D.Raycast(from, Vector2.down, castedBoxCollider.size.y, castedLayerMask);
            if (result.collider == null)
            {
                // 足元に何もないので、段差なし
                return 0;
            }

            float footY = castedOrigin.y - 0.5f * castedBoxCollider.size.y + castedBoxCollider.offset.y;
            float distanceY = result.point.y - footY;
            return distanceY;
        }
    }
}