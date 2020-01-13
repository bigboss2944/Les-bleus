using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Models
{
    public class BicycleCharacteristics
    {
        #region Attributs
        private float size;
        private float weight;
        private string color;
        private float wheelSize;
        private bool electric;
        private string confort;
        private string state;// new state or occasion
        private string brand;
        #endregion

        #region Properties
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public string Color
        {
            get { return color; }
            set { color = value; }
        }
        public float WheelSize
        {
            get { return wheelSize; }
            set { wheelSize = value; }
        }

        public bool Electric
        {
            get { return electric; }
            set { electric = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        public string Confort
        {
            get { return confort; }
            set { confort = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BicycleCharacteristics()
        {

        }
        #endregion
    }
}