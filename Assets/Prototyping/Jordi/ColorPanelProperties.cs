using UnityEngine;

[CreateAssetMenu(fileName = "Default ColorPanel Properties", menuName = "Properties/ColorPanel Properties", order = 1)]
public class ColorPanelProperties : ScriptableObject
{
    public Material defaultMaterial;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public float playerPropulsionForce = 10f;
    public float playerOnAirPropulsionForce = 12.5f;
    public float objectPropulsionForce = 25f;
    public bool setPanelJump = true;
}