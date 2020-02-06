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
        Interact(_Collider);
        /*if (_Collider.tag == "Player")
        {
            GameController.Instance.m_PlayerController.m_PanelJump = true; //TODO Implement blackboard

            if (!GameController.Instance.m_PlayerController.IsGrounded())
                m_JumpForce = 12.5f;
            else
                m_JumpForce = 10;
        }
        else if (_Collider.tag == "Cube")
            m_JumpForce = 25;

        _Collider.GetComponent<Rigidbody>().AddForce(transform.up * m_JumpForce, ForceMode.Impulse);*/
    }

    private void Interact(Collider collider)
    {
        switch (currentMode)
        {
            case WeaponScript.WeaponColor.None:
                break;
            case WeaponScript.WeaponColor.Red:
                Debug.Log("RED Activated!");
                break;
            case WeaponScript.WeaponColor.Green:
                Debug.Log("GREEN Activated!");
                break;
            case WeaponScript.WeaponColor.Blue:
                var jumpForce = collider.CompareTag("Player") ? (GameController.Instance.m_PlayerController.IsGrounded() ? 10f : 12.5f) : 25f;
                collider.GetComponent<Rigidbody>()?.AddForce(transform.up*jumpForce,ForceMode.Impulse);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
