using UnityEngine;
using System.Collections.Generic;

public class InteractionFlagManager : MonoBehaviour
{
    public static InteractionFlagManager Instance;

    private HashSet<string> unlockedAlternateIDs = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Marca un ID como desbloqueado para usar el audio alternativo.
    /// </summary>
    public void UnlockAlternate(string id)
    {
        if (!string.IsNullOrEmpty(id))
            unlockedAlternateIDs.Add(id);
    }

    /// <summary>
    /// Comprueba si el ID tiene el alternativo desbloqueado.
    /// </summary>
    public bool IsAlternateUnlocked(string id)
    {
        return unlockedAlternateIDs.Contains(id);
    }
}
