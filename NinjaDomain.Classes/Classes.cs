using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NinjaDomain.Classes
{
    public class Ninja
    {
        public Ninja()
        {
            //Instantiate EquipmentOwned in the constructor, so we can always add equipment to it.
            //Otherwise we get null expception errors when we try to add equipment without remembering to istantiate the List<> first.
            EquipmentOwned = new List<NinjaEquipment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool ServedInOniwaban { get; set; }
        public Clan Clan { get; set; }
        public int ClanId { get; set; }
        public virtual List<NinjaEquipment> EquipmentOwned { get; set; }
        //public virtual List<NinjaEquipment> EquipmentOwned { get; set; } //Property must be virtual to allow lazy loading.
        public System.DateTime DateOfBirth { get; set; }
    }

    public class NinjaEquipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
        [Required] //advises EF to create a 1 to * relationship rather than a 0..1 to * relationship (which is the default for nullable types).
        public Ninja Ninja { get; set; }
    }

    public class Clan
    {
        public Clan()
        {
            //Instantiate Ninjas in the constructor, so we can always add ninjas to it.
            //Otherwise we get null expception errors when we try to add ninjas without remembering to istantiate the List<> first.
            Ninjas = new List<Ninja>();
        }

        public int Id { get; set; }
        public string ClanName { get; set; }
        public List<Ninja> Ninjas { get; set; }
    }
}
