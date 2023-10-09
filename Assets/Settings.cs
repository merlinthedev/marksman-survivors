using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Slider camSlider;
    [SerializeField] private TMP_Text camText;

    public string[] texts = new string[4];
    public void OnCamSliderChange() {
        cam.orthographicSize = camSlider.value;

        //Compare value and set text
        if(camSlider.value == camSlider.minValue) {
            camText.SetText(texts[5]);
        }
        else if (camSlider.value < ((camSlider.maxValue - camSlider.minValue) / 3 * 1) + camSlider.minValue) {
            camText.SetText(texts[1]);
        } 
        else if (camSlider.value < ((camSlider.maxValue - camSlider.minValue) / 3 * 2) + camSlider.minValue) {
            camText.SetText(texts[2]);
        } 
        else if (camSlider.value < ((camSlider.maxValue - camSlider.minValue) / 3 * 3) + camSlider.minValue) {
            camText.SetText(texts[3]);
        }
        else if (camSlider.value == camSlider.maxValue){
            camText.SetText(texts[4]);
        }

    }





}
