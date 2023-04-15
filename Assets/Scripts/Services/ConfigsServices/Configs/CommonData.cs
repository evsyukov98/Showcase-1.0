using UnityEngine;

[CreateAssetMenu(fileName = "CommonData", menuName = "CompanyName/Configs/CommonData")]
public class CommonData : ScriptableObject
{
    [Space, Header("Purchase, AD settings:")]
    public bool isPurchaseEnabled;
    public bool isAdsEnabled;
    
    [Space, Header("Shop:")]
    public int skinCost;

    [Space, Header("Player:")] 
    public int heroHealth = 3;
    
    [Space, Header("Loading:")]
    public float firstLoadAppTime = 3f;           // Скорость загрузки при самом первом запуске игры 
    public float loadAppTime = 6f;                // Скорость загрузки при первом запуске игры
    public float interstitialLoadTime = 2.5f;     // Скорость загрузки между окнами.
}
