using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public interface IPushable
    {
        /// <summary>
        /// 押したい移動ベクトルを受け取って、動けるか試す。
        /// 動けた距離を返す。
        /// </summary>
        /// <param name="push">押したい移動ベクトル</param>
        /// <returns>実際に動けた移動ベクトル</returns>
        Vector3 Push(Vector3 push);
    }
}