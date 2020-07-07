namespace P4G_Save_Tool
{
    public class SocialLink
    {
        private string name;
        private byte id;
        private byte tarot;
        private byte level;
        private byte progress;
        private byte flag;

        public string Name { get { return name; } }
        public byte ID { get { return id; } }
        public byte Progress { get { return progress; } set { progress = value; } }
        public string Tarot { get { return Database.Arcana[tarot]; } }
        public byte Level { get { return level; } set { level = value; } }
        public byte Flag { get { return flag; } set { flag = value; } }

        public SocialLink(string name, byte id, byte tarot, byte level = 1, byte progress = 0, byte flag = 0)
        {
            this.name = name;
            this.id = id;
            this.tarot = tarot;
            this.level = level;
            this.progress = progress;
            this.flag = flag;
        }

        public SocialLink Copy()
        {
            return new SocialLink(name, id, tarot, level, progress, flag);
        }

        public SocialLink Copy(byte level, byte progress, byte flag)
        {
            return new SocialLink(name, id, tarot, level, progress, flag);
        }
    };
}
