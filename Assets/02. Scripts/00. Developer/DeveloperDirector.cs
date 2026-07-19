using UnityEngine;

public class DeveloperDirector : MonoBehaviour
{
    private void Start()
    {
        App.Get<SceneManager>().Load(SceneName.Title);
    }
}
