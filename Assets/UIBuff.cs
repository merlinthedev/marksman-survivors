using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffsDebuffs.Stacks;

public class UIBuff : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public int stacks = 0;
    public void Start() {
        SetStacks(0);
    }
    public void SetStacks(int stacks) {
        this.stacks += stacks;
        text.text = this.stacks.ToString();
        if(this.stacks > 0) {

            this.gameObject.SetActive(true);
        }
        else if (this.stacks < 1) {
              this.gameObject.SetActive(false);
        }
    }

    public void SetSprite() {

    }
}
