using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid asteroidPrefab;
    public float spawnRate = 1.5f;
    public int spawnAmount = 1;
    public float spawnDistance = 15f;
    public float trajectoryVariance = 15f;
    private void Start()
    {
        InvokeRepeating("SpawnAsteroids", spawnRate, spawnRate);
    }

    private void SpawnAsteroids()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized * spawnDistance;
            Vector3 spawnPoint = transform.position + spawnDirection;
            
            float variance = UnityEngine.Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion spawnRotation = Quaternion.AngleAxis(variance, Vector3.forward); ;
            
            Asteroid asteroid = Instantiate(asteroidPrefab, spawnPoint, spawnRotation);
            asteroid.size = UnityEngine.Random.Range(asteroidPrefab.minSize, asteroidPrefab.maxSize);
            asteroid.SetTrajectory(spawnRotation * -spawnDirection);
        }
    }
}
