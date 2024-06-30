using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicalSide
{
    public class DropProp : MonoBehaviour
    {
        public int raised;
        public int weather;
        
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
                if (disp != null && disp.cardTemplate.unit == KindofCard.Silver && disp.cardTemplate.type!="D")
                {
                    disp.cardTemplate.Points += diff;
                }
            }
        }
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
                if (disp != null && disp.cardTemplate.unit == KindofCard.Silver && disp.cardTemplate.type != "D")
                {
                    disp.cardTemplate.Points += diff;
                }
            }
        }
        public void DropClicked()
        {
            PointerData pointer = GameObject.Find("GameManager").GetComponent<PointerData>();
            pointer.PlayCard(this.gameObject);
        }


    }
}
