using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomConstraint : MonoBehaviour
{
    public List<Transform> Constraints;

    public bool EnableConstraint;

    private Vector3 ConstrainedPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GeneratePosition()
    {
        if (Constraints.Count <= 0) return;
        
        
    }
}
