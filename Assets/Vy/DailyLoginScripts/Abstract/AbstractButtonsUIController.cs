using DailyRewards;
using UnityEngine;

public abstract class AbstractButtonsUIController : MonoBehaviour
{
    public abstract void Initialize(DailyRewardsModel dataModel);
    public abstract void UpdateButtons();
    public abstract void SetAllClaimButtonsActive(bool enable);
    public abstract void SetCloseButton(bool enable);
}
