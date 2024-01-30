using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _changeSceneButton;
    [SerializeField] private int _targetSceneIndex;
    [Space]
    [SerializeField] private Button _exitgameButton;

    private void Start()
    {
        _changeSceneButton.onClick.AddListener(StartGameScene);
        _exitgameButton.onClick.AddListener(ExitGame);
    }

    private void StartGameScene() =>
            SceneManager.LoadScene(_targetSceneIndex);

    private void ExitGame() => 
        Application.Quit();
}