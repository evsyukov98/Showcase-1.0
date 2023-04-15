using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressLine;
    
    /// <summary>
    /// Сбросить данные.
    /// </summary>
    public void ResetParams()
    {
        progressLine.fillAmount = 0;
    }

    public void SetProgress(float progress)
    {
        progressLine.fillAmount = progress;
    }

    public void SetFull()
    {
        progressLine.fillAmount = 1f;
    }
}
