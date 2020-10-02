using System.Collections;

namespace SceneManagement.Scripts
{
    /// <summary>
    /// Interface for tasks that need to happen behind a loading screen when the scene loads
    /// </summary>
    public interface ISceneLoadAsyncOperation
    {
        IEnumerator Operation();
        float GetProgress();
    }
}