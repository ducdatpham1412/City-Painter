using UnityEngine.SceneManagement;

public class Navigator : Singleton<Navigator> {
    public enum Scene {
        GameScene,
        CitiesScene,
    }
    public string currentScene;

    public void NavigateTo(Scene scene, LoadSceneMode mode = LoadSceneMode.Single) {
        string name = scene.ToString();
        SceneManager.LoadScene(name, mode);
        currentScene = name;
    }

    public void UnloadSceneAsync(Scene scene) {
        SceneManager.UnloadSceneAsync(scene.ToString());
        currentScene = SceneManager.GetActiveScene().name;
    }
}
