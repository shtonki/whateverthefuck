using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view.guicomponents
{
    class EquipmentPanel : Panel
    {
        public EquipmentPanel(Equipment equipment, GLCoordinate size)
        {
            this.Size = size;

            equipment.OnEquipmentChanged += this.UpdateItem;

            foreach (EquipmentSlots slot in Enum.GetValues(typeof(EquipmentSlots)))
            {
                var item = equipment.GetItem(slot);
                if (item != null)
                {
                    UpdateItem(slot, item);
                }
            }
        }

        private void UpdateItem(EquipmentSlots slot, Item item)
        {
            switch (slot)
            {
                case EquipmentSlots.Head:
                {
                    if (HeadContainer != null) { RemoveChild(HeadContainer); }
                    HeadContainer = new ItemContainer(item);
                    HeadContainer.Size = new GLCoordinate(this.Size.X / 3, this.Size.Y / 3);
                    HeadContainer.Location = new GLCoordinate(0, 0);
                    AddChild(HeadContainer);
                } break;

                case EquipmentSlots.MainHand:
                {
                    if (MainHandContainer != null) { RemoveChild(MainHandContainer); }
                    MainHandContainer = new ItemContainer(item);
                    MainHandContainer.Size = new GLCoordinate(this.Size.X / 3, this.Size.Y / 3);
                    MainHandContainer.Location = new GLCoordinate(this.Size.X / 3, 0);
                    AddChild(MainHandContainer);
                } break;
            }
        }

        private ItemContainer HeadContainer { get; set; }

        private ItemContainer MainHandContainer { get; set; }
    }
}
