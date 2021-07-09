using System;
using System.Net.Sockets;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(PanelVisuals))]
[RequireComponent(typeof(LineRenderer))]
public class LaserCollider : MonoBehaviour
{
    [Range(0f, 100f)] public float LaserMaxDistance = 50f;
    public LayerMask LaserLayerMask;

    public float laserAdjustment = 0.1f;
    public bool UseForward = false;

    private ILaserCast _panel;

    private LineRenderer LR;
    private BoxCollider _collider;
    private PanelVisuals _visuals;

    public ILaserCast lastLaserHit;

    public bool useNewRender = false;

    private bool skipNextVisualUpdate = false;

    private void Awake()
    {
        _panel = transform.GetComponentInParent<ILaserCast>();
        _visuals = GetComponent<PanelVisuals>();
        LR = GetComponent<LineRenderer>();
        ConfigureCollider();
    }

    void ConfigureCollider()
    {
        try
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
        catch (NullReferenceException)
        {
            _collider = this.gameObject.AddComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
    }

    [Button("Update Collider")]
    public void ForceUpdate()
    {
        ConfigureCollider();
        if (LR == null)
            LR = GetComponent<LineRenderer>();
        UpdateCollider();
    }


    /*public void UpdateLaser(float maxLineDistance, LayerMask rayMask)
    {
        // This will update the points for the laser. Avoid updating every frame.
        // Unless it is picked.

        var transform1 = transform;
        Ray ray = new Ray(transform1.position, transform1.up);
        if (Physics.Raycast(ray, out RaycastHit hit, maxLineDistance, rayMask))
        {
            _visuals.UpdateLaserRenderer(transform.position, ray.direction * hit.distance);
        }
        else
        {
            _visuals.UpdateLaserRenderer(transform.position, ray.direction * maxLineDistance);
        }

        UpdateCollider();
        //_lineRenderer.SetPosition(1, ray.direction * maxLineDistance); // hit.point);
        //UpdateLaserParticles(transform.position + ray.direction * maxLineDistance);
    }*/

    public void UpdateLaserAutonomously()
    {
#if UNITY_EDITOR
        //Debug.Log("Updating collider!");
#endif
        var transform1 = transform;
        var position = transform1.position;
        Ray ray = new Ray(position, UseForward ? transform1.forward : transform1.up);

        var distance = LaserMaxDistance;
        Vector3 endPoint = ray.origin + ray.direction * (distance + laserAdjustment);
        if (Physics.Raycast(ray, out RaycastHit hit, LaserMaxDistance, LaserLayerMask) &&
            hit.transform.gameObject != this.gameObject.transform.parent.gameObject)
        {
            distance = hit.distance;
            endPoint = hit.point;
        }

        _visuals.UpdateLaserRenderer((UseForward ? Vector3.forward : Vector3.up) * (distance + laserAdjustment));
        _visuals.UpdateLaserParticles(endPoint);

        UpdateCollider();
    }

    void UpdateCollider()
    {
        Vector3[] positions = new Vector3[2];
        LR.GetPositions(positions);
        Vector3 AB = positions[1] - positions[0];
        float Length = AB.magnitude;
        _collider.center = positions[0] + (AB * 0.5f);
        var width = LR.startWidth * 0.5f;
        if (UseForward)
        {
            _collider.size = new Vector3(width, width, Length);
            return;
        }

        _collider.size = new Vector3(width, Length, width);
    }

    //TODO put this to good work.
    /*void UpdateColliderAndVisuals(Collider other)
    {
        if (other.GetHashCode() == _collider.GetHashCode()) return;
        _collider.enabled = false;

        float length;
        Vector3 distance;

        var width = LR.startWidth * 0.5f;
        if (UseForward)
        {
            float newHeight = other.bounds.center.z;
            distance = this.gameObject.transform.position + (Vector3.forward * newHeight);
            length = distance.magnitude;
            _collider.size = new Vector3(width, width, length);
            _collider.center = transform.position + Vector3.forward * newHeight;
            _visuals.UpdateLaserRenderer(distance, new Vector3(0, 0, length));
        }
        else
        {
            float newHeight = other.bounds.center.y;
            distance = this.gameObject.transform.position + (Vector3.up * newHeight);
            length = distance.magnitude;
            _collider.size = new Vector3(width, length, width);
            _collider.center = transform.position + Vector3.up * newHeight;
            _visuals.UpdateLaserRenderer(distance, new Vector3(0, length, 0));
        }

        _collider.enabled = true;
    }*/

    // void UpdateVisualsWithColision(Collider other)
    // {
    //     Debug.Log("Updating Laser");
    //     var transform1 = transform;
    //     var position = transform1.position;
    //
    //     Vector3 AB = other.gameObject.transform.position - position;
    //     float distance = AB.magnitude;
    //
    //     Vector3 realDistance = (UseForward ? Vector3.forward : Vector3.up) * (distance + laserAdjustment);
    //     Vector3 closestPoint =
    //         other.ClosestPointOnBounds(position + transform1.parent.up * (realDistance.magnitude * 0.8f));
    //
    //     _visuals.UpdateLaserRenderer(realDistance);
    //     // _visuals.UpdateLaserParticles(transform.position + transform.parent.up * realDistance.magnitude);
    //
    //     _visuals.UpdateLaserParticles(closestPoint);
    //     UpdateCollider();
    // }

    private void OnTriggerEnter(Collider other)
    {
        ReplaceLaser(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ReplaceLaser(other);
    }

    private void ReplaceLaser(Collider other)
    {
        try
        {
            ILaserCast newLaser = other.gameObject.GetComponent<ILaserCast>();

            if ((newLaser != null && newLaser.GetHashCode() != lastLaserHit?.GetHashCode()))
            {
                lastLaserHit?.StopCasting();
                lastLaserHit = newLaser;
            }

            lastLaserHit?.CastLaser();
        }
        catch (Exception e)
        {
            // ignored
        }

        if (skipNextVisualUpdate) return;
        UpdateLaserAutonomously();
    }
    // private void OnTriggerStay(Collider other)
    // {
    //     try
    //     {
    //         if (lastLaserHit.CastLaser())
    //         {
    //             UpdateVisualsWithColision(other);
    //             //UpdateLaserAutonomously();
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         // ignored
    //     }
    // }

    private void OnTriggerExit(Collider other)
    {
        try
        {
            if (other.gameObject.GetComponent<ILaserCast>() == lastLaserHit)
            {
                lastLaserHit.StopCasting();
                lastLaserHit = null;
            }

            //UpdateVisualsWithColision(other);
        }
        catch (Exception e)
        {
            // ignored
        }

        UpdateLaserAutonomously();
    }

    private void OnDisable()
    {
        lastLaserHit?.StopCasting();
        lastLaserHit = null;
        //UpdateLaserAutonomously();
    }

    private void OnBecameInvisible()
    {
        skipNextVisualUpdate = true;
    }

    private void OnBecameVisible()
    {
        skipNextVisualUpdate = false;
        UpdateLaserAutonomously();
    }
}