using UnityEngine;
using UnityEngine.SceneManagement;
using SceneTransitionFXProFREE;

namespace SceneTransitionFXProFREE
{
    public class SceneSwitcher : MonoBehaviour
    {
        public TransitionManager transitionManager;  // Reference to Transition Manager
        public string targetScene = "DemoSceneB_FREE";    // Target scene to transition to


        private void Start()
        {
            if (transitionManager != null)
            {
                MoveToSceneB();
            }
        }


        public void MoveToSceneB()
        {
            if (transitionManager != null)
            {
                // Set the target scene
                transitionManager.toScene = targetScene;

                // Assign transition effects for leaving and entering
                transitionManager.leavingFX = TransitionManager.LeavingTransitionEffect.DissolveOut;
                transitionManager.enteringFX = TransitionManager.EnteringTransitionEffect.DissolveIn;

                // Optionally, set effect-specific parameters if needed
                transitionManager.dissolveOutDuration = 2.5f;  // Adjusting duration
                transitionManager.dissolveInDuration = 2.5f;   // Adjusting duration

                // Start the transition
                transitionManager.StartTransition();
            }
            else
            {
                Debug.LogWarning("Transition Manager reference is missing.");
            }
        }
    }
}
