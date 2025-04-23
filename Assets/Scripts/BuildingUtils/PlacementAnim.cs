using System;
using UnityEngine;

namespace BuildingUtils
{

    /// <summary>
    /// The point of this component is to add 
    /// procedural animations whenever a Building
    /// or Road is placed.
    /// </summary>
    public class PlacementAnim : MonoBehaviour
    {
        private const float ANIM_LENGTH_DEFAULT = 2f;
        private const float ANIM_STEP_BY_DEFAULT = 0.01f;
        private const int ANIM_STEPS_DEFAULT = 100;

        private int anim_num_playing = 0;

        private Vector3 pos_start = default;
        private Vector3 pos_end = default;

        private Vector3? size_start;
        private Vector3? size_end;

        private float anim_length = ANIM_LENGTH_DEFAULT;
        private float anim_progress = 0.0f;
        private float? anim_step_by = null;

        private float delay = 0f;

        private int anim_steps_total = ANIM_STEPS_DEFAULT;

        private bool changes_size = false;
        private bool anim_playing = false;


        /// <remarks>
        /// Currently unused
        /// </remarks>
        public float AnimLength
        {
            get => anim_length;
            set => anim_length = Mathf.Clamp(value, 0.0f, 100f);
        }

        /// <summary>
        /// 1 / AnimStepBy is how long the animation will take to complete
        /// </summary>
        public float AnimStepBy
        {
            get => anim_step_by ?? ANIM_STEP_BY_DEFAULT;
            set => anim_step_by = Mathf.Clamp01(value);
        }

        /// <remarks>
        /// Currently unused
        /// </remarks>
        public int AnimSteps
        {
            get => anim_steps_total;
            set => anim_steps_total = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// If you want to have something 
        /// change size as it is being placed,
        /// use AnimSizeOrigin and 
        /// <seealso cref="AnimSizeTarget"/> to
        /// do that.
        /// </summary>
        public Vector3 AnimSizeOrigin
        {
            get => size_start ?? Vector3.one;
            set => size_start = value;
        }

        /// <summary>
        /// If you want to have something 
        /// change size as it is being placed,
        /// use <seealso cref="AnimSizeOrigin"/> 
        /// and AnimSizeTarget to do that.
        /// </summary>
        public Vector3 AnimSizeTarget
        {
            get => size_end ?? Vector3.one;
            set => size_end = value;
        }

        public int NumberAnimsPlaying
        {
            get => anim_num_playing;
        }

        public float Postponed
        {
            get => delay;
            set => delay = value;
        }

        public Vector3 Origin { get => pos_start; set => pos_start = value; }
        public Vector3 Target { get => pos_end; set => pos_end = value; }

        public bool AnimPlaying { get => anim_playing; }

        // You know when these get called.
        public Action OnAnimOver;
        public Action OnAnimStart;

        /// <summary>
        /// Triggers when all animations are done playing.
        /// Note: resets to Null after triggering.
        /// </summary>
        public static Action OnAllAnimsOver;


        public bool InitAnim()
        {
            changes_size = AnimSizeOrigin != AnimSizeTarget;

            anim_playing = changes_size || (pos_start != pos_end);
            if (anim_playing)
            {
                transform.position = pos_start;
                if (delay != 0f)
                {
                    anim_playing = false;
                    Invoke("StartAnim", delay);
                }
            }
            return anim_playing;
        }

        void StartAnim()
        {
            anim_playing = true;
            anim_num_playing++;
            if (OnAnimStart is not null)
                OnAnimStart();
        }

        void Start()
        {
        }

        void FixedUpdate()
        {
            if (anim_playing)
            {
                if (changes_size)
                    transform.localScale = Vector3.Slerp(AnimSizeOrigin, AnimSizeTarget, anim_progress);

                transform.position = Vector3.Slerp(pos_start, pos_end, anim_progress);


                if (anim_progress < 1.0f)
                {
                    anim_progress = Mathf.Clamp01(anim_progress + AnimStepBy);
                }
                else
                {
                    anim_playing = false;
                    if (OnAnimOver is not null)
                        OnAnimOver();
                    anim_num_playing--;
                    if (anim_num_playing == 0)
                    {
                        if (OnAllAnimsOver is not null)
                        {
                            OnAllAnimsOver();
                            OnAllAnimsOver = null;
                        }
                    }
                }

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
