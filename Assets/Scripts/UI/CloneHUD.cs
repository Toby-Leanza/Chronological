using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CloneHUD : MonoBehaviour
{
    [Header("HUD Settings")]
    public GameObject cloneIconPrefab;
    public Transform iconsContainer;
    public int iconSize = 40;         
    public int spacing = 10;         
    
    [Header("References")]
    public CloneManager cloneManager;
    
    private List<GameObject> cloneIcons = new List<GameObject>();

    void Start()
    {
        UpdateHUD();
    }

    void Update()
    {
        if (cloneManager != null && cloneIcons.Count != cloneManager.clones.Count)
        {
            UpdateHUD();
        }
    }


    public void UpdateHUD()
    {
        if (cloneManager == null) 
        {
            cloneManager = FindObjectOfType<CloneManager>();
            if (cloneManager == null) return;
        }
        
        foreach (var icon in cloneIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        cloneIcons.Clear();
        
        int cloneCount = cloneManager.clones.Count;
        for (int i = 0; i < cloneCount; i++)
        {
            CreateCloneIcon(i);
        }
    }

    private void CreateCloneIcon(int index)
    {
        if (cloneIconPrefab == null || iconsContainer == null) return;
        
        GameObject icon = Instantiate(cloneIconPrefab, iconsContainer);
        RectTransform rect = icon.GetComponent<RectTransform>();
        
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(iconSize, iconSize);
            
            float xPos = index * (iconSize + spacing);
            rect.anchoredPosition = new Vector2(xPos, 0);
        }
        
        cloneIcons.Add(icon);
    }
}