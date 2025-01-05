using TMPro;
using UnityEngine;

namespace DailyRewards
{
    public class ButtonsUIController : AbstractButtonsUIController
    {
        [SerializeField] private GameObject closeButton;
        [SerializeField] private GameObject claimButton;
        [SerializeField] private GameObject claimX2Button;
        //[SerializeField] private GameObject claimOneMoreButton;
        [SerializeField] private TextMeshProUGUI oneMoreDayText;
        [SerializeField] private string oneMoreDayTextFormat = "Day {0}";
        
        private DailyRewardsModel dataModel;
        
        public override void Initialize(DailyRewardsModel dataModel)
        {
            this.dataModel = dataModel;
        }
        
        public override void UpdateButtons()
        {
            var currentLocalDateTime = DailyRewardsHelper.GetCurrentLocalDateTime().Date;
            var isTodayHaveRewards = currentLocalDateTime.Date == dataModel.RewardDateTime.Date;
            
            // closeButton.SetActive(!isTodayHaveRewards || dataModel.HasClaimed);
            claimButton.SetActive(isTodayHaveRewards && !dataModel.HasClaimed);
            claimX2Button.SetActive(isTodayHaveRewards && !dataModel.HasClaimed);
            //claimOneMoreButton.SetActive(isTodayHaveRewards && dataModel.HasClaimed /*&& !dataModel.HasClaimedAdRewards*/);
            oneMoreDayText.text = string.Format(oneMoreDayTextFormat, dataModel.CurrentDay);
        }
        
        public override void SetAllClaimButtonsActive(bool enable)
        {
            claimButton.SetActive(enable);
            claimX2Button.SetActive(enable);
            //claimOneMoreButton.SetActive(enable);
        }
        
        public override void SetCloseButton(bool enable)
        {
            closeButton.SetActive(enable);
        }
    }
}