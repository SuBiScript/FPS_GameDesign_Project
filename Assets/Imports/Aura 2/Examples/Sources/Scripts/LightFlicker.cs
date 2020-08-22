/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Aura2API
{
    //[ExecuteInEditMode]
    public class LightFlicker : MonoBehaviour
    {
        public float maxFactor = 1.2f;
        public float minFactor = 1.0f;
        public float moveRange = 0.1f;
        public float speed = 0.1f;
        public float flickeringTime = 2.0f;
        public float timeToRestartFlickering = 1.0f;

        private float _currentFactor = 1.0f;
        private Vector3 _currentPos;
        private float _deltaTime;
        private Vector3 _initPos;
        private float _targetFactor;
        private Vector3 _targetPos;
        private float _initialFactor;
        private Color _initialMaterialcolor;
        private float _initialFactorEmissor;
        private float _time;
        private float _timeLeft;
        private float currentTimeToRestartFlickering;
        private float currentFlickeringTime;
        private bool stopFlickering;

        public Material m_StatusMaterial;
        private const string c_EmissionColor = "_EmissionColor";
        private Material[] m_StatusMaterials;

        private void Start()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                var materials = renderers[i].sharedMaterials;
                for (int j = 0; j < materials.Length; ++j)
                {
                    if (materials[j] != m_StatusMaterial)
                        continue;

                    if (m_StatusMaterials == null)
                        m_StatusMaterials = new Material[1];
                    else
                        System.Array.Resize(ref m_StatusMaterials, m_StatusMaterials.Length + 1);

                    m_StatusMaterials[m_StatusMaterials.Length - 1] = renderers[i].materials[j];
                }
            }


            Random.InitState((int)transform.position.x + (int)transform.position.y);
            _initialFactor = GetComponentInChildren<Light>().intensity;

            if (m_StatusMaterials != null)
                _initialMaterialcolor = m_StatusMaterials[0].GetColor("_EmissionColor");

            currentFlickeringTime = flickeringTime + Time.time;
        }


        private void OnEnable()
        {
            _initPos = transform.localPosition;
            _currentPos = _initPos;
        }


        private void OnDisable()
        {
            transform.localPosition = _initPos;
        }

        private void Update()
        {
            if (!stopFlickering)
            {
                if (currentFlickeringTime >= Time.time)
                    Flickering();
                else
                {
                    if (m_StatusMaterials != null)
                        m_StatusMaterials[0].SetColor("_EmissionColor", _initialMaterialcolor * _initialFactor);

                    GetComponentInChildren<Light>().intensity = _initialFactor * _initialFactor;

                    currentTimeToRestartFlickering = timeToRestartFlickering + Time.time;
                    stopFlickering = true;
                }
            }
            else
            {
                if (currentTimeToRestartFlickering <= Time.time)
                {
                    currentFlickeringTime = flickeringTime + Time.time;
                    stopFlickering = false;
                }
            }
        }

        private void Flickering()
        {

            _deltaTime = Time.deltaTime;

            float currentTime = Time.realtimeSinceStartup;
            _deltaTime = currentTime - _time;
            _time = currentTime;

            if (_timeLeft <= _deltaTime)
            {
                _targetFactor = Random.Range(minFactor, maxFactor);
                _targetPos = _initPos + Random.insideUnitSphere * moveRange;
                _timeLeft = speed;
            }
            else
            {
                float weight = _deltaTime / _timeLeft;
                _currentFactor = Mathf.Lerp(_currentFactor, _targetFactor, weight);
                GetComponentInChildren<Light>().intensity = _initialFactor * _currentFactor;

                if (m_StatusMaterials != null)
                    m_StatusMaterials[0].SetColor("_EmissionColor", _initialMaterialcolor * _currentFactor);

                _currentPos = Vector3.Lerp(_currentPos, _targetPos, weight);
                transform.localPosition = _currentPos;
                _timeLeft -= _deltaTime;
            }
        }
    }
}
