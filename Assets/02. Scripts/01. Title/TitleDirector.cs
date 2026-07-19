using UnityEngine;
using UnityEngine.UI;

public class TitleDirector : MonoBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _quitBtn;
    [SerializeField] private Button _optionBtn;

    private void Awake()
    {
        _startBtn.onClick.AddListener(StartGame);
        _quitBtn.onClick.AddListener(QuitGame);
        _optionBtn.onClick.AddListener(Option);
    }

    private void StartGame()
    {
        App.Get<SceneManager>().Load(SceneName.Game);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Option()
    {
        
    }
}
