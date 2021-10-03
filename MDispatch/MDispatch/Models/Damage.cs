using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;

namespace MDispatch.Models
{
    public class Damage
    {
        public int ID { get; set; }
        public string IndexImageVech { get; set; }
        public string TypePrefDamage { get; set; }
        public string TypeDamage { get; set; }
        public string TypeCurrentStatus { get; set; }
        public int IndexDamage { get; set; }
        public string FullNameDamage { get; set; }
        public double XInterest { get; set; }
        public double YInterest { get; set; }
        public int HeightDamage { get; set; }
        public int WidthDamage { get; set; }
        public string ImageBase64 { get; set; }
        [JsonIgnore]
        public Image Image { get; set; }
        [JsonIgnore]
        public ImageSource ImageSource { get; set; }
    }
}
