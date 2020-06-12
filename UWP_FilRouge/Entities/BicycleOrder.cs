using SQLiteNetExtensions.Attributes;

namespace UWP_FilRouge
{
    public class BicycleOrder
    {
        [ForeignKey(typeof(Bicycle))]
        public int BicycleId { get; set; }
        [ForeignKey(typeof(Order))]
        public int OrderId { get; set; }
    }
}