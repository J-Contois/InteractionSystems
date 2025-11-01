using UnityEngine;
using UnityEngine.UI;

namespace Weapon
{
    /// <summary>
    /// Simple crosshair UI script. Attach to a Canvas with an Image as the crosshair.
    /// </summary>
    public class CrosshairUI : MonoBehaviour
    {
        /// <summary>Reference to the crosshair image.</summary>
        [SerializeField] private RawImage crosshairImage;

        [Header("Dynamic Crosshair")]
        [Tooltip("Default crosshair color")]
        [SerializeField] private Color defaultColor = Color.white;
        [Tooltip("Aiming crosshair color")]
        [SerializeField] private Color aimingColor = Color.green;
        [Tooltip("Shooting crosshair color")]
        [SerializeField] private Color shootingColor = Color.red;
        [Tooltip("Default crosshair size")]
        [SerializeField] private float defaultSize = 32f;
        [Tooltip("Aiming crosshair size")]
        [SerializeField] private float aimingSize = 24f;
        [Tooltip("Shooting crosshair size")]
        [SerializeField] private float shootingSize = 40f;
        private Coroutine _shootAnim;

        /// <summary>
        /// Sets the crosshair state (normal, aiming, shooting).
        /// </summary>
        public void SetState(string state)
        {
            if (crosshairImage == null) return;
            switch (state)
            {
                case "Aiming":
                    crosshairImage.color = aimingColor;
                    crosshairImage.rectTransform.sizeDelta = new Vector2(aimingSize, aimingSize);
                    break;
                case "Shooting":
                    crosshairImage.color = shootingColor;
                    crosshairImage.rectTransform.sizeDelta = new Vector2(shootingSize, shootingSize);
                    if (_shootAnim != null) StopCoroutine(_shootAnim);
                    _shootAnim = StartCoroutine(ResetAfterShoot());
                    break;
                default:
                    crosshairImage.color = defaultColor;
                    crosshairImage.rectTransform.sizeDelta = new Vector2(defaultSize, defaultSize);
                    break;
            }
        }

        private System.Collections.IEnumerator ResetAfterShoot()
        {
            yield return new WaitForSeconds(0.1f);
            SetState("Normal");
        }
    }
}
