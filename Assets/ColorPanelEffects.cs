using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ColorPanels
{
    public static class ColorPanelEffects
    {
        private static ColorPanelObjectFSM.ColorPanelProperties lastPanelProperties;
        private static Vector3 jumpDirection;

        public static void ThrowObject(GameObject caller, Collision collision, Vector3 direction,
            ColorPanelObjectFSM.ColorPanelProperties properties)
        {
            var jumpForce = ComputeJumpForce(properties);
            try
            {
                var collisionRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                collisionRigidbody.velocity = Vector3.zero;
                collisionRigidbody.mass = 1f;
                collisionRigidbody.AddForce(caller != null ? caller.transform.up * jumpForce : direction * jumpForce,
                    ForceMode.Impulse);
            }
            catch (NullReferenceException)
            {
            }

        }

        public static void PlayerJump(PlayerControllerFSM player, Rigidbody rigidbody)
        {
            player.enableAirControl = lastPanelProperties.enableAirControl;
            rigidbody.AddForce(jumpDirection * lastPanelProperties.playerPropulsionForce, ForceMode.Impulse);
        }

        public static void PanelSetProperties(ColorPanelObjectFSM.ColorPanelProperties panelProperties, Vector3 direction)
        {
            lastPanelProperties = panelProperties;
            jumpDirection = direction;
        }

        private static float ComputeJumpForce(ColorPanelObjectFSM.ColorPanelProperties properties)
        {
            return properties.objectPropulsionForce;
        }

        public static void AttractObject(Rigidbody attracted, GameObject attractor, float force = 5f)
        {
            Vector3 temp = (attractor.transform.position - attracted.transform.position);
            if (temp.magnitude > 0.2f)
                attracted.AddForce(temp * force, ForceMode.Force);
        }

        public static void UpdateAttachedObject(Rigidbody objectAttached, GameObject m_AttachingPosition,
            float m_AttachingObjectSpeed = 25f)
        {
            if (objectAttached == null || m_AttachingPosition == null) return;
            //objectAttached.gameObject.transform.parent = m_AttachingPosition.transform;
            Vector3 l_EulerAngles = m_AttachingPosition.transform.rotation.eulerAngles;

            Quaternion m_AttachingObjectStartRotation = objectAttached.transform.rotation;
            Vector3 l_Direction = m_AttachingPosition.transform.position - objectAttached.transform.position;

            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;

            if (l_Movement >= l_Distance)
            {
                objectAttached.MovePosition(m_AttachingPosition.transform.position);
                objectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            }
            else
            {
                l_Direction /= l_Distance;
                objectAttached.MovePosition(objectAttached.transform.position + l_Direction * l_Movement);
                objectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation,
                    Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z),
                    1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            }
        }
    }
}