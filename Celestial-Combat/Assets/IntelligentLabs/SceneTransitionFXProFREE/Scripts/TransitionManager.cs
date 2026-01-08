using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransitionFXProFREE
{
    public class TransitionManager : MonoBehaviour
    {
        public enum EnteringTransitionEffect
        {
            // Free Edition Effects
            DissolveIn,
            GlitchIn,
            LeafFallIn,
            ZoomIn,

            // - Ultimate Edition Effects - //
            CardFlipIn,
            FadeAttackIn,
            FadeIn,
            MultiverseIn,
            Omni360ProIn,
            OmniArrowIn,
            PulseChargeIn,
            Slide3DShadowIn,
            SlideIn,
            SpinAttackIn,
            VortexSpinIn,
            WaveAuraIn
        }

        public enum LeavingTransitionEffect
        {
            // Free Edition Effects
            DissolveOut,
            GlitchOut,
            LeafFallOut,
            ZoomOut,

            // - Ultimate Edition Effects - //
            CardFlipOut,
            FadeAttackOut,
            FadeOut,
            MultiverseOut,
            Omni360ProOut,
            OmniArrowOut,
            PulseChargeOut,
            Slide3DShadowOut,
            SlideOut,
            SpinAttackOut,
            VortexSpinOut,
            WaveAuraOut
        }

        public string fromScene;
        public string toScene;

        public enum FlipDirection { LeftFlip, RightFlip }
        public enum LeafFallDirection { TopLeft, TopRight, BottomLeft, BottomRight }
        public enum OmniDirection { OmniLeft, OmniRight }
        public enum SlideDirection { Down, Left, Right, Up }
        public enum Slide3DShadowDirection { Left, Right, Top, Bottom }

        public EnteringTransitionEffect enteringFX = EnteringTransitionEffect.FadeIn;
        public LeavingTransitionEffect leavingFX = LeavingTransitionEffect.FadeOut;

        // Duration parameters
        public float cardFlipDurationIn = 1f;
        public float cardFlipDurationOut = 1f;
        public float dissolveInDuration = 1f;
        public float dissolveOutDuration = 1f;
        public float enteringFadeDuration = 1f;
        public float enteringZoomDuration = 1f;
        public float glitchInDuration = 1f;
        public float glitchOutDuration = 1f;
        public float leafFallInDuration = 1f;
        public float leafFallOutDuration = 1f;
        public float leavingFadeDuration = 1f;
        public float leavingZoomDuration = 1f;
        public float omniDurationIn = 1f;
        public float omniDurationOut = 1f;
        public float PulseFrequencyIn = 5f;
        public float PulseFrequencyOut = 5f;
        public float PulseDurationIn = 3f;
        public float PulseDurationOut = 3f;
        public float fadeAttackInDuration = 1f;
        public float fadeAttackOutDuration = 1f;
        public float slide3DRotationDurationOut = 3f;
        public float slide3DRotationDurationIn = 3f;
        public float slideInDuration = 1f;
        public float slideOutDuration = 1f;
        public float spinDuration = 2f;
        public float spinAttackInDuration = 2f;
        public float spinAttackOutDuration = 2f;

        // Direction parameters
        public FlipDirection cardFlipDirectionIn = FlipDirection.RightFlip;
        public FlipDirection cardFlipDirectionOut = FlipDirection.RightFlip;
        public LeafFallDirection leafFallInDirection = LeafFallDirection.TopLeft;
        public LeafFallDirection leafFallOutDirection = LeafFallDirection.TopLeft;
        public OmniDirection omniDirectionIn = OmniDirection.OmniRight;
        public OmniDirection omniDirectionOut = OmniDirection.OmniLeft;
        public SlideDirection slideInDirection = SlideDirection.Right;
        public SlideDirection slideOutDirection = SlideDirection.Left;
        public Slide3DShadowDirection slide3DShadowInDirection = Slide3DShadowDirection.Right;
        public Slide3DShadowDirection slide3DShadowOutDirection = Slide3DShadowDirection.Left;

        // Other parameters
        public float speedAccelerationIn = 2f;
        public float speedAccelerationOut = 2f;
        public float vortexIntensityIn = 2f;
        public float vortexIntensityOut = 2f;
        public float waveSlideIn = 2f;
        public float waveSlideOut = 2f;
        public float waveOffsetIn = 1f;
        public float waveOffsetOut = 1f;

        public float fragmentedBurstDurationIn = 1.5f;
        public float fragmentedBurstDurationOut = 1.5f;
        public int fragmentCountIn = 8;
        public int fragmentCountOut = 8;
        public float burstSpreadIn = 200f;
        public float burstSpreadOut = 200f;

        private CanvasGroup transitionCanvasGroup;
        private bool isTransitioning = false;

        private void Awake()
        {
            fromScene = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
        }

        private void OnValidate()
        {
            fromScene = SceneManager.GetActiveScene().name;
        }

        public void StartTransition()
        {
            if (!isTransitioning)
            {
                StartCoroutine(PerformTransition());
            }
        }

        private IEnumerator PerformTransition()
        {
            isTransitioning = true;

            // Handle Leaving FX
            switch (leavingFX)
            {
                // Free Edition Effects

                case LeavingTransitionEffect.DissolveOut:
                    yield return StartCoroutine(DissolveOut());
                    break;
                case LeavingTransitionEffect.GlitchOut:
                    yield return StartCoroutine(GlitchOut());
                    break;
                case LeavingTransitionEffect.LeafFallOut:
                    yield return StartCoroutine(LeafFallOut());
                    break;
                case LeavingTransitionEffect.ZoomOut:
                    yield return StartCoroutine(ZoomOut());
                    break;

                // - Ultimate Edition Effects - //

                case LeavingTransitionEffect.CardFlipOut:
                    yield return StartCoroutine(CardFlipOut());
                    break;
                case LeavingTransitionEffect.FadeAttackOut:
                    yield return StartCoroutine(FadeAttackOut());
                    break;
                case LeavingTransitionEffect.FadeOut:
                    yield return StartCoroutine(FadeOut());
                    break;
                case LeavingTransitionEffect.MultiverseOut:
                    yield return StartCoroutine(MultiverseOut());
                    break;
                case LeavingTransitionEffect.Omni360ProOut:
                    yield return StartCoroutine(Omni360ProOut());
                    break;
                case LeavingTransitionEffect.OmniArrowOut:
                    yield return StartCoroutine(OmniArrowOut());
                    break;
                case LeavingTransitionEffect.PulseChargeOut:
                    yield return StartCoroutine(PulseChargeOut());
                    break;
                case LeavingTransitionEffect.Slide3DShadowOut:
                    yield return StartCoroutine(Slide3DShadowOut());
                    break;
                case LeavingTransitionEffect.SlideOut:
                    yield return StartCoroutine(SlideOut());
                    break;
                case LeavingTransitionEffect.SpinAttackOut:
                    yield return StartCoroutine(SpinAttackOut());
                    break;
                case LeavingTransitionEffect.VortexSpinOut:
                    yield return StartCoroutine(VortexSpinOut());
                    break;
                case LeavingTransitionEffect.WaveAuraOut:
                    yield return StartCoroutine(WaveAuraOut());
                    break;
            }

            // Load the new scene
            yield return SceneManager.LoadSceneAsync(toScene);

            // Ensure the CanvasGroup for the new scene is found before fading in
            FindTransitionPanel();

            // Handle Entering FX
            switch (enteringFX)
            {
                // Free Edition Effects

                case EnteringTransitionEffect.DissolveIn:
                    yield return StartCoroutine(DissolveIn());
                    break;
                case EnteringTransitionEffect.GlitchIn:
                    yield return StartCoroutine(GlitchIn());
                    break;
                case EnteringTransitionEffect.LeafFallIn:
                    yield return StartCoroutine(LeafFallIn());
                    break;
                case EnteringTransitionEffect.ZoomIn:
                    yield return StartCoroutine(ZoomIn());
                    break;

                // - Ultimate Edition Effects - //

                case EnteringTransitionEffect.CardFlipIn:
                    yield return StartCoroutine(CardFlipIn());
                    break;
                case EnteringTransitionEffect.FadeAttackIn:
                    yield return StartCoroutine(FadeAttackIn());
                    break;
                case EnteringTransitionEffect.FadeIn:
                    yield return StartCoroutine(FadeIn());
                    break;
                case EnteringTransitionEffect.MultiverseIn:
                    yield return StartCoroutine(MultiverseIn());
                    break;
                case EnteringTransitionEffect.Omni360ProIn:
                    yield return StartCoroutine(Omni360ProIn());
                    break;
                case EnteringTransitionEffect.OmniArrowIn:
                    yield return StartCoroutine(OmniArrowIn());
                    break;
                case EnteringTransitionEffect.PulseChargeIn:
                    yield return StartCoroutine(PulseChargeIn());
                    break;
                case EnteringTransitionEffect.Slide3DShadowIn:
                    yield return StartCoroutine(Slide3DShadowIn());
                    break;
                case EnteringTransitionEffect.SlideIn:
                    yield return StartCoroutine(SlideIn());
                    break;
                case EnteringTransitionEffect.SpinAttackIn:
                    yield return StartCoroutine(SpinAttackIn());
                    break;
                case EnteringTransitionEffect.VortexSpinIn:
                    yield return StartCoroutine(VortexSpinIn());
                    break;
                case EnteringTransitionEffect.WaveAuraIn:
                    yield return StartCoroutine(WaveAuraIn());
                    break;
            }

            Destroy(gameObject); // Delay destruction until both transitions are complete
        }


        // - TRANSITIONS FX PRO FREE - //

        private IEnumerator DissolveIn()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one * 0.8f;
            Vector3 endScale = Vector3.one;

            float startAlpha = 0f;
            float endAlpha = 1f;

            while (elapsedTime < dissolveInDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / dissolveInDuration);

                transitionCanvasGroup.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

                yield return null;
            }
        }

        private IEnumerator DissolveOut()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one;
            Vector3 endScale = Vector3.one * 0.8f;

            float startAlpha = 1f;
            float endAlpha = 0f;

            while (elapsedTime < dissolveOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / dissolveOutDuration);

                transitionCanvasGroup.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

                yield return null;
            }
        }

        private IEnumerator GlitchIn()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < glitchInDuration)
            {
                elapsedTime += Time.deltaTime;

                float xGlitch = Random.Range(-10f, 10f);
                float yGlitch = Random.Range(-10f, 10f);
                transitionCanvasGroup.transform.localPosition = new Vector3(xGlitch, yGlitch, 0);

                transitionCanvasGroup.alpha = Mathf.PingPong(Time.time * 20f, 1f);

                yield return null;
            }

            transitionCanvasGroup.transform.localPosition = Vector3.zero;
            transitionCanvasGroup.alpha = 1f;
        }

        private IEnumerator GlitchOut()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < glitchOutDuration)
            {
                elapsedTime += Time.deltaTime;

                float xGlitch = Random.Range(-10f, 10f);
                float yGlitch = Random.Range(-10f, 10f);
                transitionCanvasGroup.transform.localPosition = new Vector3(xGlitch, yGlitch, 0);

                transitionCanvasGroup.alpha = Mathf.PingPong(Time.time * 20f, 1f);

                yield return null;
            }

            transitionCanvasGroup.transform.localPosition = Vector3.zero;
        }

        private IEnumerator LeafFallIn()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startPosition = GetLeafFallStartPosition(leafFallInDirection);
            Vector3 endPosition = Vector3.zero;

            Quaternion startRotation = Quaternion.Euler(0, 0, -45);
            Quaternion endRotation = Quaternion.identity;

            while (elapsedTime < leafFallInDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / leafFallInDuration);

                transitionCanvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
                transitionCanvasGroup.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

                yield return null;
            }
        }

        private IEnumerator LeafFallOut()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = GetLeafFallEndPosition(leafFallOutDirection);

            Quaternion startRotation = Quaternion.identity;
            Quaternion endRotation = Quaternion.Euler(0, 0, -45);

            while (elapsedTime < leafFallOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / leafFallOutDuration);

                transitionCanvasGroup.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
                transitionCanvasGroup.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

                yield return null;
            }
        }

        // Helper for Leaf Fall Transitions
        private Vector3 GetLeafFallStartPosition(LeafFallDirection direction)
        {
            switch (direction)
            {
                case LeafFallDirection.TopLeft:
                    return new Vector3(-Screen.width / 2, Screen.height, 0);
                case LeafFallDirection.TopRight:
                    return new Vector3(Screen.width / 2, Screen.height, 0);
                case LeafFallDirection.BottomLeft:
                    return new Vector3(-Screen.width / 2, -Screen.height, 0);
                case LeafFallDirection.BottomRight:
                    return new Vector3(Screen.width / 2, -Screen.height, 0);
                default:
                    return Vector3.zero;
            }
        }

        private Vector3 GetLeafFallEndPosition(LeafFallDirection direction)
        {
            switch (direction)
            {
                case LeafFallDirection.TopLeft:
                    return new Vector3(-Screen.width / 2, Screen.height, 0);
                case LeafFallDirection.TopRight:
                    return new Vector3(Screen.width / 2, Screen.height, 0);
                case LeafFallDirection.BottomLeft:
                    return new Vector3(-Screen.width / 2, -Screen.height, 0);
                case LeafFallDirection.BottomRight:
                    return new Vector3(Screen.width / 2, -Screen.height, 0);
                default:
                    return Vector3.zero;
            }
        }

        private IEnumerator ZoomIn()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;

            while (elapsedTime < enteringZoomDuration)
            {
                elapsedTime += Time.deltaTime;
                transitionCanvasGroup.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / enteringZoomDuration);
                yield return null;
            }
        }

        private IEnumerator ZoomOut()
        {
            FindTransitionPanel();
            if (transitionCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one;
            Vector3 endScale = Vector3.zero;

            while (elapsedTime < leavingZoomDuration)
            {
                elapsedTime += Time.deltaTime;
                transitionCanvasGroup.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / leavingZoomDuration);
                yield return null;
            }
        }


        // - TRANSITIONS FX PRO ULTIMATE - //

        private IEnumerator CardFlipIn()
        {
            yield return null;
        }

        private IEnumerator CardFlipOut()
        {
            yield return null;
        }

        private IEnumerator FadeAttackIn()
        {
            yield return null;
        }

        private IEnumerator FadeAttackOut()
        {
            yield return null;
        }

        private IEnumerator FadeIn()
        {
            yield return null;
        }

        private IEnumerator FadeOut()
        {
            yield return null;
        }

        private IEnumerator Omni360ProIn()
        {
            yield return null;
        }

        private IEnumerator Omni360ProOut()
        {
            yield return null;
        }

        private IEnumerator OmniArrowIn()
        {
            yield return null;
        }

        private IEnumerator OmniArrowOut()
        {
            yield return null;
        }

        private IEnumerator Slide3DShadowIn()
        {
            yield return null;
        }

        private IEnumerator Slide3DShadowOut()
        {
            yield return null;
        }

        private IEnumerator SlideIn()
        {
            yield return null;
        }

        private IEnumerator SlideOut()
        {
            yield return null;
        }

        private IEnumerator SpinAttackIn()
        {
            yield return null;
        }

        private IEnumerator SpinAttackOut()
        {
            yield return null;
        }

        private IEnumerator VortexSpinIn()
        {
            yield return null;
        }

        private IEnumerator VortexSpinOut()
        {
            yield return null;
        }

        private IEnumerator PulseChargeIn()
        {
            yield return null;
        }

        private IEnumerator PulseChargeOut()
        {
            yield return null;
        }

        private IEnumerator WaveAuraIn()
        {
            yield return null;
        }

        private IEnumerator WaveAuraOut()
        {
            yield return null;
        }

        private IEnumerator MultiverseIn()
        {
            yield return null;
        }

        private IEnumerator MultiverseOut()
        {
            yield return null;
        }

        // Helper method to find transition panel
        private void FindTransitionPanel()
        {
            if (transitionCanvasGroup == null)
            {
                GameObject panelObj = GameObject.FindGameObjectWithTag("TransitionPanel");

                if (panelObj != null)
                {
                    transitionCanvasGroup = panelObj.GetComponent<CanvasGroup>();

                    if (transitionCanvasGroup == null)
                    {
                        transitionCanvasGroup = panelObj.AddComponent<CanvasGroup>();
                    }
                }
                else
                {
                    Debug.LogError("TransitionPanel not found in the current scene. Please tag your panel with 'TransitionPanel'.");
                }
            }
        }
    }
}
