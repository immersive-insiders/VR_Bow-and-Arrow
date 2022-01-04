using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StringInteraction : XRBaseInteractable
{

    [SerializeField] public Transform stringStartPoint;
    [SerializeField] public Transform stringEndPoint;

    private XRBaseInteractor stringInteractor = null;
    private Vector3 pullPoistion;
    private Vector3 pullDirection;
    private Vector3 targetDirection;

    public float PullAmount { get; private set; } = 0.0f;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        stringInteractor = args.interactor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        stringInteractor = null;
        PullAmount = 0f;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
        {
            pullPoistion = stringInteractor.transform.position;
            PullAmount = CalculatePull(pullPoistion);
            Debug.Log("<<<<< Pull amount is "+ PullAmount+" >>>>>");
        }
    }

    private float CalculatePull(Vector3 pullPosition)
    {

        pullDirection = pullPoistion - stringStartPoint.position;
        targetDirection = stringEndPoint.position - stringStartPoint.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();

        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 1);
    }

    
}