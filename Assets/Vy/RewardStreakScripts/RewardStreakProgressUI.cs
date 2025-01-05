using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

namespace RewardStreak
{
    public class RewardStreakProgressUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private RewardStreakMilestone[] milestones;
        [SerializeField] private int beginIndex = 0;

        [SerializeField, Header("Custom Animation")]
        private float moveTimePerFragment = 1f;
        [SerializeField] private Ease moveEase = Ease.Linear;
        
        private HashSet<float> triggeredMilestones = new HashSet<float>();
        
        private int TotalMilestone => milestones.Length;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MoveFromCurrentTo(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MoveFromCurrentTo(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                MoveFromCurrentTo(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                MoveFromCurrentTo(0);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                Initialize(beginIndex);
            }
        }

        private void Awake()
        {
            Initialize(beginIndex);
        }
        
        [ContextMenu("Initialize at start")]
        public void InitializeAtStart()
        {
            Initialize(beginIndex);
        }

        public void Initialize(int startIndex)
        {
            if (!IsIndexValid(startIndex, milestones))
            {
                Debug.LogError($"{nameof(RewardStreakProgressUI)}: Invalid index");
                return;
            }

            var progressValue = startIndex / ((float)milestones.Length - 1);
            slider.value = progressValue;
            
            for (int i = 0; i < milestones.Length; i++)
            {
                milestones[i].Initialize(i <= startIndex);
            }
        }

        private void MoveFromCurrentTo(int toIndex)
        {
            var toProgressValue = toIndex / (float)TotalMilestone;
            var currentProgressValue = slider.value;
            var totalFragments = TotalMilestone - 1;
            var currentValue = slider.value;
            var moveTimeDuration = Math.Abs(currentValue - toProgressValue) * totalFragments * moveTimePerFragment;
            triggeredMilestones.Clear();
            
            Action<float> checkMilestone = currentProgressValue < toProgressValue ? CheckMilestonesOn : CheckMilestonesOff;
            DOTween.To(() => slider.value, x => slider.value = x, toProgressValue, moveTimeDuration)
                .OnUpdate(() =>
                {
                    checkMilestone(slider.value);
                })
                .OnComplete(() =>
                {
                    Debug.Log("Animation done");
                });
        }

        private void MoveTo(int fromIndex, int toIndex)
        {
            var sliderValue = fromIndex / (float)TotalMilestone;
            slider.value = sliderValue;

            if (fromIndex == toIndex)
            {
                milestones[fromIndex].TriggerOn();
                return;
            }
            
            var toProgressValue = toIndex / (float)TotalMilestone;
            var moveTimeDuration = moveTimePerFragment * Mathf.Abs(toIndex - fromIndex);
            triggeredMilestones.Clear();
            Action<float> checkMilestone = toIndex > fromIndex ? CheckMilestonesOn : CheckMilestonesOff;
            DOTween.To(() => slider.value, x => slider.value = x, toProgressValue, moveTimeDuration)
                .OnUpdate(() =>
                {
                    checkMilestone(slider.value);
                })
                .OnComplete(() =>
                {
                    Debug.Log("Animation done");
                });
        }
        
        private void CheckMilestonesOn(float currentValue)
        {
            foreach (var milestone in milestones)
            {
                var milestoneValue = milestone.ValueOnSlider;

                if (currentValue < milestoneValue || triggeredMilestones.Contains(milestoneValue)) continue;
                Debug.Log($"Milestone {milestoneValue} reached!");
                triggeredMilestones.Add(milestoneValue);
                if (milestone != null)
                    milestone.TriggerOn();
            }
        }
        
        private void CheckMilestonesOff(float currentValue)
        {
            foreach (var milestone in milestones)
            {
                var milestoneValue = milestone.ValueOnSlider;

                if (currentValue > milestoneValue || triggeredMilestones.Contains(milestoneValue)) continue;
                Debug.Log($"Milestone {milestoneValue} reached!");
                triggeredMilestones.Add(milestoneValue);
                if (milestone != null)
                    milestone.TriggerOn();
            }
        }

        public void Snap(int fromIndex, int toIndex)
        {
        }

        private static bool IsIndexValid<T>(int index, T[] list)
        {
            return 0 <= index && index < list.Length;
        }
    }
}