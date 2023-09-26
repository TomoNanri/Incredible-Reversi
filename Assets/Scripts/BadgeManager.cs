using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BadgeEventType { PASS, POINT, GAMEOVER}

public class BadgeManager : MonoBehaviour
{
    [SerializeField]
    private float _flashInterval = 0.5f;
    [SerializeField]
    private float _displayTime = 2.0f;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private AIPlayer _aiPlayer;
    private Player _player;
    private GameObject _PanelBeginner;
    private GameObject _PanelFullFledged;
    private GameObject _PanelTUEE;
    private GameObject _PanelNicePass;
    private GameObject _PanelSelfish;
    private GameObject _PanelTUEEE;
    private GameObject _PanelTUEEEE;
    private GameObject _PanelZama;
    private bool _Beginner;
    private bool _FullFledged;
    private bool _TUEE;
    private bool _NicePass;
    //private bool _Selfish;
    private bool _TUEEE;
    //private bool _TUEEEE;
    //private bool _Zama;
    private int _passCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        _PanelBeginner = GameObject.Find("PanelBeginner");
        _PanelBeginner.SetActive(false);
        _PanelFullFledged = GameObject.Find("PanelFull-fledged");
        _PanelFullFledged.SetActive(false);
        _PanelTUEE = GameObject.Find("PanelTUEE");
        _PanelTUEE.SetActive(false);
        _PanelNicePass = GameObject.Find("PanelNicePass");
        _PanelNicePass.SetActive(false);
        _PanelSelfish = GameObject.Find("PanelSelfish");
        _PanelSelfish.SetActive(false);
        _PanelTUEEE = GameObject.Find("PanelTUEEE");
        _PanelTUEEE.SetActive(false);
        _PanelTUEEEE = GameObject.Find("PanelTUEEEE");
        _PanelTUEEEE.SetActive(false);
        _PanelZama = GameObject.Find("PanelZama");
        _PanelZama.SetActive(false);

        _aiPlayer = FindAnyObjectByType<AIPlayer>();
        _aiPlayer.OnBadgeEvent += BadgeEventHandler;
        _gm = FindAnyObjectByType<GameManager>();
        _gm.OnBadgeEvent += BadgeEventHandler;
        _gm.OnInitializeGame += Initialize;
        _gameBoard = FindAnyObjectByType<GameBoard>();
        _player = FindAnyObjectByType<Player>();
        _passCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        _PanelBeginner.SetActive(false);
        _PanelFullFledged.SetActive(false);
        _PanelTUEE.SetActive(false);
        _PanelNicePass.SetActive(false);
        _PanelSelfish.SetActive(false);
        _PanelTUEEE.SetActive(false);
        _PanelTUEEEE.SetActive(false);
        _PanelZama.SetActive(false);
        _Beginner = false;
        _FullFledged = false;
        _TUEE = false;
        _NicePass = false;
        _TUEEE = false;
        //_TUEEEE = false;
        //_Zama = false;
        _passCount = 0;
    }
    private void BadgeEventHandler(BadgeEventArgs args)
    {
        switch (args.EventType)
        {
            case BadgeEventType.PASS:
                _passCount++;

                if (_passCount < 3)
                {
                    if (!_NicePass)
                    {
                        // NicePass‚ð•\Ž¦
                        _NicePass = true;
                        StartCoroutine(ShowBadge(_PanelNicePass, _flashInterval, _displayTime));
                    }
                    break;
                }
                else
                {
                    // Selfish‚ð•\Ž¦
                    StartCoroutine(ShowBadge(_PanelSelfish, _flashInterval, _displayTime));
                }

                break;

            case BadgeEventType.POINT:
                if (!_Beginner)
                {
                    _Beginner = true;
                    StartCoroutine(ShowBadge(_PanelBeginner, _flashInterval, _displayTime));
                }
                if (!_FullFledged && _player.PlayerPoint>2)
                {
                    _FullFledged = true;
                    StartCoroutine(ShowBadge(_PanelFullFledged, _flashInterval, _displayTime));
                }
                if (!_TUEE && _player.PlayerPoint > 4)
                {
                    _TUEE = true;
                    StartCoroutine(ShowBadge(_PanelTUEE, _flashInterval, _displayTime));
                }
                if (!_TUEEE && _player.PlayerPoint > 8)
                {
                    _TUEEE = true;
                    StartCoroutine(ShowBadge(_PanelTUEEE, _flashInterval, _displayTime));
                }
                break;

            case BadgeEventType.GAMEOVER:
                int pointMe = (_gm.PlayerColor == DiscColor.Black) ? _gameBoard.WhiteCount : _gameBoard.BlackCount;
                int pointYou = (_gm.PlayerColor == DiscColor.Black) ? _gameBoard.BlackCount : _gameBoard.WhiteCount;
                if(pointMe == 0)
                {
                    StartCoroutine(ShowBadge(_PanelTUEEEE, _flashInterval, _displayTime));
                }
                if(pointYou == 0)
                {
                    StartCoroutine(ShowBadge(_PanelZama, _flashInterval, _displayTime));
                }
                break;

            default:
                Debug.LogError($"[{this.name}] Invalid Event Name.");
                break;
        }
    }
    IEnumerator ShowBadge(GameObject obj,float sec1, float sec2)
    {
        for(int i=0; i < 2; i++)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(sec1);
            obj.SetActive(false);
            yield return new WaitForSeconds(sec1);
        }
        obj.SetActive(true);
        yield return new WaitForSeconds(sec2);
        obj.SetActive(false);
    }
}
