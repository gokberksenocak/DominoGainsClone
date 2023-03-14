using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private DominoManager _dominoManager;
    public void Falling()
    {
        _dominoManager.Fall();
    }

    public void GettingUp()
    {
        _dominoManager.GetUp();
    }

}
