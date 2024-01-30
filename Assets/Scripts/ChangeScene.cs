using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Button _changeSceneButton;
    [SerializeField] private int _sceneIndex;

    private void Awake() => 
        _changeSceneButton.onClick.AddListener(ChangeCurrentScene);

    private void ChangeCurrentScene() => 
        SceneManager.LoadScene(_sceneIndex);
}
