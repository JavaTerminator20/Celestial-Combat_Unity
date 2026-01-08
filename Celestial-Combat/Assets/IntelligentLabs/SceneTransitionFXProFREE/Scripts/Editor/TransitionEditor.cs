using UnityEditor;
using UnityEngine;
using static SceneTransitionFXProFREE.TransitionManager;
using System.Linq;

namespace SceneTransitionFXProFREE
{
    [CustomEditor(typeof(TransitionManager))]
    public class TransitionEditor : Editor
    {
        // Reference to the Ultimate Edition Store page (free edition only)
        private const string ultimateEditionUrl = "https://assetstore.unity.com/packages/tools/visual-scripting/scene-transition-fx-pro-ultimate-edition-297299";

        // Reference to the Ultimate Edition image (free edition only)
        private Texture2D ultimatePreviewImage;

        // Reference to "TAG YOUR PANEL" image
        private Texture2D tagYourPanelImage;

        string[] sceneNames;
        bool assignDefaultEnteringFX = false;  // Checkbox state

        void OnEnable()
        {
            // Load the TagYourPanel image
            tagYourPanelImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntelligentLabs/SceneTransitionFXProFREE/Sprites/TagYourPanelGreen_FREE.png", typeof(Texture2D));

            // Load the ultimatePreview image
            ultimatePreviewImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntelligentLabs/SceneTransitionFXProFREE/Sprites/UltimatePreviewGRN.png", typeof(Texture2D));

            sceneNames = EditorBuildSettings.scenes
                .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            TransitionManager manager = (TransitionManager)target;

            EditorGUI.BeginChangeCheck();

            // Display current scene as static text
            EditorGUILayout.LabelField("Current Scene", manager.fromScene);

            // To Scene dropdown
            if (sceneNames.Length > 0)
            {
                manager.toScene = SceneDropdown("Target Scene", manager.toScene);
            }
            else
            {
                // Add space and helper info
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Target scene not found in Build Settings. Please add it under 'Scenes in Build' to proceed.", MessageType.Info);
            }

            // Add space
            GUILayout.Space(10);

            // Checkbox for assigning default Entering FX
            assignDefaultEnteringFX = EditorGUILayout.Toggle("* Auto Assign FX *", assignDefaultEnteringFX);

            // Add space
            GUILayout.Space(10);

            // Leaving Scene FX dropdown
            manager.leavingFX = (LeavingTransitionEffect)EditorGUILayout.Popup("Leaving Scene FX",
                (int)manager.leavingFX,
                System.Enum.GetValues(typeof(LeavingTransitionEffect)).Cast<LeavingTransitionEffect>().Select(e => GetEffectDisplayName(e)).ToArray()
            );

            // Assign default Entering FX based on checkbox state
            if (assignDefaultEnteringFX)
            {
                AssignDefaultEnteringFX(manager);
            }

            // Show controls for Leaving FX
            ShowLeavingFXControls(manager);

            // Add space
            GUILayout.Space(10);

            // Entering Scene FX dropdown (allow user control)
            manager.enteringFX = (EnteringTransitionEffect)EditorGUILayout.Popup("Entering Scene FX",
                (int)manager.enteringFX,
                System.Enum.GetValues(typeof(EnteringTransitionEffect)).Cast<EnteringTransitionEffect>().Select(e => GetEffectDisplayName(e)).ToArray()
            );

            // Show specific controls for Entering FX
            ShowEnteringFXControls(manager);

            // Add Helper Info
            GUILayout.Label(tagYourPanelImage, GUILayout.Width(225), GUILayout.Height(95));

            // Display the UltimatePreview image if an Ultimate effect is selected
            if (IsUltimateEffectSelected(manager))
            {
                GUILayout.Label("* Unlock 16 Exclusive Transitions - Click Below *", EditorStyles.boldLabel);
                GUILayout.Space(5);

                if (GUILayout.Button(ultimatePreviewImage, GUILayout.Width(300), GUILayout.Height(205)))
                {
                    Application.OpenURL(ultimateEditionUrl);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(manager);
            }
        }

        private bool IsUltimateEffectSelected(TransitionManager manager)
        {
            // Check if the selected Entering or Leaving FX is an Ultimate effect
            return GetEffectDisplayName(manager.leavingFX).Contains("(Get Ultimate)") ||
                   GetEffectDisplayName(manager.enteringFX).Contains("(Get Ultimate)");
        }

        private void AssignDefaultEnteringFX(TransitionManager manager)
        {
            switch (manager.leavingFX)
            {
                case LeavingTransitionEffect.CardFlipOut:
                    manager.enteringFX = EnteringTransitionEffect.CardFlipIn;
                    break;
                case LeavingTransitionEffect.DissolveOut:
                    manager.enteringFX = EnteringTransitionEffect.DissolveIn;
                    break;
                case LeavingTransitionEffect.FadeAttackOut:
                    manager.enteringFX = EnteringTransitionEffect.FadeAttackIn;
                    break;
                case LeavingTransitionEffect.FadeOut:
                    manager.enteringFX = EnteringTransitionEffect.FadeIn;
                    break;
                case LeavingTransitionEffect.GlitchOut:
                    manager.enteringFX = EnteringTransitionEffect.GlitchIn;
                    break;
                case LeavingTransitionEffect.LeafFallOut:
                    manager.enteringFX = EnteringTransitionEffect.LeafFallIn;
                    break;
                case LeavingTransitionEffect.MultiverseOut:
                    manager.enteringFX = EnteringTransitionEffect.MultiverseIn;
                    break;
                case LeavingTransitionEffect.Omni360ProOut:
                    manager.enteringFX = EnteringTransitionEffect.Omni360ProIn;
                    break;
                case LeavingTransitionEffect.OmniArrowOut:
                    manager.enteringFX = EnteringTransitionEffect.OmniArrowIn;
                    break;
                case LeavingTransitionEffect.PulseChargeOut:
                    manager.enteringFX = EnteringTransitionEffect.PulseChargeIn;
                    break;
                case LeavingTransitionEffect.Slide3DShadowOut:
                    manager.enteringFX = EnteringTransitionEffect.Slide3DShadowIn;
                    break;
                case LeavingTransitionEffect.SlideOut:
                    manager.enteringFX = EnteringTransitionEffect.SlideIn;
                    break;
                case LeavingTransitionEffect.SpinAttackOut:
                    manager.enteringFX = EnteringTransitionEffect.SpinAttackIn;
                    break;
                case LeavingTransitionEffect.VortexSpinOut:
                    manager.enteringFX = EnteringTransitionEffect.VortexSpinIn;
                    break;
                case LeavingTransitionEffect.WaveAuraOut:
                    manager.enteringFX = EnteringTransitionEffect.WaveAuraIn;
                    break;
                case LeavingTransitionEffect.ZoomOut:
                    manager.enteringFX = EnteringTransitionEffect.ZoomIn;
                    break;
            }
        }

        private void ShowEnteringFXControls(TransitionManager manager)
        {
            switch (manager.enteringFX)
            {
                case EnteringTransitionEffect.CardFlipIn:
                    manager.cardFlipDurationIn = EditorGUILayout.FloatField("Duration <- In", manager.cardFlipDurationIn);
                    manager.cardFlipDirectionIn = (FlipDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.cardFlipDirectionIn);
                    break;
                case EnteringTransitionEffect.DissolveIn:
                    manager.dissolveInDuration = EditorGUILayout.FloatField("Duration <- In", manager.dissolveInDuration);
                    break;
                case EnteringTransitionEffect.FadeAttackIn:
                    manager.fadeAttackInDuration = EditorGUILayout.FloatField("Duration <- In", manager.fadeAttackInDuration);
                    break;
                case EnteringTransitionEffect.FadeIn:
                    manager.enteringFadeDuration = EditorGUILayout.FloatField("Duration <- In", manager.enteringFadeDuration);
                    break;
                case EnteringTransitionEffect.GlitchIn:
                    manager.glitchInDuration = EditorGUILayout.FloatField("Duration <- In", manager.glitchInDuration);
                    break;
                case EnteringTransitionEffect.LeafFallIn:
                    manager.leafFallInDuration = EditorGUILayout.FloatField("Duration <- In", manager.leafFallInDuration);
                    manager.leafFallInDirection = (LeafFallDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.leafFallInDirection);
                    break;
                case EnteringTransitionEffect.MultiverseIn:
                    manager.fragmentedBurstDurationIn = EditorGUILayout.FloatField("Burst Duration <- In", manager.fragmentedBurstDurationIn);
                    manager.fragmentCountIn = EditorGUILayout.IntField("Fragment Count <- In", manager.fragmentCountIn);
                    manager.burstSpreadIn = EditorGUILayout.FloatField("Burst Spread <- In", manager.burstSpreadIn);
                    break;
                case EnteringTransitionEffect.Omni360ProIn:
                    manager.omniDurationIn = EditorGUILayout.FloatField("Duration <- In", manager.omniDurationIn);
                    manager.omniDirectionIn = (OmniDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.omniDirectionIn);
                    break;
                case EnteringTransitionEffect.OmniArrowIn:
                    manager.omniDurationIn = EditorGUILayout.FloatField("Duration <- In", manager.omniDurationIn);
                    manager.omniDirectionIn = (OmniDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.omniDirectionIn);
                    break;
                case EnteringTransitionEffect.PulseChargeIn:
                    manager.PulseFrequencyIn = EditorGUILayout.FloatField("Frequency <- In", manager.PulseFrequencyIn);
                    manager.PulseDurationIn = EditorGUILayout.FloatField("Duration <- In", manager.PulseDurationIn);
                    break;
                case EnteringTransitionEffect.Slide3DShadowIn:
                    manager.slide3DRotationDurationIn = EditorGUILayout.FloatField("Duration <- In", manager.slide3DRotationDurationIn);
                    manager.slide3DShadowInDirection = (Slide3DShadowDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.slide3DShadowInDirection);
                    break;
                case EnteringTransitionEffect.SlideIn:
                    manager.slideInDuration = EditorGUILayout.FloatField("Duration <- In", manager.slideInDuration);
                    manager.slideInDirection = (SlideDirection)EditorGUILayout.EnumPopup("Direction <- In", manager.slideInDirection);
                    break;
                case EnteringTransitionEffect.SpinAttackIn:
                    manager.spinAttackInDuration = EditorGUILayout.FloatField("Duration <- In", manager.spinAttackInDuration);
                    manager.speedAccelerationIn = EditorGUILayout.FloatField("Acceleration  <- In", manager.speedAccelerationIn);
                    break;
                case EnteringTransitionEffect.VortexSpinIn:
                    manager.vortexIntensityIn = EditorGUILayout.FloatField("Intensity <- In", manager.vortexIntensityIn);
                    break;
                case EnteringTransitionEffect.WaveAuraIn:
                    manager.waveSlideIn = EditorGUILayout.FloatField("Duration <- In", manager.waveSlideIn);
                    manager.waveOffsetIn = EditorGUILayout.FloatField("Wave Offset <- In", manager.waveOffsetIn);
                    break;
                case EnteringTransitionEffect.ZoomIn:
                    manager.enteringZoomDuration = EditorGUILayout.FloatField("Duration <- In", manager.enteringZoomDuration);
                    break;
            }
        }

        private void ShowLeavingFXControls(TransitionManager manager)
        {
            switch (manager.leavingFX)
            {
                case LeavingTransitionEffect.CardFlipOut:
                    manager.cardFlipDurationOut = EditorGUILayout.FloatField("Duration -> Out", manager.cardFlipDurationOut);
                    manager.cardFlipDirectionOut = (FlipDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.cardFlipDirectionOut);
                    break;
                case LeavingTransitionEffect.DissolveOut:
                    manager.dissolveOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.dissolveOutDuration);
                    break;
                case LeavingTransitionEffect.FadeAttackOut:
                    manager.fadeAttackOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.fadeAttackOutDuration);
                    break;
                case LeavingTransitionEffect.FadeOut:
                    manager.leavingFadeDuration = EditorGUILayout.FloatField("Duration -> Out", manager.leavingFadeDuration);
                    break;
                case LeavingTransitionEffect.GlitchOut:
                    manager.glitchOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.glitchOutDuration);
                    break;
                case LeavingTransitionEffect.LeafFallOut:
                    manager.leafFallOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.leafFallOutDuration);
                    manager.leafFallOutDirection = (LeafFallDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.leafFallOutDirection);
                    break;
                case LeavingTransitionEffect.MultiverseOut:
                    manager.fragmentedBurstDurationOut = EditorGUILayout.FloatField("Burst Duration -> Out", manager.fragmentedBurstDurationOut);
                    manager.fragmentCountOut = EditorGUILayout.IntField("Fragment Count -> Out", manager.fragmentCountOut);
                    manager.burstSpreadOut = EditorGUILayout.FloatField("Burst Spread -> Out", manager.burstSpreadOut);
                    break;
                case LeavingTransitionEffect.Omni360ProOut:
                    manager.omniDurationOut = EditorGUILayout.FloatField("Duration -> Out", manager.omniDurationOut);
                    manager.omniDirectionOut = (OmniDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.omniDirectionOut);
                    break;
                case LeavingTransitionEffect.OmniArrowOut:
                    manager.omniDurationOut = EditorGUILayout.FloatField("Duration -> Out", manager.omniDurationOut);
                    manager.omniDirectionOut = (OmniDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.omniDirectionOut);
                    break;
                case LeavingTransitionEffect.PulseChargeOut:
                    manager.PulseFrequencyOut = EditorGUILayout.FloatField("Frequency -> Out", manager.PulseFrequencyOut);
                    manager.PulseDurationOut = EditorGUILayout.FloatField("Duration -> Out", manager.PulseDurationOut);
                    break;
                case LeavingTransitionEffect.Slide3DShadowOut:
                    manager.slide3DRotationDurationOut = EditorGUILayout.FloatField("Duration -> Out", manager.slide3DRotationDurationOut);
                    manager.slide3DShadowOutDirection = (Slide3DShadowDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.slide3DShadowOutDirection);
                    break;
                case LeavingTransitionEffect.SlideOut:
                    manager.slideOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.slideOutDuration);
                    manager.slideOutDirection = (SlideDirection)EditorGUILayout.EnumPopup("Direction -> Out", manager.slideOutDirection);
                    break;
                case LeavingTransitionEffect.SpinAttackOut:
                    manager.spinAttackOutDuration = EditorGUILayout.FloatField("Duration -> Out", manager.spinAttackOutDuration);
                    manager.speedAccelerationOut = EditorGUILayout.FloatField("Acceleration -> Out", manager.speedAccelerationOut);
                    break;
                case LeavingTransitionEffect.VortexSpinOut:
                    manager.vortexIntensityOut = EditorGUILayout.FloatField("Intensity -> Out", manager.vortexIntensityOut);
                    break;
                case LeavingTransitionEffect.WaveAuraOut:
                    manager.waveSlideOut = EditorGUILayout.FloatField("Duration -> Out", manager.waveSlideOut);
                    manager.waveOffsetOut = EditorGUILayout.FloatField("Wave Offset -> Out", manager.waveOffsetOut);
                    break;
                case LeavingTransitionEffect.ZoomOut:
                    manager.leavingZoomDuration = EditorGUILayout.FloatField("Duration -> Out", manager.leavingZoomDuration);
                    break;
            }
        }

        private string GetEffectDisplayName(LeavingTransitionEffect effect)
        {
            switch (effect)
            {
                case LeavingTransitionEffect.CardFlipOut:
                case LeavingTransitionEffect.FadeAttackOut:
                case LeavingTransitionEffect.FadeOut:
                case LeavingTransitionEffect.MultiverseOut:
                case LeavingTransitionEffect.Omni360ProOut:
                case LeavingTransitionEffect.OmniArrowOut:
                case LeavingTransitionEffect.PulseChargeOut:
                case LeavingTransitionEffect.Slide3DShadowOut:
                case LeavingTransitionEffect.SlideOut:
                case LeavingTransitionEffect.SpinAttackOut:
                case LeavingTransitionEffect.VortexSpinOut:
                case LeavingTransitionEffect.WaveAuraOut:
                    return $"{effect} (Get Ultimate)";
                default:
                    return effect.ToString();
            }
        }

        private string GetEffectDisplayName(EnteringTransitionEffect effect)
        {
            switch (effect)
            {
                case EnteringTransitionEffect.CardFlipIn:
                case EnteringTransitionEffect.FadeAttackIn:
                case EnteringTransitionEffect.FadeIn:
                case EnteringTransitionEffect.MultiverseIn:
                case EnteringTransitionEffect.Omni360ProIn:
                case EnteringTransitionEffect.OmniArrowIn:
                case EnteringTransitionEffect.PulseChargeIn:
                case EnteringTransitionEffect.Slide3DShadowIn:
                case EnteringTransitionEffect.SlideIn:
                case EnteringTransitionEffect.SpinAttackIn:
                case EnteringTransitionEffect.VortexSpinIn:
                case EnteringTransitionEffect.WaveAuraIn:
                    return $"{effect} (Get Ultimate)";
                default:
                    return effect.ToString();
            }
        }

        private string SceneDropdown(string label, string currentScene)
        {
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(sceneNames, currentScene));
            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, sceneNames);
            return sceneNames[selectedIndex];
        }
    }
}
