using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicalSide
{
    /// <summary>
    /// This class tracks how much the board area has been affected by raising or weather conditions
    /// and applies those effects to the cards on the drop area.
    /// </summary>
    public class DropProp : MonoBehaviour
    {
        public int raised;
        public int weather;
        
        /// <summary>
        /// Adjusts the effects on the drop area when the status changes (e.g., a buff or debuff is applied).
        /// This method modifies the `raised` or `weather` variables based on whether the `diff` is positive or negative.
        /// </summary>
        public void DropStatus(int diff)
        {
            if (diff > 0)
                raised += diff;
            else
                weather += diff;
            CardDisplay disp;
            foreach (Transform cardTransform in transform)
            {
                disp = cardTransform.GetComponent<CardDisplay>();
                if (disp != null && disp.cardTemplate.unit == KindofCard.Silver && disp.cardTemplate.TypeOfCard!="D")
                {
                    disp.cardTemplate.Power += diff;
                }
            }
        }

        /// <summary>
        /// Reverses the effects on the drop area when a reset is triggered.
        /// This function behaves similarly to DropStatus but handles the inversion of effects.
        /// </summary>
        public void DropOnReset(int diff)
        {
            if (diff < 0)
                raised += diff;
            else
                weather += diff;
            CardDisplay disp;
            foreach (Transform cardTransform in transform)
            {
                disp = cardTransform.GetComponent<CardDisplay>();
                if (disp != null && disp.cardTemplate.unit == KindofCard.Silver && disp.cardTemplate.TypeOfCard != "D")
                {
                    disp.cardTemplate.Power += diff;
                }
            }
        }

        /// <summary>
        /// Handles the interaction when a player clicks on the drop area.
        /// This triggers the card playing logic by calling the `PlayCard` method on the selected card.
        /// </summary>
        public void DropClicked()
        {
            PointerData pointer = GameObject.Find("GameManager").GetComponent<PointerData>();
            pointer.PlayCard(this.gameObject);
        }


    }
}
