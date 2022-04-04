using Poukoute;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour {
    [Range(0, 16)]
    public int outlineSize = 1;
    [Range(2, 4)]
    public float lerp = 3f;
    [SerializeField]
    private Color outLineColor = Color.white;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private const float MAX_FLASH_AMOUNT = 0.5f;
    private const float MIN_FLASH_AMOUNT = 0.1f;
    private const float EPSILON = 0.01f;

    private float flashAmount = MIN_FLASH_AMOUNT;
    private float rightAngle = MAX_FLASH_AMOUNT;
    private Color flashColor = Color.white;
    private Material spriteRenderMat;

    void OnEnable() {
        OnCountryClick countryClick = GetComponent<OnCountryClick>();
        flashColor = countryClick.countryIndex > 8 ?
            new Color(1f, 47 / 255f, 120 / 255f, 102 / 255f) :
            new Color(63 / 255f, 203 / 255f, 1f);
        spriteRenderMat = spriteRenderer.material;
        UpdateOutline(true);
    }

    void OnDisable() {
        UpdateOutline(false);
    }

    void Update() {
        FlashSpriteColor();
    }

    void UpdateOutline(bool isEnable) {
        spriteRenderMat.SetColor("_FlashColor", isEnable ? flashColor : Color.white);
        spriteRenderMat.SetFloat("_FlashAmount", isEnable ? MIN_FLASH_AMOUNT : 0.0f);
        spriteRenderMat.SetFloat("_Outline", isEnable ? 1f : 0);
        spriteRenderMat.SetColor("_OutlineColor", outLineColor);
        spriteRenderMat.SetFloat("_OutlineSize", outlineSize);
    }

    private void FlashSpriteColor() {
        flashAmount = Mathf.Lerp(flashAmount, rightAngle, lerp * Time.unscaledDeltaTime);
        //Debug.Log("Update " + rightAngle + " " + flashAmount);
        spriteRenderMat.SetFloat("_FlashAmount", flashAmount);
        if (GameHelper.NearlyEqual(flashAmount, MAX_FLASH_AMOUNT, EPSILON)) {
            rightAngle = MIN_FLASH_AMOUNT;
        } else if (GameHelper.NearlyEqual(flashAmount, MIN_FLASH_AMOUNT, EPSILON)) {
            rightAngle = MAX_FLASH_AMOUNT;
        }
    }
}
