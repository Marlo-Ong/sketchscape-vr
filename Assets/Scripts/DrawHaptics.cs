using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(HapticImpulsePlayer))]
public class DrawHaptics : MonoBehaviour
{
    [Tooltip("Which brush (controller) to send haptic feedback to")]
    [SerializeField] private TexturePainter.Brush brush;

    private HapticImpulsePlayer haptics;
    private Vector3 previousPosition;
    private Coroutine paintHapticLoop;

    private void Start()
    {
        this.haptics = GetComponent<HapticImpulsePlayer>();
    }

    private void OnEnable()
    {
        TexturePainter.OnStartPainting += this.PlayHaptics;
        TexturePainter.OnStopPainting += this.StopHaptics;
    }

    private void OnDisable()
    {
        TexturePainter.OnStartPainting -= this.PlayHaptics;
        TexturePainter.OnStopPainting -= this.StopHaptics;
    }

    private void StopHaptics(TexturePainter.Brush inputState)
    {
        // Only stop when this brush flag is gone AND we actually have a loop running.
        if (inputState.HasFlag(this.brush) || this.paintHapticLoop == null)
            return;

        StopCoroutine(paintHapticLoop);
        this.paintHapticLoop = null;
    }

    private void PlayHaptics(TexturePainter.Brush inputState)
    {
        // If this brush just turned on, and we're not already looping, start once.
        if (!inputState.HasFlag(this.brush) || this.paintHapticLoop != null)
            return;

        // Reset the previous position so the very first frame isn't a giant spike.
        this.previousPosition = this.transform.position;
        this.paintHapticLoop = StartCoroutine(ContinuePaintHaptics(this.transform));
    }

    private IEnumerator ContinuePaintHaptics(Transform tr)
    {
        while (true)
        {
            var pos = tr.position;
            var velocity = (pos - this.previousPosition) / Time.deltaTime;
            float amplitude = Mathf.Clamp(velocity.sqrMagnitude, 0, 1);
            this.haptics.SendHapticImpulse(amplitude, .2f);
            this.previousPosition = pos;
            yield return null;
        }
    }
}