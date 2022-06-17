using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbScript : MonoBehaviour {

    public ParticleSystem PS;
    public ParticleSystem.Particle[] Particles;
    public Transform Target;
    public float EffectDistance;
	
	// Update is called once per frame
	void Update () {
        if (PS.isPlaying)
        {
            Particles = new ParticleSystem.Particle[PS.particleCount];

            PS.GetParticles(Particles);

            for (int i = 0; i < Particles.GetUpperBound(0); i++)
            {
                Vector2 dirVector = Target.position - Particles[i].position;
                Vector2 dir = dirVector.normalized;
                float dist = dirVector.magnitude;

                float force = ((Particles[i].remainingLifetime) * dist);

                //Debug.DrawRay(Particles[i].position, ((dir * force) * 60) * Time.deltaTime);
                Particles[i].velocity = ((dir * force) * 300) * Time.deltaTime;
                //Particles[i].position = Vector2.Lerp(Particles[i].position, Target.position, Time.deltaTime / 2.0f);

            }
            PS.SetParticles(Particles, Particles.Length);
        }
    }
}
