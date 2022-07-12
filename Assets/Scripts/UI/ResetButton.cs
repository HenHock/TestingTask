using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void ResetLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Inventory.Reset();
    }
}
