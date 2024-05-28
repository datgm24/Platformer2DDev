using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public interface IFallable
    {
        /// <summary>
        /// 着地しているとき、trueを返す。
        /// </summary>
        bool IsGrounded { get; }

        /// <summary>
        /// Y速度を設定する。
        /// </summary>
        /// <param name="y">Y方向の速度</param>
        void SetVelocityY(float y);

        /// <summary>
        /// 経過秒数に従って、重力落下を処理する。
        /// </summary>
        /// <param name="delta">前回からの経過秒数</param>
        void Fall(float delta);
    }
}