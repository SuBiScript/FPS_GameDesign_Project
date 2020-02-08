using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Material List", menuName = "Properties/Material List", order = 2)]
public class MaterialList : ScriptableObject
{
    public Material defaultMaterial;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
}