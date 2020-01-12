using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public abstract class BicycleCharacteristics : EntityBase
    {
        #region Attributs
        private float size;
        private float weight;
        private string color;
        private float wheelSize;
        private bool electric;
        private string confort;
        // new state or occasion
        private string state;
        private string brand;
        #endregion

        #region Properties
        public float Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        public float Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }
        public float WheelSize
        {
            get { return wheelSize; }
            set
            {
                wheelSize = value;
                OnPropertyChanged("WheelSize");
            }
        }

       public bool Electric
        {
            get { return electric; }
            set
            {
                electric = value;
                OnPropertyChanged("Electric");
            }
        }

        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        public string Brand
        {
            get { return brand; }
            set
            {
                brand = value;
                OnPropertyChanged("Brand");
            }
        }

        public string Confort
        {
            get { return confort; }
            set
            {
                confort = value;
                OnPropertyChanged("Confort");
            }
        }
        #endregion
    }
}
