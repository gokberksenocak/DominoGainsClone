using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DominoManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _fallPanel;
    [SerializeField] private GameObject _getUpPanel;
    [SerializeField] private List<GameObject> _dominos;
    [SerializeField] private List<GameObject> _activeDominos;
    [SerializeField] private Vector3[] _firstPos;
    [SerializeField] private Quaternion[] _firstRot;
    private Rigidbody _rb;
    private int _index;

    public Quaternion[] DominosFirstRot 
    {
        get {return _firstRot; }
        set {_firstRot=value; }
    }
    public int Index 
    {
        get {return _index; }
        set {_index=value; }
    }
    public List<GameObject> Dominos 
    {
        get {return _dominos; }
        set {_dominos=value; }
    }
    public List<GameObject> ActiveDominos
    {
        get { return _activeDominos; }
        set { _activeDominos = value; }
    }
    public GameObject FallPanel 
    {
        get {return _fallPanel; }
        set {_fallPanel=value; }
    }
    void Start()
    {
        for (int i = 0; i < _dominos.Count; i++)
        {
            if (_dominos[i].activeSelf)
            {
                _activeDominos.Add(_dominos[i]);
            }
        }
        for (int i = 0; i < _dominos.Count; i++)
        {
            _firstPos[i]= _dominos[i].transform.position;
            _firstRot[i] = _dominos[i].transform.rotation;
        }
        InvokeRepeating(nameof(FallIsOver), .2f, .15f);
        _rb = GetComponent<Rigidbody>();
        _index = _activeDominos.Count - 1;
    }
    public void Fall()
    {
        for (int i = 0; i < _activeDominos.Count; i++)
        {
            _activeDominos[i].GetComponent<Rigidbody>().isKinematic = false;
            _activeDominos[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        _rb.constraints = RigidbodyConstraints.None;
        _rb.AddTorque(Vector3.back * 80f, ForceMode.Force);
        //_rb.AddForce(Vector3.right * 100f, ForceMode.Force);
        _getUpPanel.SetActive(false);
        _fallPanel.SetActive(false);
        for (int i = 0; i < _gameManager.Buttons.Length; i++)
        {
            _gameManager.Buttons[i].interactable = false;
        }
        
    }
    void FallIsOver()
    {
        if (_activeDominos[^1].transform.GetChild(0).GetComponent<HitCheck>().Hitted)
        {
            _getUpPanel.SetActive(true);
            _fallPanel.SetActive(false);
            _activeDominos[^1].transform.GetChild(0).GetComponent<HitCheck>().Hitted = false;
        }
    }
    public void GetUp()
    {
        if (_index>0)
        {
            _activeDominos[_index].GetComponent<Rigidbody>().isKinematic = true;
            _activeDominos[_index].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            _activeDominos[_index].transform.DOMove(_firstPos[_index], .5f).SetEase(Ease.Linear);
            _activeDominos[_index].transform.DORotateQuaternion(_firstRot[_index], .5f).SetEase(Ease.Linear);
            _index--;
        }
        else if (_index==0)
        {
            _activeDominos[_index].GetComponent<Rigidbody>().isKinematic = true;
            _activeDominos[_index].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            _activeDominos[_index].transform.DOMove(_firstPos[_index], 1f).SetEase(Ease.Linear);
            _activeDominos[_index].transform.DORotateQuaternion(_firstRot[_index], .5f).SetEase(Ease.Linear).OnComplete(()=> 
            {
                _fallPanel.SetActive(true);
                _getUpPanel.SetActive(false);
                _index = _activeDominos.Count - 1;
                for (int i = 0; i < _gameManager.Buttons.Length; i++)
                {
                    _gameManager.Buttons[i].interactable = true;
                }
                _gameManager.BuyCheck();
                for (int i = 0; i < _activeDominos.Count; i++)
                {
                    _activeDominos[i].transform.GetChild(0).GetComponent<HitCheck>().FirstTime = false;
                }
                _gameManager.MergeButtonLock();
            });
        }
    }
}