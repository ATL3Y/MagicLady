using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darko
{
    public class PlayerTorso : MonoBehaviour
    {
        public Transform head;
        public float yDist;
        public float zDist;
        Renderer rend;

        private void Awake()
        {
            // turn off chest visibility in build
            rend = GetComponent<Renderer>();
            if (!Application.isEditor)
            {
                rend.enabled = false;
            }
        }

        private void Update()
        {
            Vector3 headForward = head.transform.forward;
            Vector3 headForwardFlat = Vector3.ProjectOnPlane(headForward, Vector3.up);
            Vector3 headForwardFlatNormalized = headForwardFlat.normalized;
            //Debug.DrawLine(Vector3.zero, headForwardFlatNormalized, Color.blue);
            Vector3 yOffset = new Vector3(0, -yDist, 0);
            float headInverted = Mathf.Sign(Vector3.Dot(head.transform.up, Vector3.up));
            Vector3 headForwardFlatNormalizedInverted = headInverted * headForwardFlatNormalized;
            Vector3 zOffset = headForwardFlatNormalizedInverted * -zDist;
            //Debug.DrawLine(Vector3.zero, zOffset, Color.red);
            Vector3 xyzOffset = yOffset + zOffset;
            transform.position = head.transform.position - xyzOffset;
            transform.forward = headForwardFlatNormalizedInverted;
        }
    }
}