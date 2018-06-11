﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleController : MonoBehaviour {

    public ParticleSystem particeSystem;

    public int particleCount_1;
    public int particleCount_2;
    public int particleCount_3;
    public int particleCount_4;
    public int particleCount_5;
    public int particleCount_6;
    public int particleCount_7;
    public int particleCount_8;
    public int particleCount_9;
    public int particleCount_10;
    
    public void SetIntensity(int intensity) {
        var em = particeSystem.emission;

        switch (intensity) {
            case 0:
                em.rateOverTime = 0;
                break;
            case 1:
                em.rateOverTime = particleCount_1;
                break;
            case 2:
                em.rateOverTime = particleCount_2;
                break;
            case 3:
                em.rateOverTime = particleCount_3;
                break;
            case 4:
                em.rateOverTime = particleCount_4;
                break;
            case 5:
                em.rateOverTime = particleCount_5;
                break;
            case 6:
                em.rateOverTime = particleCount_6;
                break;
            case 7:
                em.rateOverTime = particleCount_7;
                break;
            case 8:
                em.rateOverTime = particleCount_8;
                break;
            case 9:
                em.rateOverTime = particleCount_9;
                break;
            case 10:
                em.rateOverTime = particleCount_10;
                break;
            //default:
            //    Debug.LogError("PARTICLECONTROLLER " + this.name + ": received a incorrect intensity value (" +  intensity + ")");
        }
    }
    void Awake () {
        var emision = particeSystem.emission;
        emision.rateOverTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
