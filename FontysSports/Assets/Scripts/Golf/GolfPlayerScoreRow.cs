using TMPro;
using UnityEngine;

public class GolfPlayerScoreRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI currentHitsText;

    [SerializeField]
    private TextMeshProUGUI totalHitsText;

    public void SetPlayerScoreInfo(GolfPlayerScoreInfo scoreInfo)
    { 
        nameText.text = scoreInfo.Name;
        currentHitsText.text = scoreInfo.CurrentHits.ToString();
        totalHitsText.text = scoreInfo.TotalHits.ToString();
    }
}
