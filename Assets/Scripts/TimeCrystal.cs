using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeCrystal : MonoBehaviour
{
    [Header("Region Settings")]
    public float innerRegionRadius = 10f;
    public float midRegionRadius = 20f;

    [Header("Region Penalties (Currency Cost to Connect)")]
    public int midRegionPenaltyCost = 25;
    public int outerRegionPenaltyCost = 50;

    [Header("Region Penalties (Rewind Time Increase)")]
    public float midRegionTimePenalty = 1.0f;
    public float outerRegionTimePenalty = 2.0f;

    [Header("Rewind Time Settings")]
    public float baseRewindCastTime = 1.0f;
    private bool isRewinding = false;

    [Header("Connection Limits")]
    public int maxTurretConnections = 1;
    public int maxMortarConnections = 1;
    public int maxFlamethrowerConnections = 1;

    [Header("Connected Towers")]
    public List<Turret> connectedTurrets = new List<Turret>();
    public List<Mortar> connectedMortars = new List<Mortar>();
    public List<Flamethrower> connectedFlamethrowers = new List<Flamethrower>();

    [Header("Unity Setup")]
    public GameObject threadPrefab;
    private Dictionary<object, GameObject> threadVisuals = new Dictionary<object, GameObject>();

    public void ActivateRewind()
    {
        if (isRewinding) return;
        if (GameManager.instance.UseRewindCharge())
        {
            StartCoroutine(RewindSequence());
        }
    }

    IEnumerator RewindSequence()
    {
        isRewinding = true;

        IEnumerator RewindTowerList<T>(List<T> towerList) where T : MonoBehaviour
        {
            List<T> towersToRewind = new List<T>(towerList);

            foreach (var tower in towersToRewind)
            {
                if (tower == null) continue;
                
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                float regionPenalty = 0f;

                if (distance > midRegionRadius) regionPenalty = outerRegionTimePenalty;
                else if (distance > innerRegionRadius) regionPenalty = midRegionTimePenalty;

                float rewindTime = baseRewindCastTime + regionPenalty;
                StartCoroutine(RewindSingleTower(tower, rewindTime));
            }
            yield return null;
        }

        yield return StartCoroutine(RewindTowerList(connectedTurrets));
        yield return StartCoroutine(RewindTowerList(connectedMortars));
        yield return StartCoroutine(RewindTowerList(connectedFlamethrowers));

        yield return new WaitForSeconds(baseRewindCastTime + 5f);
        isRewinding = false;
    }

    IEnumerator RewindSingleTower(MonoBehaviour tower, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (tower != null)
        {
            if (tower is Turret t) t.Rewind();
            if (tower is Mortar m) m.Rewind();
            if (tower is Flamethrower f) f.Rewind();
        }
    }

    private bool CanAffordConnection(MonoBehaviour tower)
    {
        float distance = Vector2.Distance(transform.position, tower.transform.position);
        int penaltyCost = 0;

        if (distance > midRegionRadius) penaltyCost = outerRegionPenaltyCost;
        else if (distance > innerRegionRadius) penaltyCost = midRegionPenaltyCost;

        if (GameManager.instance.currency >= penaltyCost)
        {
            if (penaltyCost > 0) GameManager.instance.SpendCurrency(penaltyCost);
            return true;
        }
        
        return false;
    }

    public void AddConnection(Turret tower)
    {
        if (!connectedTurrets.Contains(tower) && connectedTurrets.Count < maxTurretConnections)
        {
            if (CanAffordConnection(tower))
            {
                connectedTurrets.Add(tower);
                CreateThreadVisual(tower);
            }
        }
    }
    public void RemoveConnection(Turret tower)
    {
        if (connectedTurrets.Contains(tower))
        {
            connectedTurrets.Remove(tower);
            RemoveThreadVisual(tower);
        }
    }
    public void AddConnection(Mortar tower)
    {
        if (!connectedMortars.Contains(tower) && connectedMortars.Count < maxMortarConnections)
        {
            if (CanAffordConnection(tower))
            {
                connectedMortars.Add(tower);
                CreateThreadVisual(tower);
            }
        }
    }
    public void RemoveConnection(Mortar tower)
    {
        if (connectedMortars.Contains(tower))
        {
            connectedMortars.Remove(tower);
            RemoveThreadVisual(tower);
        }
    }
    public void AddConnection(Flamethrower tower)
    {
        if (!connectedFlamethrowers.Contains(tower) && connectedFlamethrowers.Count < maxFlamethrowerConnections)
        {
            if (CanAffordConnection(tower))
            {
                connectedFlamethrowers.Add(tower);
                CreateThreadVisual(tower);
            }
        }
    }
    public void RemoveConnection(Flamethrower tower)
    {
        if (connectedFlamethrowers.Contains(tower))
        {
            connectedFlamethrowers.Remove(tower);
            RemoveThreadVisual(tower);
        }
    }
    
    // --- UPDATED METHOD ---
    private void CreateThreadVisual(MonoBehaviour tower)
    {
        Debug.Log("Attempting to create a thread visual...");

        if (threadPrefab == null)
        {
            Debug.LogError("FAIL: The 'Thread Prefab' is NOT ASSIGNED on the Time Crystal Inspector!");
            return;
        }

        if (threadVisuals.ContainsKey(tower))
        {
            Debug.LogWarning("INFO: A thread for this tower already exists.");
            return;
        }

        GameObject threadGO = Instantiate(threadPrefab, Vector3.zero, Quaternion.identity);
        // --- FIXED LOGIC: Now looks for the correct EnergyThread script ---
        EnergyThread thread = threadGO.GetComponent<EnergyThread>();

        if (thread == null)
        {
            // The error message is also updated to be more helpful
            Debug.LogError("FAIL: The instantiated thread prefab is MISSING the 'EnergyThread.cs' script!");
            Destroy(threadGO);
            return;
        }

        thread.startPoint = this.transform;
        thread.endPoint = tower.transform;
        threadVisuals.Add(tower, threadGO);
        Debug.Log("SUCCESS: Thread visual created and connected to " + tower.name);
    }

    private void RemoveThreadVisual(MonoBehaviour tower)
    {
        if (threadVisuals.ContainsKey(tower))
        {
            Destroy(threadVisuals[tower]);
            threadVisuals.Remove(tower);
            Debug.Log("Thread visual removed for " + tower.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRegionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midRegionRadius);
    }
}

