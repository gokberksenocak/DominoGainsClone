using UnityEngine;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private float _price;
    [SerializeField] private int _level;
    public float Price
    {
        get { return _price; }
        set { _price = value; }
    }
    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }
    public TextMeshProUGUI PriceText 
    {
        get {return _priceText; }
        set {_priceText=value; }
    }
    public TextMeshProUGUI LevelText
    {
        get { return _levelText; }
        set { _levelText = value; }
    }
}
