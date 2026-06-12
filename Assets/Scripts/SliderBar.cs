using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [Header("Setting")] 
    [SerializeField] private float lerpSpeed = 5f;
    
    [Header("UI")]
    public Slider sliderBar;
    public Slider easeSlider;
    
    private float targetValue;

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
    }

    public void UpdateValue(float newValue)
    {
        targetValue = newValue;
        if (sliderBar != null)
        {
            sliderBar.value = newValue;
        }
    }

    private void Update()
    {
        if (easeSlider != null && easeSlider.value != targetValue)
        {
            if (easeSlider.value > targetValue)
            {
                easeSlider.value = Mathf.Lerp(easeSlider.value, targetValue, lerpSpeed * Time.deltaTime);
                if (Mathf.Abs(easeSlider.value - targetValue) < 0.01f)
                {
                    easeSlider.value = targetValue;
                }
            }
            else
            {
                easeSlider.value = targetValue;
            }
        }
    }
}
