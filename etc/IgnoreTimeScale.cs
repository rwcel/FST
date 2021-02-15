using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreTimeScale : SingleTon<IgnoreTimeScale>
{
    [SerializeField] ParticleSystem[] _Particles;
    private int length;

    private void Start() {
        length = _Particles.Length;
        enabled = false;
    }

    private void Update() {
        for (int i = 0; i < length; i++) {
            _Particles[i].Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
