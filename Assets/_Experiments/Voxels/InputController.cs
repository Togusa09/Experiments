using Experimental.Voxel;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    Player _player;

    [SerializeField]
    MapGenPanel _mapGenPanel;

    // Start is called before the first frame update
    void Start()
    {
        _player.PlayerState = PlayerState.ControlDisabled;

        _mapGenPanel.OnMapGenerated += HideMenu;
    }

    public void ShowMenu()
    {
        _mapGenPanel.gameObject.SetActive(true);
        _player.PlayerState = PlayerState.ControlDisabled;
    }

    public void HideMenu()
    {
        _mapGenPanel.gameObject.SetActive(false);
        _player.PlayerState = PlayerState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowMenu();
        }
                


        //if (Input.GetKey(KeyCode.W))
        //{
        //    _player.OnKey(KeyCode.W);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    _player.OnKey(KeyCode.S);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    _player.OnKey(KeyCode.A);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    _player.OnKey(KeyCode.D);
        //}
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    _player.OnKey(KeyCode.Space);
        //}
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    _player.OnKey(KeyCode.LeftShift);
        //}
    }
}
