using UnityEngine;

namespace RewardStreak
{
    public class RewardStreakMilestone : MonoBehaviour
    {
        [SerializeField] private float valueOnSlider = 0;
        [SerializeField] private Animator animator;
        [SerializeField] private string activateTrigger = "Activate";
        [SerializeField] private string deactivateTrigger = "Deactivate";
        [SerializeField] private GameObject gameObjectImageOn;

        public float ValueOnSlider => valueOnSlider;
        
        public void Initialize(bool on)
        {
            gameObjectImageOn.SetActive(on);
            animator.SetTrigger(on ? activateTrigger : deactivateTrigger);
        }
        
        public void TriggerOn()
        {
            gameObjectImageOn.SetActive(true);
            animator.SetTrigger(activateTrigger);
        }

        public void TriggerOff()
        {
            gameObjectImageOn.SetActive(true);
            animator.SetTrigger(deactivateTrigger);
        }
    }
}