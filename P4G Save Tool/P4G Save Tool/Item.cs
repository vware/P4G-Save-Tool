namespace P4G_Save_Tool
{
    public class Item
    {
        private string name;
        private ushort id;
        public string Name { get { return name; } }
        public ushort ID { get { return id; } }
        public Item(string name, ushort id)
        {
            this.name = name;
            this.id = id;
        }
    };

    public class ItemStack
    {
        public string Text { get; set; }
        public ushort ID;

        public ItemStack(string text, ushort id)
        {
            Text = text;
            ID = id;
        }
    };

    public class ItemByte
    {
        private string name;
        private byte id;
        public string Name { get { return name; } }
        public byte ID { get { return id; } }

        public ItemByte(string name, byte id)
        {
            this.name = name;
            this.id = id;
        }
    };
}
