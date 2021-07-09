using Sirenix.OdinInspector;
using UnityEngine;

// https://www.omnicalculator.com/physics/projectile-motion#projectile-motion-equations
[ExecuteInEditMode]
public class JumpDistanceCalculator : MonoBehaviour
{
    private PlayerMover Player;
    [AssetSelector]
    public Mesh MeshItem;
    public bool CalculateFromFeet =false;
    private void Awake()
    {
        Player = GetComponent<PlayerMover>();
    }

    private void OnDrawGizmos()
    {
        //if (Player != null && Player.Controller != null) return;
        Gizmos.DrawWireMesh(MeshItem, GetJumpPosition(), Quaternion.identity);
    }

    private Vector3 GetJumpPosition()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f);
        Vector3 newPos = hit.point;

        newPos.y += Player.Properties.ScaledMaxHeight;
        
        if (!CalculateFromFeet)
            newPos.y += Player.Controller.height * 0.5f;

        return newPos;
    }
}