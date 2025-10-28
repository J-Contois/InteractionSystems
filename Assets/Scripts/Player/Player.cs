using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main controller for the player. Toute la logique de mouvement/rotation est dans PlayerMovement.
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// Initializes the cursor in locked mode at startup.
        /// </summary>
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
