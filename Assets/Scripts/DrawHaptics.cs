using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(HapticImpulsePlayer))]
public class DrawHaptics : MonoBehaviour
{
    [SerializeField] private HapticImpulseData properties;
    [Tooltip("Which brush (controller) to send haptic feedback to")]
    [SerializeField] private TexturePainter.Brush brush;

    private HapticImpulsePlayer haptics;

    private void Start()
    {
        this.haptics = GetComponent<HapticImpulsePlayer>();
    }

    private void OnEnable()
    {
        TexturePainter.OnStartPainting += this.PlayHaptics;
    }

    private void OnDisable()
    {
        TexturePainter.OnStartPainting -= this.PlayHaptics;
    }

    private void PlayHaptics(TexturePainter.Brush brush)
    {
        if (brush != this.brush)
            return;

        this.PlayHaptics();
    }

    private void PlayHaptics()
    {
        this.haptics.SendHapticImpulse(this.properties.amplitude, this.properties.duration);
    }
}