using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class ArrowInteraction : MonoBehaviour
{
    private XRGrabInteractable xRGrabInteractable;
    private bool inAir;
    private Vector3 lastPosition;
    private Rigidbody arrowRigidBody;

    [SerializeField] private float speed;
    [SerializeField] private Transform tipPosition;

    private void Awake()
    {
        this.arrowRigidBody = GetComponent<Rigidbody>();
        this.inAir = false;
        this.lastPosition = Vector3.zero;
        this.xRGrabInteractable = GetComponent<XRGrabInteractable>();
        this.arrowRigidBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (inAir)
        {
            CheckCollision();
            this.lastPosition = tipPosition.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tipPosition.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.TryGetComponent(out Rigidbody body))
            {
                this.arrowRigidBody.interpolation = RigidbodyInterpolation.None;
                this.transform.parent = hitInfo.transform;
                body.AddForce(arrowRigidBody.velocity, ForceMode.Impulse);
            }
            StopArrow();
        }
    }

    private void StopArrow()
    {
        this.inAir = false;
        SetPhysics(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        this.arrowRigidBody.useGravity = usePhysics;
        this.arrowRigidBody.isKinematic = !usePhysics;
    }

    public void ReleaseArrow(float value)
    {
        this.inAir = true;
        SetPhysics(true);
        MaskAndFire(value);
        StartCoroutine(RotateWithVelocity());
        this.lastPosition = tipPosition.position;

    }

    private void MaskAndFire(float power)
    {
        this.xRGrabInteractable.colliders[0].enabled = false;
        this.xRGrabInteractable.interactionLayerMask = 1 << LayerMask.NameToLayer("Ignore");
        Vector3 force = transform.forward * power * speed;
        this.arrowRigidBody.AddForce(force, ForceMode.Impulse);
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(arrowRigidBody.velocity);
            this.transform.rotation = newRotation;
            yield return null;
        }
    }
}