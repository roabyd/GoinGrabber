using UnityEngine.XR;

namespace GoinGrabber
{
    public static class VibrationUtil
    {
        public static void Vibrate(XRNode node, float amplitude = 0.5f, float duration = 0.2f)
        {
            try
            {
                InputDevice device = InputDevices.GetDeviceAtXRNode(node);

                device.SendHapticImpulse(0, amplitude, duration);
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"Failed to send haptic feedback: {ex.Message}");
            }
        }
    }
}
