using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    /// <summary>
    /// キャラクターの移動行動の指示を受けるメソッド。
    /// </summary>
    public interface ICharacterMoveable
    {
        /// <summary>
        /// 歩く方向を引数で指示するメソッド。
        /// </summary>
        /// <param name="moveX">-1で左、0で停止、1で右</param>
        void Walk(float moveX);

        /// <summary>
        /// ジャンプの開始を指示するメソッド。
        /// </summary>
        void Jump();
    }
}