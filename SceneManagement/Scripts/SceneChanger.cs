using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SceneManagement.Scripts
{
    public static class SceneChanger
    {
        // <------- public events ------->
        public static event Action StartLoading;
        public static event Action FinishedLoading;
        public static event Action<float> UpdateProgress;

        // Shorthand for getting the current scene
        public static Scene CurrentScene => SceneManager.GetActiveScene();
    
        /// <summary>
        /// Change the currently loaded scene
        /// </summary>
        public static void ChangeScene (string name)
        {
            DoChangeScene(name).Run();
        }

        private static float GetOperationListProgress(float currentOperationProgress, float currentOperationIndex,
            float totalOperations)
        {
            return Mathf.Lerp(currentOperationIndex / totalOperations, (currentOperationIndex + 1) / totalOperations,
                currentOperationProgress);
        }

        private static IEnumerator UnloadOperations(List<ISceneUnloadAsyncOperation> operations, int numOperations)
        {
            var currentOperation = 0f;
            // loop through all the unload operations
            foreach (var unloadOperation in operations)
            {
                // run the operation and don't yield it 
                unloadOperation.Operation().Run();

                // get the progress of the unload operation
                var progress = unloadOperation.GetProgress();
                // while the progress isn't done
                while (progress < 1)
                {
                    // send an event telling anything that's listening how far through the unload process we are
                    // divide by two because unloading is the first part of the sequence - we want a number between 0 and 0.5
                    UpdateProgress?.Invoke(GetOperationListProgress(progress, currentOperation, numOperations) / 2);
                    // update the progress
                    progress = unloadOperation.GetProgress();
                    yield return null;
                }
            
                // we're done with this operation, onto the next one!
                ++currentOperation;
            }
        }
    
        private static IEnumerator LoadOperations(List<ISceneLoadAsyncOperation> operations, int numOperations)
        {
        
            var currentOperation = 0f;
            // loop through all the unload operations
            foreach (var unloadOperation in operations)
            {
                // run the operation and don't yield it 
                unloadOperation.Operation().Run();

                // get the progress of the unload operation
                var progress = unloadOperation.GetProgress();
                // while the progress isn't done
                while (progress < 1)
                {
                    // send an event telling anything that's listening how far through the unload process we are
                    // divide by two, add 0.5 because unloading is the first part of the sequence - we want a number between 0 and 0.5
                    UpdateProgress?.Invoke(GetOperationListProgress(progress, currentOperation, numOperations) / 2 + 0.5f);
                    // update the progress
                    progress = unloadOperation.GetProgress();
                    yield return null;
                }
            
                // we're done with this operation, onto the next one!
                ++currentOperation;
            }
        }
    
        private static IEnumerator DoChangeScene (string name)
        {
            StartLoading?.Invoke();

            // grab the current scene, before it gets unloaded
            var unloadScene = CurrentScene;

            // find all the cleanup operations
            var unloadOperations = unloadScene.FindInterfaces<ISceneUnloadAsyncOperation>();
            // cache the number of unload operations
            var numOperations = unloadOperations.Count + 1;
            yield return UnloadOperations(unloadOperations, numOperations);
        
            // unload the scene
            var unload = SceneManager.UnloadSceneAsync(CurrentScene);
        
            while (!unload.isDone)
            {
                // send an event telling anything that's listening how far through the unload process we are
                // divide by two because unloading is the first part of the sequence - we want a number between 0 and 0.5
                UpdateProgress?.Invoke(GetOperationListProgress(unload.progress, unloadOperations.Count, numOperations) / 2);
                yield return null;
            }

            // load the scene
            var load = SceneManager.LoadSceneAsync (name);
        
            while (!load.isDone)
            {
                // send an event telling anything that's listening how far through the unload process we are
                // divide by two, add 0.5 because unloading is the first part of the sequence - we want a number between 0.5 and 1
                UpdateProgress?.Invoke(GetOperationListProgress(load.progress, 0, 5) / 2 + 0.5f);
                yield return null;
            }

            var loadedScene = CurrentScene;
            // find all the cleanup operations
            var loadOperations = loadedScene.FindInterfaces<ISceneLoadAsyncOperation>();
            // cache the number of unload operations
            numOperations = loadOperations.Count + 1;
            yield return LoadOperations(loadOperations, numOperations);
            FinishedLoading?.Invoke();
        }
    }
}