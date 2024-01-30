using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] private int _targetSceneIndex;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(_targetSceneIndex);
    }    
}