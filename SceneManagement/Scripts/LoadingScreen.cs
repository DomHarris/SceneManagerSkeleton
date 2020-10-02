using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace SceneManagement.Scripts
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
        // we only ever want one of these loading screens
        private static bool _hasLoadingScreen = false;
    
        // a UGUI Image to use as the progress bar
        [SerializeField] private Image progress;
        
        // a Canvas Group that fades the whole loading screen in/out
        private CanvasGroup _canvas;

        
        /// <summary>
        /// When the screen is first loaded, before Start/OnEnable
        /// </summary>
        private void Awake()
        {
            // if there's already a loading screen, warn the user and unload the new loading screen scene
            if (_hasLoadingScreen)
            {
                Debug.LogWarning("You've already got a loading screen, why are you adding another?");
                SceneManager.UnloadSceneAsync("LoadingScreen");
                return;
            }
            // grab the attached canvas group
            _canvas = GetComponent<CanvasGroup>();
            
            // hide the loading screen
            _canvas.alpha = 0;
            _canvas.blocksRaycasts = false;
        
            // don't ever destroy this loading screen object
            DontDestroyOnLoad(gameObject);
            // and the corresponding event system
            DontDestroyOnLoad(FindObjectOfType<EventSystem>().gameObject);
            
            // Unload the Loading Screen scene, we're done with it
            SceneManager.UnloadSceneAsync("LoadingScreen");
        }

        /// <summary>
        /// When the object is enabled, subscribe to the necessary events
        /// </summary>
        private void OnEnable()
        {
            SceneChanger.StartLoading += Show;
            SceneChanger.FinishedLoading += Hide;
            SceneChanger.UpdateProgress += SceneChangerOnUpdateProgress;
        }

        /// <summary>
        /// When the object is disabled, unsubscribe from the necessary events
        /// </summary>
        private void OnDisable()
        {
            SceneChanger.StartLoading -= Show;
            SceneChanger.FinishedLoading -= Hide;
            SceneChanger.UpdateProgress -= SceneChangerOnUpdateProgress;
        }

        /// <summary>
        /// Update the progress bar image
        /// </summary>
        private void SceneChangerOnUpdateProgress(float currentProgress)
        {
            progress.fillAmount = currentProgress;
        }

        /// <summary>
        /// Fade the whole loading screen in
        /// </summary>
        private void Show()
        {
            Fade(1, TimeSpan.FromSeconds(0.33f)).Run();
        }

        /// <summary>
        /// Fade the whole loading Screen out
        /// </summary>
        private void Hide()
        {
            Fade(0, TimeSpan.FromSeconds(0.33f)).Run();
        }

        
        /// <summary>
        /// Smoothly transition between two fade states over a certain time span
        /// </summary>
        private IEnumerator Fade(float to, TimeSpan time)
        {
            // cache the starting alpha
            var from = _canvas.alpha;
            
            var currentTime = 0f;
            
            while (currentTime < time.TotalSeconds)
            {
                currentTime += Time.deltaTime;
                _canvas.alpha = Mathf.Lerp(from, to, currentTime / (float)time.TotalSeconds);
                yield return null;
            }

            _canvas.blocksRaycasts = !_canvas.blocksRaycasts;
        }
    }
}