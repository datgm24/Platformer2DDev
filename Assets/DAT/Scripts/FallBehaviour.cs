using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public class FallBehaviour : MonoBehaviour, IFallable
    {
        public bool IsGrounded => false;

        public void Fall(float delta)
        {
            Debug.Log($"落下 {delta}s");
        }

        public void SetVelocityY(float y)
        {
            Debug.Log($"Y速度 {y}");
        }
    }
}