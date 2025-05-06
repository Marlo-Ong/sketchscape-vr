using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrawAudio : MonoBehaviour
{
    private AudioSource source;
    private Vector3 previousPosition;
    private Coroutine paintAudioLoop;

    private void Start()
    {
        this.source = GetComponent<AudioSource>();
        this.source.Play();
        this.source.Pause();
    }

    private void OnEnable()
    {
        TexturePainter.OnStartPainting += this.StartPaintAudio;
        TexturePainter.OnStopPainting += this.StopPaintAudio;
    }

    private void OnDisable()
    {
        TexturePainter.OnStartPainting -= this.StartPaintAudio;
        TexturePainter.OnStopPainting -= this.StopPaintAudio;
    }

    private void StopPaintAudio()
    {
        this.source.Pause();
        if (this.paintAudioLoop != null)
        {
            StopCoroutine(this.paintAudioLoop);
            this.paintAudioLoop = null;
        }
    }

    private void StartPaintAudio(Transform tr)
    {
        this.source.UnPause();
        this.paintAudioLoop = StartCoroutine(ContinuePaintAudio(tr));
    }

    private IEnumerator ContinuePaintAudio(Transform tr)
    {
        while (this.source.isPlaying)
        {
            var pos = tr.position;
            var velocity = (pos - previousPosition) / Time.deltaTime;
            float newPitch = Mathf.Clamp(1 + velocity.sqrMagnitude, 1, 3);
            this.source.pitch = newPitch;
            previousPosition = pos;
            yield return null;
        }
    }
}