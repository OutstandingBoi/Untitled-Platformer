using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Slider energySlider;
    [SerializeField] PlayerToggleCollision player;

    void Start()
    {
        energySlider.maxValue = player.MaxEnergy;
        energySlider.value = player.MaxEnergy;
    }

    void Update()
    {
        energySlider.value = player.CurrentEnergy;
    }
}
