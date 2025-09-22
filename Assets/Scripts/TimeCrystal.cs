using UnityEngine;
using System.Collections.Generic;

public class TimeCrystal : MonoBehaviour
{
    [Header("Thread Settings")]
    public int maxTurretConnections = 1;
    public int maxMortarConnections = 1;
    
    [Header("Unity Setup")]
    public List<Turret> connectedTurrets = new List<Turret>();
    public List<Mortar> connectedMortars = new List<Mortar>();
    public GameObject threadPrefab;

    // A dictionary to reliably track which thread belongs to which tower.
    private Dictionary<Transform, GameObject> activeThreads = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// Activates the rewind ability, consuming a charge from the GameManager.
    /// </summary>
    public void ActivateRewind()
    {
        if (GameManager.instance.UseRewindCharge())
        {
            // Rewind all connected Turrets
            foreach (Turret turret in connectedTurrets)
            {
                if (turret != null) turret.Rewind();
            }
            // Rewind all connected Mortars
            foreach (Mortar mortar in connectedMortars)
            {
                if (mortar != null) mortar.Rewind();
            }
        }
    }

    // --- Connection Logic for Turrets ---
    public void AddConnection(Turret turret)
    {
        if (!connectedTurrets.Contains(turret) && connectedTurrets.Count < maxTurretConnections)
        {
            connectedTurrets.Add(turret);
            CreateThreadVisual(turret.transform);
        }
    }
    
    public void RemoveConnection(Turret turret)
    {
        if (connectedTurrets.Contains(turret))
        {
            connectedTurrets.Remove(turret);
            RemoveThreadVisual(turret.transform);
        }
    }

    // --- Connection Logic for Mortars ---
    public void AddConnection(Mortar mortar)
    {
        if (!connectedMortars.Contains(mortar) && connectedMortars.Count < maxMortarConnections)
        {
            connectedMortars.Add(mortar);
            CreateThreadVisual(mortar.transform);
        }
    }

    public void RemoveConnection(Mortar mortar)
    {
        if (connectedMortars.Contains(mortar))
        {
            connectedMortars.Remove(mortar);
            RemoveThreadVisual(mortar.transform);
        }
    }

    /// <summary>
    /// Creates a visual thread connecting the crystal to a target tower.
    /// </summary>
    void CreateThreadVisual(Transform target)
    {
        if (threadPrefab != null && !activeThreads.ContainsKey(target))
        {
            GameObject threadGO = Instantiate(threadPrefab, Vector3.zero, Quaternion.identity);
            ThreadSprite thread = threadGO.GetComponent<ThreadSprite>();
            if (thread != null)
            {
                thread.startPoint = this.transform;
                thread.endPoint = target;
            }
            activeThreads.Add(target, threadGO);
        }
    }

    /// <summary>
    /// Finds and destroys the visual thread connected to a specific tower.
    /// </summary>
    void RemoveThreadVisual(Transform target)
    {
        if (activeThreads.ContainsKey(target))
        {
            Destroy(activeThreads[target]);
            activeThreads.Remove(target);
        }
    }
}

