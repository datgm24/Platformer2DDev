using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    /// <summary>
    /// プレイヤーを操作するための入力を見て、移動処理に指示を出す。
    /// </summary>
    public class PlayerInputToAction : MonoBehaviour
    {
        ICharacterMoveable moveable;
        ICharacterMoveable Moveable
        {
            get
            {
                if (moveable == null)
                {
                    moveable = GetComponent<ICharacterMoveable>();
                }
                return moveable;
            }
        }

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                Moveable.Jump();
            }
            Moveable.Walk(Input.GetAxisRaw("Horizontal"));
        }
    }
}