using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAppear : MonoBehaviour
{
    public Transform transformo;

    public float targetX;
    public float speedo;
    // Start is called before the first frame update
    void Start()
    {
        transformo = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transformo.position.x > targetX) transformo.position += Vector3.left * (speedo * Time.deltaTime);
    }
}
