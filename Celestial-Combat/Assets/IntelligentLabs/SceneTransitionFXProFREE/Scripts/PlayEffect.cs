using UnityEngine;

namespace SceneTransitionFXProFREE
{
    public class PlayEffect : MonoBehaviour
    {
        private const string ultimateEditionUrl = "https://assetstore.unity.com/packages/tools/visual-scripting/scene-transition-fx-pro-ultimate-edition-297299";

        public TransitionManager transitionManager;  // Assign in Inspector

        public void OnButtonClick()
        {
            if (transitionManager != null)
            {
                if (IsUltimateEffectSelected())
                {
                    // Visit the Ultimate version at Unity Asset Store
                    Application.OpenURL(ultimateEditionUrl);
                }
                else
                {
                    // Start Free Transitions
                    Debug.Log("Starting transition to scene: " + transitionManager.toScene);
                    transitionManager.StartTransition();  // Start the transition
                }
            }
            else
            {
                Debug.LogError("TransitionManager is not assigned in PlayEffect.");
            }
        }

        private bool IsUltimateEffectSelected()
        {
            // Check if either the selected Entering or Leaving FX is an Ultimate effect
            return GetEffectDisplayName(transitionManager.leavingFX).Contains("(Get Ultimate)") ||
                   GetEffectDisplayName(transitionManager.enteringFX).Contains("(Get Ultimate)");
        }

        private string GetEffectDisplayName(TransitionManager.LeavingTransitionEffect effect)
        {
            switch (effect)
            {
                case TransitionManager.LeavingTransitionEffect.CardFlipOut:
                case TransitionManager.LeavingTransitionEffect.FadeAttackOut:
                case TransitionManager.LeavingTransitionEffect.FadeOut:
                case TransitionManager.LeavingTransitionEffect.MultiverseOut:
                case TransitionManager.LeavingTransitionEffect.Omni360ProOut:
                case TransitionManager.LeavingTransitionEffect.OmniArrowOut:
                case TransitionManager.LeavingTransitionEffect.PulseChargeOut:
                case TransitionManager.LeavingTransitionEffect.Slide3DShadowOut:
                case TransitionManager.LeavingTransitionEffect.SlideOut:
                case TransitionManager.LeavingTransitionEffect.SpinAttackOut:
                case TransitionManager.LeavingTransitionEffect.VortexSpinOut:
                case TransitionManager.LeavingTransitionEffect.WaveAuraOut:
                    return $"{effect} (Get Ultimate)";
                default:
                    return effect.ToString();
            }
        }

        private string GetEffectDisplayName(TransitionManager.EnteringTransitionEffect effect)
        {
            switch (effect)
            {
                case TransitionManager.EnteringTransitionEffect.CardFlipIn:
                case TransitionManager.EnteringTransitionEffect.FadeAttackIn:
                case TransitionManager.EnteringTransitionEffect.FadeIn:
                case TransitionManager.EnteringTransitionEffect.MultiverseIn:
                case TransitionManager.EnteringTransitionEffect.Omni360ProIn:
                case TransitionManager.EnteringTransitionEffect.OmniArrowIn:
                case TransitionManager.EnteringTransitionEffect.PulseChargeIn:
                case TransitionManager.EnteringTransitionEffect.Slide3DShadowIn:
                case TransitionManager.EnteringTransitionEffect.SlideIn:
                case TransitionManager.EnteringTransitionEffect.SpinAttackIn:
                case TransitionManager.EnteringTransitionEffect.VortexSpinIn:
                case TransitionManager.EnteringTransitionEffect.WaveAuraIn:
                    return $"{effect} (Get Ultimate)";
                default:
                    return effect.ToString();
            }
        }
    }
}
