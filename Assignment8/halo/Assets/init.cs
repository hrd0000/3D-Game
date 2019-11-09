using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init : MonoBehaviour {
    public ParticleSystem particleSystem;
    public Camera camera;
    public int particleNum = 10000; // 粒子数目
    public float minRadius = 5.0f;  // 光环最小半径
    public float maxRadius = 10.0f; // 光环最大半径

    private ParticleSystem.Particle[] particles;
    private float[] particleAngle;  // 粒子角度
    private float[] particleR;  // 粒子半径
    private int speedLevel = 5; // 粒子旋转速度水平
    private float particleSpeed = 0.1f;  // 粒子旋转速度

    private Ray ray;
    private RaycastHit hit;

    private float[] before;   
    private float[] after;   
    private float shrinkSpeed = 2f;
    private bool ischange = false;

	void Start () {
        particleAngle = new float[particleNum];
        particleR = new float[particleNum];
        before = new float[particleNum];
        after = new float[particleNum];
        particles = new ParticleSystem.Particle[particleNum];

        particleSystem.maxParticles = particleNum;
        particleSystem.Emit(particleNum);
        particleSystem.GetParticles(particles);

        Ndistribution nd = new Ndistribution();

        for (int i = 0; i < particleNum; i++) {
            float r = (float)nd.getNormalDistribution((minRadius+maxRadius)*0.5f, 1);
            float angle = UnityEngine.Random.Range(0.0f, 360.0f);
            particleAngle[i] = angle;
            particleR[i] = r;

            before[i] = r;
            after[i] = 0.1f * r;
            
            if (after[i] < minRadius * 1.1f) {
                float midRadius = minRadius * 2f;
                after[i] = UnityEngine.Random.Range(UnityEngine.Random.Range(minRadius, midRadius), (minRadius * 1.1f));
            }
        }
	}

	void Update () {
		for(int i = 0; i < particleNum; i++) {
            if (ischange) {
                if(particleR[i] > after[i]) {
                    particleR[i] -= shrinkSpeed * (particleR[i] / after[i]) * Time.deltaTime;
                }
            } else {
                if (particleR[i] < before[i]) {
                    particleR[i] += shrinkSpeed * (before[i] / particleR[i]) * Time.deltaTime;
                } else if (particleR[i] > before[i]) {
                    particleR[i] = before[i];
                }
            }

            if (i % 2 == 0) {
                particleAngle[i] += (i % speedLevel + 1) * particleSpeed;  
            } else {
                particleAngle[i] -= (i % speedLevel + 1) * particleSpeed;  
            }  

            particleAngle[i] = particleAngle[i] % 360;
            float rad = particleAngle[i] / 180 * Mathf.PI;  

            particles[i].position = new Vector3(particleR[i] * Mathf.Cos(rad), particleR[i] * Mathf.Sin(rad), 0f);  
        }
        particleSystem.SetParticles(particles, particleNum);  
        ray = camera.ScreenPointToRay(Input.mousePosition);  
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "button") ischange = true;  
        else ischange = false;  
	}
}

class Ndistribution {
    System.Random rand = new System.Random();
    public double getNormalDistribution(double mean, double stdDev) {
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }
}