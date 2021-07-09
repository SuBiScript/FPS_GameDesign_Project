using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapon;

public class PanelVisuals : MonoBehaviour
{
    // This class handles all material, shaders and visualization for the panels.
    // Send it a mode and it will switch the colors and sounds IG.
    //
    // This also setups the Lasers and the colors.

    [Title("Components")] public ParticleSystem endPointParticles;
    [Title("Materials")] public Material defaultMaterial;
    public Material redMaterial;
    public Material greenMaterial;

    public Material blueMaterial;

    //public float maxLineDistance = 50f;
    [Title("Settings")]
    //public Transform rayOrigin;
    //public LayerMask rayMask;
    [HideInInspector]
    public LineRenderer _lineRenderer;

    public MeshRenderer _meshRenderer;

    private void Awake()
    {
        //_meshRenderer = GetComponent<MeshRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = true;
    }

    // TODO To avoid raycasting every frame from many cubes, 
    // try only updating the laser when an object is nearby...?
    // Maybe the cube does a scan around for red activated panels and
    // tells them to check if he's in the middle...?
    // Maybe a manager that checks the distance between every object and 
    // update the red panels that have to be updated...?
    // Even better, a trigger around the laser that detects when the trigger is hit and gives the new position
    // instead of having to update the raycast every frame? // DING DING DING I LIKE IT.


    public void UpdateLaserRenderer(Vector3 finalDistance)
    {
        _lineRenderer.SetPosition(1, finalDistance);
    }
    
    public void UpdateLaserParticles(Vector3 position)
    {
        endPointParticles.gameObject.transform.position = position;
    }

    private void OnEnable()
    {
        endPointParticles.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        endPointParticles.gameObject.SetActive(false);
    }

    /*private void OnWillRenderObject()
    {
        // TODO Investigate this, can work WONDERS. WITH ALL PARTICLES! THERE ARE A LOT! OSCAR STOP! TOO MANY!
        // This works good, with some work we can disable all particles that ARE NOT BEING RENDERED. THANKS.
        if (Camera.current.CompareTag("MainCamera") )
        {
            endPointParticles.gameObject.SetActive(true);
        }
    }*/

    public void ChangeVisualColor(WeaponColor color)
    {
        try
        {
            Material[] objectMaterials = _meshRenderer.materials;
            Material materialSlot;
            switch (color)
            {
                case WeaponColor.None:
                    materialSlot = defaultMaterial;
                    break;
                case WeaponColor.Red:
                    materialSlot = redMaterial;
                    break;
                case WeaponColor.Green:
                    materialSlot = greenMaterial;
                    break;
                case WeaponColor.Blue:
                    materialSlot = blueMaterial;
                    break;
                default:
                    materialSlot = defaultMaterial;
                    break;
            }

            objectMaterials[1] = materialSlot;
            _meshRenderer.materials = objectMaterials;
        }
        catch (NullReferenceException)
        {
            // Happens. When no materials are set correctly. Does not matter.
        }
    }
}