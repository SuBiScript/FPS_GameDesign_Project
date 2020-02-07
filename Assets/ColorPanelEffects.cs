using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorPanels
{
    public static class ColorPanelEffects
    {
        public static void ThrowObject(GameObject caller, Collider collider, Vector3 direction)
        {
            var jumpForce = collider.CompareTag("Player")
                ? (GameController.Instance.m_PlayerController.IsGrounded() ? 10f : 12.5f)
                : 25f;
            collider.GetComponent<Rigidbody>()?.AddForce(caller != null ? caller.transform.up * jumpForce : direction * jumpForce,
                ForceMode.Impulse);
        }
        
        public static void ThrowObject(GameObject caller, Collision collision, Vector3 direction)
        {
            var isPlayer = collision.gameObject.CompareTag("Player");
            var jumpForce = isPlayer
                ? (GameController.Instance.m_PlayerController.IsGrounded() ? 10f : 12.5f)
                : 25f;
            if (isPlayer) GameController.Instance.m_PlayerController.m_PanelJump = true;
            collision.gameObject.GetComponent<Rigidbody>()?.AddForce(caller != null ? caller.transform.up * jumpForce : direction * jumpForce,
                ForceMode.Impulse);
        }

        public static void AttractObject(Rigidbody attracted, GameObject attractor, float force = 5f)
        {
            
            Vector3 temp = (attractor.transform.position - attracted.transform.position);
            if (temp.magnitude > 0.2f)
                attracted.AddForce(temp, ForceMode.Force);
        }
    }
}