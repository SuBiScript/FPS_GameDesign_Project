using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LaserPillar : MonoBehaviour
{
    [FoldoutGroup("Components")] public LineRenderer topLaserRenderer;
    [FoldoutGroup("Components")] public LineRenderer midLaserRenderer;
    [FoldoutGroup("Components")] public LineRenderer botLaserRenderer;

    [OnValueChanged("UpdateColors")] public Gradient laserColor;
    [OnValueChanged("UpdateMaterials")] public Material laserMaterial;

    [OnValueChanged("ToggleCollider")] public bool GenerateCollider = false;
    [ShowIf("GenerateCollider")] public BoxCollider wallCollider;

    [Header("Pillars")] public LaserPillar nextPillar;

#if UNITY_EDITOR
    [HideIf("@this.nextPillar == null")]
    [Button("Update Pillar", ButtonSizes.Large)]
    public void UpdateConnectedPillars()
    {
        UpdatePillar(nextPillar);
    }

    [HideIf("@this.nextPillar != null")]
    [Button("Add Next Pillar", ButtonSizes.Large)]
    public void AddNextPillar()
    {
        GameObject o;
        nextPillar = Instantiate((o = gameObject), o.transform.position + Vector3.forward,
            Quaternion.identity).GetComponent<LaserPillar>();
        
        var pillarGo = nextPillar.gameObject;
        pillarGo.name = this.gameObject.name;
        Selection.activeGameObject = pillarGo;
    }

    private void Update()
    {
        UpdatePillar(nextPillar);
    }
#endif
    private void Start()
    {
        UpdatePillar(nextPillar);
    }

    private void UpdatePillar(LaserPillar tower)
    {
        if (!tower)
        {
            ToggleLasers(false);
            return;
        }

        ToggleLasers(true);

        ConfigurePositions(topLaserRenderer, tower.topLaserRenderer);
        ConfigurePositions(midLaserRenderer, tower.midLaserRenderer);
        ConfigurePositions(botLaserRenderer, tower.botLaserRenderer);

        if (GenerateCollider)
        {
            UpdateCollider();
        }
    }

    private void ConfigurePositions(LineRenderer lr, Vector3 posOne)
    {
        lr.useWorldSpace = true;

        lr.SetPosition(0, lr.transform.position);
        lr.SetPosition(1, posOne);
    }

    private void ConfigurePositions(LineRenderer lr, LineRenderer lr2)
    {
        lr.useWorldSpace = true;

        lr.SetPosition(0, lr.transform.position);
        lr.SetPosition(1, lr2.transform.position);
    }

    private void ToggleLasers(bool enable)
    {
        topLaserRenderer.enabled = enable;
        midLaserRenderer.enabled = enable;
        botLaserRenderer.enabled = enable;
    }

    private void UpdateColors()
    {
        topLaserRenderer.colorGradient = laserColor;
        midLaserRenderer.colorGradient = laserColor;
        botLaserRenderer.colorGradient = laserColor;
    }

    private void UpdateMaterials()
    {
        topLaserRenderer.material = laserMaterial;
        midLaserRenderer.material = laserMaterial;
        botLaserRenderer.material = laserMaterial;
    }

    private void ToggleCollider()
    {
        if (!wallCollider)
            wallCollider = GetComponentInChildren<BoxCollider>();
        wallCollider.enabled = GenerateCollider;
    }

    private void UpdateCollider()
    {
        // Here we set the collider values and sizes to the correct thing.
        Vector3 direction = nextPillar.transform.position - this.gameObject.transform.position;
        wallCollider.center = new Vector3(0f, 0f, direction.magnitude * 0.5f);
        wallCollider.transform.forward = direction.normalized;

        Vector3 prevSize = wallCollider.size;
        prevSize = new Vector3(prevSize.x, prevSize.y, direction.magnitude - 0.25f);
        wallCollider.size = prevSize;
    }
}