using UnityEngine;
using UnityEngine.XR;

public class SwingInputState : MonoBehaviour
{
    public bool IsTriggerPressed { get; private set; }

    private UnityEngine.XR.InputDevice leftController;

    void Start()
    {
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        bool pressed;
        leftController.TryGetFeatureValue(CommonUsages.triggerButton, out pressed);
        IsTriggerPressed = pressed;
    }
}