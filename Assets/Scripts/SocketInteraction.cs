using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(StringInteraction))]
public class SocketInteraction : XRSocketInteractor
{

    private XRBaseInteractor handHoldingArrow;
    private XRBaseInteractable currentArrow;
    private StringInteraction stringInteraction;
    private BowInteraction bowInteraction;
    private ArrowInteraction currentArrowInteraction;

    protected override void Awake()
    {
        base.Awake();
        this.stringInteraction = GetComponent<StringInteraction>();
        this.bowInteraction = GetComponentInParent<BowInteraction>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        stringInteraction.selectExited.AddListener(TryToReleasaeArrow);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        stringInteraction.selectExited.RemoveListener(TryToReleasaeArrow);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        this.handHoldingArrow = args.interactable.selectingInteractor;
        if (args.interactable.tag == "Arrow" && bowInteraction.BowHeld)
        {
            interactionManager.SelectExit(handHoldingArrow, args.interactable);
            interactionManager.SelectEnter(handHoldingArrow, stringInteraction);
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        StoreArrow(args.interactable);
    }

    private void StoreArrow(XRBaseInteractable interactable)
    {
        if (interactable.tag == "Arrow")
        {
            this.currentArrow = interactable;
            this.currentArrowInteraction = currentArrow.gameObject.GetComponent<ArrowInteraction>();
        }
    }

    private void TryToReleasaeArrow(SelectExitEventArgs arg0)
    {
        if (currentArrow && bowInteraction.BowHeld)
        {
            ForceDetach();
            ReleaseArrowFromSocket();
            ClearVariables();
        }
    }

    public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
    {
        get { return XRBaseInteractable.MovementType.Instantaneous; }
    }

    private void ForceDetach()
    {
        interactionManager.SelectExit(this, currentArrow);
    }

    private void ReleaseArrowFromSocket()
    {
        currentArrowInteraction.ReleaseArrow(stringInteraction.PullAmount);
    }

    private void ClearVariables()
    {
        this.currentArrow = null;
        this.currentArrowInteraction = null;
        this.handHoldingArrow = null;
    }
}