using System;
using System.Collections;
using System.Collections.Generic;
using Prototyping.Jordi;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ColorPanelController : MonoBehaviour
{
    float m_JumpForce = 10.0f;
    public Material defaultMaterial;
    public WeaponScript.WeaponColor currentMode;
    public MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        var temp = meshRenderer.materials;
        temp[1] = defaultMaterial;
        meshRenderer.sharedMaterials = temp;
        ChangeColor(WeaponScript.WeaponColor.None, defaultMaterial);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "Player")
        {
            GameController.Instance.m_PlayerController.m_PanelJump = true;

            if (!GameController.Instance.m_PlayerController.IsGrounded())
                m_JumpForce = 12.5f;
            else
                m_JumpForce = 10;
        }
        else if (_Collider.tag == "Cube")
            m_JumpForce = 25;

        _Collider.GetComponent<Rigidbody>().AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
    }

    public bool ChangeColor(WeaponScript.WeaponColor color, Material material)
    {
        //Here you change the weapon material of the block and stuff.
        if (currentMode == color) return false;
        currentMode = color;
        var temp = meshRenderer.materials;
        temp[1] = material;
        meshRenderer.materials = temp;
        return true;
    }
}
