using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ColorPanels
{
    public static class ColorPanelEffects
    {
        public static void ThrowObject(GameObject caller, Collider collider, Vector3 direction,
            ColorPanelProperties properties)
        {
            var isPlayer = collider.CompareTag("Player");
            float jumpForce = ComputeJumpForce(isPlayer, properties);
            if (isPlayer && properties.setPanelJump)
                GameController.Instance.m_PlayerController.PlatformJump();

            try
            {
                var colliderRigidbody = collider.GetComponent<Rigidbody>();
                colliderRigidbody.velocity = Vector3.zero;
                colliderRigidbody.AddForce(caller != null ? caller.transform.up * jumpForce : direction * jumpForce,
                    ForceMode.Impulse);
            }
            catch (NullReferenceException)
            {
            }
        }

        public static void ThrowObject(GameObject caller, Collision collision, Vector3 direction,
            ColorPanelProperties properties)
        {
            var isPlayer = collision.gameObject.CompareTag("Player");
            float jumpForce = ComputeJumpForce(isPlayer, properties);
            if (isPlayer && properties.setPanelJump)
                GameController.Instance.m_PlayerController.PlatformJump();

            try
            {
                var collisionRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                collisionRigidbody.velocity = Vector3.zero;
                collisionRigidbody.AddForce(caller != null ? caller.transform.up * jumpForce : direction * jumpForce,
                    ForceMode.Impulse);
            }
            catch (NullReferenceException)
            {
            }
        }

        private static float ComputeJumpForce(bool isPlayer, ColorPanelProperties properties)
        {
            return isPlayer
                ? (GameController.Instance.m_PlayerController.IsGrounded()
                    ? properties.playerPropulsionForce
                    : properties.playerOnAirPropulsionForce)
                : properties.objectPropulsionForce;
        }

        public static void AttractObject(Rigidbody attracted, GameObject attractor, float force = 5f)
        {
            Vector3 temp = (attractor.transform.position - attracted.transform.position);
            if (temp.magnitude > 0.2f)
                attracted.AddForce(temp, ForceMode.Force);
        }
    }
}