using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    public TextMeshProUGUI levelText;
    
    private int currentLevel = 1;
    private GameObject[] allEnemies;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in allEnemies)
        {
            enemy.SetActive(false);
        }
        
        StartLevel1();
    }
    
    public void OnPickupCollected(int pickupsRemaining)
    {
        if (pickupsRemaining == 8 && currentLevel == 1)
        {
            StartLevel2();
        }
        else if (pickupsRemaining == 4 && currentLevel == 2)
        {
            StartLevel3();
        }
    }
    
    private void StartLevel1()
    {
        currentLevel = 1;
        UpdateLevelText("Level 1 - Easy");
        
        if (allEnemies.Length > 0) allEnemies[0].SetActive(true);
        if (allEnemies.Length > 1) allEnemies[1].SetActive(true);
    }
    
    private void StartLevel2()
    {
        currentLevel = 2;
        UpdateLevelText("Level 2 - Medium");
        
        if (allEnemies.Length > 2) allEnemies[2].SetActive(true);
        if (allEnemies.Length > 3) allEnemies[3].SetActive(true);
    }
    
    private void StartLevel3()
    {
        currentLevel = 3;
        UpdateLevelText("Level 3 - Hard");
        
        if (allEnemies.Length > 4) allEnemies[4].SetActive(true);
        if (allEnemies.Length > 5) allEnemies[5].SetActive(true);
    }
    
    private void UpdateLevelText(string text)
    {
        if (levelText != null)
        {
            levelText.text = text;
        }
        Debug.Log($"✓ {text}");
    }
}
