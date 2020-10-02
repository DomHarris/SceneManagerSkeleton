using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Get all interfaces in the given scene
        /// </summary>
        public static List<T> FindInterfaces<T>(this Scene scene)
        {
            var interfaces = new List<T>();
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                var childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                    interfaces.Add(childInterface);
            }

            return interfaces;
        }
    }
}