using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyRemaining : Singleton<EnemyRemaining>
{
     private TMP_Text _enemyRemainingText;

     protected override void Awake()
     {
          base.Awake();

          _enemyRemainingText = GetComponent<TMP_Text>();
     }

     public void SetText(int value)
     {
          _enemyRemainingText.text = $"Enemy Remaining : {value}";
     }
}
