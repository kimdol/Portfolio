using ARPG.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Items
{
    [Serializable]
    public class ItemBuff
    {
        #region Variables
        public CharacterAttribute stat;
        public int value;

        [SerializeField]
        private int min;

        [SerializeField]
        private int max;
        #endregion Variables

        #region Properties
        public int Min => min;
        public int Max => max;

        #endregion Properties

        #region Methods

        public ItemBuff(int min, int max)
        {
            this.min = min;
            this.max = max;

            GenerateValue();
        }

        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }

        #endregion Methods

        #region IModifier interface
        public void AddValue(ref int v)
        {
            v += value;
        }
        #endregion IModifier interface
    }

}