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

        static RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[CollisionMax];
        /// <summary>
        /// 前回のCastで取得した接触相手の配列。
        /// </summary>
        public static RaycastHit2D[] RaycastHit2Ds => raycastHit2Ds;

        /// <summary>
        /// 前回のCastで接触した数。
        /// </summary>
        public static int HitCount { get; private set; }

        /// <summary>
        /// Castで使った移動ベクトル
        /// </summary>
        static Vector2 moveVector;

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
            HitCount = 0;
            moveVector = move;
            Debug.Log($"未実装");
            return HitCount;
        }

        /// <summary>
        /// 前回Castした結果、moveに対して移動できる距離を返す。
        /// </summary>
        /// <returns>moveVectorに対して、移動可能な距離を返す。</returns>
        public static float CanMoveDistance()
        {
            Debug.Log($"未実装");
            return moveVector.magnitude;
        }

        /// <summary>
        /// 前回Castした結果、一番近いオブジェクトを返す。
        /// </summary>
        /// <returns>一番近いオブジェクト。接触していなければnull</returns>
        public static MonoBehaviour GetNearestObject()
        {
            Debug.Log($"未実装");
            return null;
        }

        /// <summary>
        /// 接触面の法線ベクトルを返す。
        /// </summary>
        /// <returns>法線ベクトル。未接触のとき、上</returns>
        public static Vector2 GetNormal()
        {
            Debug.Log($"未実装");

            return Vector2.up;
        }

        /// <summary>
        /// 足元から、接触オブジェクトの上面への距離。
        /// </summary>
        /// <returns>Y方向の距離</returns>
        public static float GetStepHeight()
        {
            Debug.Log($"未実装");

            return 0;
        }
    }
}