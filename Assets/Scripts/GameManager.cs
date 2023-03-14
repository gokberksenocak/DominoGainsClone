using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DominoManager _dominoManager;
    [SerializeField] private ButtonManager[] _buttonManagers;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private ParticleSystem[] _particles;
    [SerializeField] private Transform[] _mergePoints;
    [SerializeField] private Transform _mergePointParent;
    [SerializeField] private Transform _dominosParent;
    [SerializeField] private List<GameObject> _roads;
    [SerializeField] private List<Vector3> _dominosFirstPos;
    [SerializeField] private List<int> _first3MergeIndex;
    [SerializeField] private TextMeshProUGUI _textMoney;
    [SerializeField] private Material _green;
    [SerializeField] private Material _blue;
    [SerializeField] private Material _orange;
    private Material _mergeMaterial;
    private float _income = 1;
    private float _money = 10000;
    private int _mergeIndex;
    private int _colorCount;
    private bool _isFound;
    public float Income 
    {
        get {return _income; }
        set {_income=value; }
    }
    public float Money
    {
        get { return _money; }
        set { _money = value; }
    }
    public TextMeshProUGUI MoneyText
    {
        get { return _textMoney; }
        set { _textMoney = value; }
    }
    public Button[] Buttons 
    {
        get {return _buttons; }
        set {_buttons=value; }
    }
    public Material GreenMat 
    {
        get {return _green; }
        set {_green=value; }
    }
    public Material BlueMat
    {
        get { return _blue; }
        set { _blue = value; }
    }
    public Material OrangeMat
    {
        get { return _orange; }
        set { _orange = value; }
    }
    void Start()
    {
        BuyCheck();
        MergeButtonLock();
    }

    public void AddDomino()
    {
        for (int i = 0; i < _dominoManager.Dominos.Count; i++)
        {
            if (!_dominoManager.Dominos[i].activeSelf)
            {
                _dominoManager.Dominos[i].SetActive(true);
                _dominoManager.ActiveDominos.Add(_dominoManager.Dominos[i]);
                break;
            }
        }
        _dominoManager.Index++;
        ButtonsPriceAndLevelUpdates(0);
        BuyCheck();
        MergeButtonLock();
    }
    public void AddRoad()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            if (!_roads[i].activeSelf)
            {
                _roads[i].SetActive(true);
                break;
            }
        }
        ButtonsPriceAndLevelUpdates(1);
        BuyCheck();
        CameraCheck();
    }
    public void IncomeIncrease()
    {
        _income += 1f;
        ButtonsPriceAndLevelUpdates(2);
        BuyCheck();
    }
    public void MergeButtonClick()
    {
        for (int i = 0; i < _dominosFirstPos.Count; i++)
        {
            _dominosFirstPos[i] = _dominoManager.Dominos[i].transform.position;
        }
        Merging();
        ButtonManager buttonManager = _buttons[3].GetComponent<ButtonManager>();
        _money -= buttonManager.Price;
        buttonManager.Price += 50;
        buttonManager.Level++;
        _buttons[3].transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = buttonManager.Price.ToString();
        _textMoney.text = _money.ToString();
        BuyCheck();
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = false;
        }
    }
    void Merging()
    {
        _dominoManager.FallPanel.SetActive(false);
        _isFound = false;
        for (int i = 0; i < _dominoManager.ActiveDominos.Count; i++)
        {
            if (_dominoManager.ActiveDominos[i].GetComponent<MeshRenderer>().sharedMaterial == _blue)
            {
                _mergeIndex = i;
                _first3MergeIndex.Add(_mergeIndex);
                if (_first3MergeIndex.Count==3)
                {
                    _mergeIndex = _first3MergeIndex[0];
                    _mergeMaterial = _green;
                    _isFound = true;
                    break;
                }
            }
        }
        if (!_isFound)
        {
            _first3MergeIndex.Clear();
            for (int i = 0; i < _dominoManager.ActiveDominos.Count; i++)
            {
                if (_dominoManager.ActiveDominos[i].GetComponent<MeshRenderer>().sharedMaterial == _green)
                {
                    _mergeIndex = i;
                    _first3MergeIndex.Add(_mergeIndex);
                    if (_first3MergeIndex.Count == 3)
                    {
                        _mergeIndex = _first3MergeIndex[0];
                        _mergeMaterial = _orange;
                        _isFound = true;
                        break;
                    }
                }
            }
        }
        Vector3 startPos = _dominoManager.ActiveDominos[_mergeIndex].transform.position;
        _dominoManager.ActiveDominos[_mergeIndex].GetComponent<BoxCollider>().enabled = false;
        _dominoManager.ActiveDominos[_first3MergeIndex[1]].GetComponent<BoxCollider>().enabled = false;
        _dominoManager.ActiveDominos[_first3MergeIndex[2]].GetComponent<BoxCollider>().enabled = false;
        _dominoManager.ActiveDominos[_mergeIndex].GetComponent<Rigidbody>().useGravity = false;
        _dominoManager.ActiveDominos[_first3MergeIndex[1]].GetComponent<Rigidbody>().useGravity = false;
        _dominoManager.ActiveDominos[_first3MergeIndex[2]].GetComponent<Rigidbody>().useGravity = false;
        _dominoManager.ActiveDominos[_mergeIndex].transform.DOMove(_mergePoints[0].position, 1f);
        _dominoManager.ActiveDominos[_mergeIndex].transform.DORotateQuaternion(_mergePoints[0].rotation, 1f);
        _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.DOMove(_mergePoints[1].position, 1f);
        _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.DORotateQuaternion(_mergePoints[1].rotation, 1f);
        _dominoManager.ActiveDominos[_first3MergeIndex[2]].transform.DORotateQuaternion(_mergePoints[2].rotation, 1f);
        _dominoManager.ActiveDominos[_first3MergeIndex[2]].transform.DOMove(_mergePoints[2].position, 1f).OnComplete(() =>
        {
            _dominoManager.ActiveDominos[_mergeIndex].transform.SetParent(_mergePointParent);
            _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.SetParent(_mergePointParent);
            _dominoManager.ActiveDominos[_first3MergeIndex[2]].transform.SetParent(_mergePointParent);
            _particles[0].Play();
            _particles[1].Play();
            _particles[2].Play();
            _mergePointParent.transform.DORotate(new Vector3(0f, 360f, 0f), .5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(3).OnComplete(() =>
            {
                _dominoManager.ActiveDominos[_mergeIndex].transform.SetParent(_dominosParent);
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.SetParent(_dominosParent);
                _dominoManager.ActiveDominos[_first3MergeIndex[2]].transform.SetParent(_dominosParent);
                _dominoManager.ActiveDominos[_mergeIndex].GetComponent<MeshRenderer>().sharedMaterial = _mergeMaterial;
                _particles[3].Play();
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].SetActive(false);
                _dominoManager.ActiveDominos[_first3MergeIndex[2]].SetActive(false);
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].GetComponent<BoxCollider>().enabled = true;
                _dominoManager.ActiveDominos[_first3MergeIndex[2]].GetComponent<BoxCollider>().enabled = true;
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].GetComponent<Rigidbody>().useGravity = true;
                _dominoManager.ActiveDominos[_first3MergeIndex[2]].GetComponent<Rigidbody>().useGravity = true;
                _dominoManager.ActiveDominos[_mergeIndex].transform.DORotateQuaternion(Quaternion.identity, 1f);
                _dominoManager.ActiveDominos[_mergeIndex].transform.DOMove(startPos, 1f).OnComplete(() =>
                {
                    _dominoManager.ActiveDominos[_mergeIndex].GetComponent<BoxCollider>().enabled = true;
                    _dominoManager.ActiveDominos[_mergeIndex].GetComponent<Rigidbody>().useGravity = true;
                });
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.position += Vector3.up * .1f;
                _dominoManager.ActiveDominos[_first3MergeIndex[1]].transform.rotation = Quaternion.identity;
                _dominoManager.ActiveDominos.Remove(_dominoManager.ActiveDominos[_first3MergeIndex[1]]);
                _dominoManager.Dominos.Remove(_dominoManager.Dominos[_first3MergeIndex[1]]);
                _dominoManager.ActiveDominos[_first3MergeIndex[2] - 1].transform.position += Vector3.up * .1f;
                _dominoManager.ActiveDominos[_first3MergeIndex[2] - 1].transform.rotation = Quaternion.identity;
                _dominoManager.ActiveDominos.Remove(_dominoManager.ActiveDominos[_first3MergeIndex[2] - 1]);
                _dominoManager.Dominos.Remove(_dominoManager.Dominos[_first3MergeIndex[2] - 1]);
                _dominoManager.Dominos.Add(_dominosParent.GetChild(_dominosParent.childCount - 2).gameObject);
                _dominoManager.Dominos.Add(_dominosParent.GetChild(_dominosParent.childCount - 1).gameObject);
                _dominoManager.Index = _dominoManager.ActiveDominos.Count - 1;
                _first3MergeIndex.Clear();
            });
        });
        Invoke(nameof(AfterMergeNewPos_RotDelay), 3.8f);
    }
    void AfterMergeNewPos_RotDelay()
    {
        for (int i = 0; i < _dominoManager.Dominos.Count; i++)
        {
            _dominoManager.Dominos[i].transform.DOMove(_dominosFirstPos[i], 1f);
            _dominoManager.Dominos[i].transform.DORotateQuaternion(_dominoManager.DominosFirstRot[i], 1f);
            _dominoManager.FallPanel.SetActive(true);
        }
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = true;
        }
        MergeButtonLock();
    }
    public void BuyCheck()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_money < _buttonManagers[i].Price)
            {
                _buttons[i].interactable = false;
            }
        }
    }
    public void MergeButtonLock()
    {
        for (int i = 0; i < _dominoManager.ActiveDominos.Count; i++)
        {
            if (_dominoManager.ActiveDominos[i].GetComponent<MeshRenderer>().sharedMaterial==_blue)
            {
                _colorCount++;
            }
        }
        
        if (_colorCount>=3)
        {
            _buttons[3].interactable = true;
        }
        else
        {
            _colorCount = 0;
            for (int i = 0; i < _dominoManager.ActiveDominos.Count; i++)
            {
                if (_dominoManager.ActiveDominos[i].GetComponent<MeshRenderer>().sharedMaterial == _green)
                {
                    _colorCount++;
                }
            }

            if (_colorCount >= 3)
            {
                _buttons[3].interactable = true;
            }
            else
            {
                _buttons[3].interactable = false;
            }
        }
        _colorCount = 0;
    }
    void ButtonsPriceAndLevelUpdates(int buttonIndex)
    {
        ButtonManager buttonManager = _buttons[buttonIndex].GetComponent<ButtonManager>();
        _money -= buttonManager.Price;
        buttonManager.Price += 50;
        buttonManager.Level++;
        _buttons[buttonIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LEVEL " + buttonManager.Level.ToString();
        _buttons[buttonIndex].transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = buttonManager.Price.ToString();
        _textMoney.text = _money.ToString();
    }
    void CameraCheck()
    {
        if (_roads[3].activeSelf)
        {
            Camera.main.fieldOfView = 70;
        }
    }
}