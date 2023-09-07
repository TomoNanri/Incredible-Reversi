using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DiscColor { Black, White}
public enum GameState { Intro, InGame, GameOver}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }
    public DiscColor PlayerColor => _playerColor;
    [SerializeField]
    private DiscColor _playerColor = DiscColor.Black;
    [SerializeField]
    private GameState _gameState;

    // Start is called before the first frame update
    void Start()
    {
        _playerColor = DiscColor.Black;
        _gameState = GameState.Intro;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
