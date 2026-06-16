using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [Header("Setting")] 
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float delayDuration = 0.2f;
    
    [Header("UI")]
    public Slider sliderBar;
    public Slider easeSlider;
    
    private float targetValue;
    private float delayTimer;

    public void Initialize(float maxValue, float minValue, float startValue)
    {
        if (sliderBar != null)
        {
            sliderBar.maxValue = maxValue;
            sliderBar.minValue = minValue;
            sliderBar.value = startValue;
        }

        if (easeSlider != null)
        {
            easeSlider.maxValue = maxValue;
            easeSlider.minValue = minValue;
            easeSlider.value = startValue;
        }

        targetValue = startValue;
        delayTimer = 0f;
    }

    public void UpdateValue(float newValue)
    {
        if (Mathf.Approximately(targetValue, newValue)) return;

        targetValue = newValue;
        if (sliderBar != null)
        {
            sliderBar.value = newValue;
        }
        delayTimer = delayDuration;
    }

    private void Update()
    {
        if (easeSlider != null && easeSlider.value != targetValue)
        {
            if (easeSlider.value > targetValue)
            {
                if (delayTimer > 0)
                {
                    delayTimer -= Time.deltaTime;
                }
                else
                {
                    easeSlider.value = Mathf.Lerp(easeSlider.value, targetValue, lerpSpeed * Time.deltaTime);
                    if (Mathf.Abs(easeSlider.value - targetValue) < 0.01f)
                    {
                        easeSlider.value = targetValue;
                    }
                }
            }
            else
            {
                easeSlider.value = targetValue;
            }
        }
    }
}
