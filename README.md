# SceneManagerSkeleton
A little scene management starter. **Make sure you add LoadingScreen to your build settings**

To add tasks to the loading screen, make sure your class inherits from MonoBehaviour and implements either `ISceneLoadAsyncOperation` or `ISceneUnloadAsyncOperation`. An example would be: 
```css
public class ExampleLoadOperation : MonoBehaviour, ISceneLoadAsyncOperation
{
    private float _timeToComplete = 2f;
    private float _currentTime = 0f;

    // the actual operation to complete
    public IEnumerator Operation()
    {
        while (_currentTime < _timeToComplete)
        {
            _currentTime += Time.deltaTime;
            yield return null;
        }
    }
    
    // the current progress of the operation
    public float GetProgress()
    {
        return _currentTime / _timeToComplete;
    }
}
```
This is totally optional, though - it will still show a progress bar even if no load/unload operations are present.

Load a new scene with `SceneChanger.ChangeScene ("SceneName");` - everything else will be handled.

Licensed with the Unlicence.
