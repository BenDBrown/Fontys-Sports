using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    private int goalAmount = 0;
    private int missAmount = 0;
    private float currentGoalSpeed = 0;
    private float fastestGoalSpeed = 0;

    [SerializeField] private TextMeshProUGUI goalAmountDisplay;
    [SerializeField] private TextMeshProUGUI missAmountDisplay;
    [SerializeField] private TextMeshProUGUI currentGoalSpeedDisplay;
    [SerializeField] private TextMeshProUGUI fastestGoalSpeedDisplay;
    [SerializeField] private float speedMultiplier = 1;

    public void Goal(float ballSpeed = 0)
    {
        goalAmount++;
        goalAmountDisplay.SetText(goalAmount.ToString());

        if (ballSpeed > 0) UpdateGoalSpeed(ballSpeed);
    }

    private void UpdateGoalSpeed(float ballSpeed)
    {
        currentGoalSpeed = Mathf.RoundToInt(100 * ballSpeed * speedMultiplier) / 100f;
        currentGoalSpeedDisplay.SetText(currentGoalSpeed.ToString());

        if (currentGoalSpeed > fastestGoalSpeed)
        {
            fastestGoalSpeed = currentGoalSpeed;
            fastestGoalSpeedDisplay.SetText(fastestGoalSpeed.ToString());
        }
    }

    public void Miss()
    {
        missAmount++;
        missAmountDisplay.SetText(missAmount.ToString());
    }

    public void ResetScore()
    {
        goalAmount = 0;
        missAmount = 0;
        currentGoalSpeed = 0;
        fastestGoalSpeed = 0;
        goalAmountDisplay.text = "0";
        missAmountDisplay.text = "0";
    }
}
