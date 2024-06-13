using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAT
{
    public class PushedBlock : MonoBehaviour, IPushable
    {
        public Vector3 Push(Vector3 push)
        {
            Debug.Log($"押す {push}");
            return Vector3.zero;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}