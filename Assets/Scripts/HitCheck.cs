using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class HitCheck : MonoBehaviour
{
    [SerializeField] private DominoManager _dominoManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private List<TextMeshPro> _incomeNumbersTexts;
    private int _incomeNumbersIndex;
    private bool _firstTime;
    private bool _hitted;
    public bool Hitted 
    {
        get {return _hitted; }
        set { _hitted = value; }
    }
    public bool FirstTime
    {
        get { return _firstTime; }
        set { _firstTime = value; }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && !_firstTime)
        {
            _hitted = true;
            DominoContactText(new Vector3(-1.8f, 0.3f, 1f));
            ColorIncomeCheck();
        }
        else if (other.CompareTag("Player") && !_firstTime)
        {
            DominoContactText(new Vector3(0f, -2f, 1f));
            ColorIncomeCheck();
        }
    }
    void ColorIncomeCheck()
    {
        if (gameObject.transform.parent.GetComponent<MeshRenderer>().sharedMaterial == _gameManager.BlueMat)
        {
            _gameManager.Money += _gameManager.Income;
            _gameManager.MoneyText.text = _gameManager.Money.ToString();
            _incomeNumbersTexts[_incomeNumbersIndex].text = _gameManager.Income.ToString() + "$";
        }
        else if (gameObject.transform.parent.GetComponent<MeshRenderer>().sharedMaterial == _gameManager.GreenMat)
        {
            _gameManager.Money += _gameManager.Income * 4;
            _gameManager.MoneyText.text = _gameManager.Money.ToString();
            _incomeNumbersTexts[_incomeNumbersIndex].text = (_gameManager.Income * 4).ToString() + "$";
        }
        else if (gameObject.transform.parent.GetComponent<MeshRenderer>().sharedMaterial == _gameManager.OrangeMat)
        {
            _gameManager.Money += _gameManager.Income * 15;
            _gameManager.MoneyText.text = _gameManager.Money.ToString();
            _incomeNumbersTexts[_incomeNumbersIndex].text = (_gameManager.Income * 15).ToString() + "$";
        }
    }
    void DominoContactText(Vector3 textPos)
    {
        _firstTime = true;
        for (int i = 0; i < _dominoManager.ActiveDominos.Count; i++)
        {
            if (_dominoManager.ActiveDominos[i] == transform.parent.gameObject)
            {
                _incomeNumbersIndex = i;
                break;
            }
        }
        _incomeNumbersTexts[_incomeNumbersIndex].transform.position = transform.position + textPos;
        _incomeNumbersTexts[_incomeNumbersIndex].transform.DOMoveY(_incomeNumbersTexts[_incomeNumbersIndex].transform.position.y + 5f, .5f).OnComplete(() =>
        {
            _incomeNumbersTexts[_incomeNumbersIndex].DOBlendableColor(Color.black, .2f).OnComplete(() =>
            {
                _incomeNumbersTexts[_incomeNumbersIndex].transform.position = new Vector3(_incomeNumbersTexts[_incomeNumbersIndex].transform.position.x, _incomeNumbersTexts[_incomeNumbersIndex].transform.position.y - 10f, _incomeNumbersTexts[_incomeNumbersIndex].transform.position.z);
                _incomeNumbersTexts[_incomeNumbersIndex].color = Color.green;
            });
        });
    }
}