using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlayEventInvoker : MonoBehaviour
{
    private ParticleSystem particle;
    private event Action onPlayAction;
    private bool wasPlaying = false;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        wasPlaying = particle.isPlaying;
    }
    void Update()
    {
        if (!wasPlaying && particle.isPlaying)
        {
            onPlayAction?.Invoke();
        }

        wasPlaying = particle.isPlaying;
    }
    public void SetAction(Action action)
    {
        onPlayAction = action;
    }
}
