using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleTriggerHandler : MonoBehaviour
{
    public ParticleSystem ps;
    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
    //ParticleSystem.Particle[] tempParticles = new ParticleSystem.Particle[8];
    static Action onComplete;
    static Action onFirstCollision;

    RectTransform toShake = null;
    bool started = false;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        //Since you want a failsafe of destroying particles after a certain time (to prevent infinite loop) you need this check here too as some particles will not collide
        if (!started && ps.particleCount > 0)
            started = true;

        if (ps.particleCount == 0 && started)
        {
            onComplete?.Invoke();
            ps.Stop();
            started = false;
            //Destroy(gameObject);
        }
    }

    public void SetInfoAndPlay(CreatureType ct, int numParts, Color color, Color trailColor, Component collide, Action firstCollideAction, Action endAction)
    {
        onComplete = endAction;
        onFirstCollision = firstCollideAction;

        ps.emission.SetBurst(0, new ParticleSystem.Burst() { count = numParts, probability = 1, repeatInterval = 1, cycleCount = 1, maxCount = (short)numParts, minCount = (short)numParts, time = 0 });

        ps.trigger.AddCollider(collide);
        var main = ps.main;
        main.startColor = color;
        //var trails = ps.trails;
        //Gradient g = new Gradient();

        //g.SetKeys(new GradientColorKey[2] { new GradientColorKey() { color = trailColor, time = 0 }, new GradientColorKey() { color = new Color(trailColor.r, trailColor.g, trailColor.b, 0), time = 1 } },
        //          new GradientAlphaKey[2] { new GradientAlphaKey() { alpha = 1, time = 0 }, new GradientAlphaKey() { time = 1, alpha = 0 } });
        //trails.colorOverTrail = new ParticleSystem.MinMaxGradient() { gradient = g, mode = ParticleSystemGradientMode.Gradient };

        ps.Play();
    }


    private void OnParticleTrigger()
    {
        onFirstCollision?.Invoke();
        onFirstCollision = null;


        onComplete?.Invoke();
        ps.Clear();
        ps.Stop();
        started = false;
    }
}
