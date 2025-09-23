using UnityEngine;

// This defines one group of enemies within a wave
[System.Serializable]
public class SubWave
{
    public EnemyData enemyType;
    public int count;
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "New Wave", menuName = "Enemies/Wave Data")]
public class WaveData : ScriptableObject
{
    public SubWave[] subWaves;
    public float timeBetweenSubWaves = 2.0f;
}
