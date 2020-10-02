using System;
using SceneManagement.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public enum GameState
    {
        Game,
        Loading,
        Paused
    }
    public static class GameManager
    {
        // Keep track of the current game state
        public static GameState State;
        
        /// <summary>
        ///When the game starts, initialise the loading scene 
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void InitLoadingScene()
        {
            SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        }

        /// <summary>
        /// When the game starts, initialise the current state
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void InitState()
        {
            State = GameState.Game;
        }

        /// <summary>
        /// When the game starts, subscribe to the scene load/unload events
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void SubscribeToSceneEvents()
        {
            SceneChanger.StartLoading += SceneChangerOnStartLoading;
            SceneChanger.FinishedLoading += SceneChangerOnFinishedLoading;
        }

        /// <summary>
        /// When the scene has finished loading, set the timescale and current game state
        /// </summary>
        private static void SceneChangerOnFinishedLoading()
        {
            Time.timeScale = 1;
            State = GameState.Game;
        }

        
        /// <summary>
        /// When the scene starts loading, set the timescale and current game state
        /// </summary>
        private static void SceneChangerOnStartLoading()
        {
            Time.timeScale = 1;
            State = GameState.Loading;
        }

        /// <summary>
        /// Pause/Unpause the game. If the game is already paused, unpauses the game.
        /// If the game is loading a scene, returns false.
        /// </summary>
        public static bool Pause()
        {
            switch (State)
            {
                case GameState.Loading:
                    return false;
                case GameState.Game:
                    Time.timeScale = 0;
                    State = GameState.Game;
                    return true;
                case GameState.Paused:
                    Time.timeScale = 1;
                    State = GameState.Paused;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}