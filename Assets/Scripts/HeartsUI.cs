using UnityEngine;

public class HeartsUI : MonoBehaviour
{
    public Animator[] heartAnimators;

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartAnimators.Length; i++)
        {
            bool empty = i >= currentHealth;
            heartAnimators[i].SetBool("Empty", empty);
        }
    }
}




