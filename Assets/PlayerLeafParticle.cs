using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeafParticle : MonoBehaviour
{
    public ParticleSystem particleFront;
    public ParticleSystem particleLeft;
    public ParticleSystem particleBack;
    public ParticleSystem particleRight;
    bool[] particlesState = {false,false,false,false};

    void Update()
    {
        //turn on/off particles according the particlesState
        ParticleOnOff(particlesState[0], particleFront);
        ParticleOnOff(particlesState[1], particleLeft);
        ParticleOnOff(particlesState[2], particleBack);
        ParticleOnOff(particlesState[3], particleRight);

        //turn off all particles in next update
        particlesState[0] = particlesState[1] = particlesState[2] = particlesState[3] = false; 
    }

    void ParticleOnOff(bool state, ParticleSystem particleSystem) //turn on/off particles according the particlesState
    {
        if (state && !particleSystem.isPlaying)
            particleSystem.Play();
        else if (!state && particleSystem.isPlaying)
            particleSystem.Stop();
    }

    public void SetLeafParticleOn(int n)
    {
        particlesState[n] = true;
    }
}
