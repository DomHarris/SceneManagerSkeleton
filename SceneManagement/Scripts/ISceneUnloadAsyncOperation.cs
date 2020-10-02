using System.Collections;

namespace SceneManagement.Scripts
{
    /// <summary>
    /// Interface for cleanup tasks that need to happen behind a loading screen when the scene unloads
    /// </summary>
    public interface ISceneUnloadAsyncOperation
    {
        IEnumerator Operation();
        float GetProgress();
    }
}