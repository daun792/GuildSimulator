using UnityEngine;

public class DeveloperMaanger : MonoBehaviour
{
    private void Start()
    {
        App.Get<SceneManager>().Load(SceneName.Title);
    }
}
