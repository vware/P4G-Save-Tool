namespace P4G_Save_Tool
{
    public class Persona
    {
        public string Name { get { return Database.personae[id]; } }
        public bool exists;
        public byte unknown0;
        public ushort id;
        public byte level { get; set; }
        public uint totalxp { get; set; }
        public ushort unknown1;
        public byte unknown2;

        public Item skill1 { get; set; }
        public Item skill2 { get; set; }
        public Item skill3 { get; set; }
        public Item skill4 { get; set; }
        public Item skill5 { get; set; }
        public Item skill6 { get; set; }
        public Item skill7 { get; set; }
        public Item skill8 { get; set; }
        public byte st { get; set; }
        public byte ma { get; set; }
        public byte de { get; set; }
        public byte ag { get; set; }
        public byte lu { get; set; }

        private Item FromID(ushort id)
        {
            if (id >= Database.allSkills.Length) return Database.skills[0];
            for (int i = 0; i < Database.skills.Count; i++)
                if (Database.skills[i].ID == id)
                    return Database.skills[i];
            return Database.skills[0];
        }

        public Persona(bool exists = true, byte unknown0 = 0, ushort id = 0, byte level = 0, uint totalxp = 0, ushort unknown1 = 0, byte unknown2 = 2, ushort skill1 = 0, ushort skill2 = 0, ushort skill3 = 0, ushort skill4 = 0, ushort skill5 = 0, ushort skill6 = 0, ushort skill7 = 0, ushort skill8 = 0, byte st = 0, byte ma = 0, byte de = 0, byte ag = 0, byte lu = 0)
        {
            this.exists = exists;
            this.unknown0 = unknown0;
            this.id = id;
            this.level = level;
            this.totalxp = totalxp;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.skill1 = FromID(skill1);
            this.skill2 = FromID(skill2);
            this.skill3 = FromID(skill3);
            this.skill4 = FromID(skill4);
            this.skill5 = FromID(skill5);
            this.skill6 = FromID(skill6);
            this.skill7 = FromID(skill7);
            this.skill8 = FromID(skill8);
            this.st = st;
            this.ma = ma;
            this.de = de;
            this.ag = ag;
            this.lu = lu;
        }
    }
}
