using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class LandingPoint : MonoBehaviour
{
    private bool IgnoreParent = true;

    [DisableIf("IgnoreParent")][HideLabel] [VerticalGroup("Split")]
    public Vector3 FixedPosition;

    [DisableIf("IgnoreParent")][Button("Store Transform's Position", ButtonSizes.Small)] [VerticalGroup("Split")]
    public void StoreCurrentPosition()
    {
        FixedPosition = this.gameObject.transform.position;
    }
    
    [ShowIf("IgnoreParent")]
    [Button("Enable Edit", ButtonSizes.Medium), GUIColor(0,1,0)]
    public void EditPosition()
    {
        IgnoreParent = false;
    }
    
    [HideIf("IgnoreParent")]
    [Button("Disable Edit", ButtonSizes.Medium), GUIColor(1,0.2f,0)]
    public void StorePosition()
    {
        IgnoreParent = true;
    }

    private void Update()
    {
        if (IgnoreParent)
            transform.position = FixedPosition;
    }
}