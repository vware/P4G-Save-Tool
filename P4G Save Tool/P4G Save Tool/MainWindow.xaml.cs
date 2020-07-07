using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace P4G_Save_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filename;
        byte[] currentFileCopy;
        bool readyEvents;
        bool[] scroll = { false, false };
        int[] itemSel = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        ScrollViewer invScroll, stackScroll;
        Item[] charEList;
        List<Item> armor;
        List<Item> accessories;
        List<Item> consumables;
        List<Item> books;
        List<Item> materials;
        List<Item> cards;
        List<Item> veggies;
        List<Item> minerals;
        List<Item> socialLink;
        List<Item> shelf;
        List<Item> costumes;
        List<Item> questitems;
        List<Item> other;
        SocialLink[] socialLinkIDs;
        string originalTitle = "P4G Save Tool";
        List<Item> personae;
        string[] courageLevels;
        string[] diligenceLevels;
        string[] understandingLevels;
        string[] expressionLevels;
        string[] knowledgeLevels;

        uint mcTotalXp;
        ushort[] socialStats;
        ushort[] equippedWeapons;
        ushort[] equippedArmors;
        ushort[] equippedAccessories;
        ushort[] equippedCostumes;
        public byte Day { get; set; }
        public byte NextDay { get; set; }
        public byte DayPhase { get; set; }
        public byte NextDayPhase { get; set; }
        byte mcLevel;
        uint yen;
        string firstname, surname;
        Persona[][] slots;
        List<Item>[] weps;
        List<Item>[] items;
        Persona[] compendium;

        public string Yen
        {
            get { return yen.ToString(); }
        }

        public MainWindow()
        {
            readyEvents = false;
            InitializeComponent();
            invScroll = (ScrollViewer)invBox.GetDescendantByType(typeof(ScrollViewer));
            stackScroll = (ScrollViewer)stackBox.GetDescendantByType(typeof(ScrollViewer));

            mcTotalXp = 0;

            compendium = new Persona[232];

            socialStats = new ushort[5];

            understandingLevels = new string[] { "Basic", "Kindly", "Generous", "Motherly", "Saintly" };
            knowledgeLevels = new string[] { "Aware", "Informed", "Expert", "Professor", "Sage" };
            courageLevels = new string[] { "Average", "Reliable", "Brave", "Daring", "Heroic" };
            expressionLevels = new string[] { "Rough", "Eloquent", "Persuasive", "Touching", "Enthralling" };
            diligenceLevels = new string[] { "Callow", "Persistent", "Strong", "Thorough", "Rock Solid" };

            ItemByte[] members = new ItemByte[7]
            {
                new ItemByte("Blank", 0),
                new ItemByte("Yosuke", 2),
                new ItemByte("Chie", 3),
                new ItemByte("Yukiko", 4),
                new ItemByte("Kanji", 6),
                new ItemByte("Naoto", 7),
                new ItemByte("Teddie", 8),
            };

            member1.ItemsSource = members;
            member2.ItemsSource = members;
            member3.ItemsSource = members;
            member1.SelectedIndex = 0;
            member2.SelectedIndex = 0;
            member3.SelectedIndex = 0;

            charEList = new Item[]
            {
                new Item("Yu Narukami", 0),
                new Item("Yosuke Hanamura", 1),
                new Item("Chie Satonaka", 2),
                new Item("Yukiko Amagi", 3),
                new Item("Kanji Tatsumi", 5),
                new Item("Naoto Shirogane", 6),
                new Item("Teddie", 7)
            };

            charBox.ItemsSource = charEList;
            charBox.SelectedIndex = 0;

            equippedWeapons = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedArmors = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedAccessories = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            equippedCostumes = new ushort[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            invScroll.ScrollChanged += (object o, ScrollChangedEventArgs e) =>
            {
                if (invScroll.IsMouseOver)
                {
                    stackScroll.ScrollToVerticalOffset(invScroll.VerticalOffset);
                }
            };

            stackScroll.ScrollChanged += (object o, ScrollChangedEventArgs e) =>
            {
                if (stackScroll.IsMouseOver)
                {
                    invScroll.ScrollToVerticalOffset(stackScroll.VerticalOffset);
                }
            };

            string[] itmSections = new string[]
            {
                "Weapons",
                "Armor",
                "Accessories",
                "Consumables",
                "Materials",
                "Skill Cards",
                "Books",
                "Veggies",
                "Minerals",
                "Social Link",
                "Shelf",
                "Costumes",
                "Bugs",
                "Fish",
                "Quest Items",
                "Other"
            };

            socialLinkIDs = new SocialLink[]
            {
                new SocialLink("Blank", 0, 0, 0),
                new SocialLink("Investigation Team", 1, 1, 1),
                new SocialLink("Nanako", 2, 9, 1),
                new SocialLink("Rise", 3, 7, 1),
                new SocialLink("Rise (GF)", 4, 7, 1),
                new SocialLink("Yukiko", 5, 3, 1),
                new SocialLink("Yukiko (GF)", 6, 3, 1),
                new SocialLink("Yosuke", 7, 2, 1),
                new SocialLink("Dojima", 8, 6, 1),
                new SocialLink("Sayoko", 9, 16, 1),
                new SocialLink("Kanji", 10, 5, 1),
                new SocialLink("Chie", 11, 8, 1),
                new SocialLink("Chie (GF)", 12, 3, 1),
                new SocialLink("Fox", 13, 10, 1),
                new SocialLink("Naoto", 14, 11, 1),
                new SocialLink("Naoto (GF)", 15, 11, 1),
                new SocialLink("Fellow Athletes (Kou)", 16, 12, 1),
                new SocialLink("Fellow Athletes (Daisuke)", 17, 12, 1),
                new SocialLink("Naoki", 18, 13, 1),
                new SocialLink("Hisano", 19, 14, 1),
                new SocialLink("Margaret", 20, 4, 1),
                new SocialLink("Ai", 21, 19, 1),
                new SocialLink("Ai (GF)", 22, 19, 1),
                new SocialLink("Shu", 23, 17, 1),
                new SocialLink("Teddie", 24, 18, 1),
                new SocialLink("Yumi", 25, 20, 1),
                new SocialLink("Yumi (GF)", 26, 20, 1),
                new SocialLink("Ayane", 27, 20, 1),
                new SocialLink("Ayane (GF)", 28, 20, 1),
                new SocialLink("Eri", 29, 15, 1),
                new SocialLink("The Seekers of Truth", 30, 21, 1),
                new SocialLink("Adachi", 31, 25, 1),
                new SocialLink("Adachi (Hunger)", 32, 26, 1),
                new SocialLink("Marie", 33, 22, 1),
                new SocialLink("Marie (GF)", 34, 22, 1),
            };

            inventory = new byte[2559];

            sLinkComboBox.ItemsSource = socialLinkIDs;
            sLinkComboBox.SelectedIndex = 0;

            personae = new List<Item>(Utils.GetFromDatabase(Database.personae, 1, 42));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 44, 8));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 53, 127));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 182, 32));
            personae.AddRange(Utils.GetFromDatabase(Database.personae, 224, 26));

            personae.Insert(0, new Item("Blank", 0));

            Database.skills = new List<Item>(Utils.GetFromDatabase(Database.allSkills, 0, 255));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 259, 42));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 349, 46));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 440, 13));
            Database.skills.AddRange(Utils.GetFromDatabase(Database.allSkills, 472, 151));

            compendiumComboBox.ItemsSource = personae;
            compendiumComboBox.SelectedIndex = 0;

            Item wepBlank = Utils.GetFromDatabase(Database.allItems, 0, 1)[0];
            Item armorBlank = Utils.GetFromDatabase(Database.allItems, 256, 1)[0];
            Item accessoryBlank = Utils.GetFromDatabase(Database.allItems, 512, 1)[0];
            Item consumableBlank = Utils.GetFromDatabase(Database.allItems, 768, 1)[0];
            Item materialBlank = Utils.GetFromDatabase(Database.allItems, 1280, 1)[0];
            Item cardBlank = Utils.GetFromDatabase(Database.allItems, 1536, 1)[0];
            Item costumeBlank = Utils.GetFromDatabase(Database.allItems, 1792, 1)[0];
            Item questitemsBlank = Utils.GetFromDatabase(Database.allItems, 977, 1)[0];
            Item otherBlank = Utils.GetFromDatabase(Database.allItems, 1024, 1)[0];
            Item mineralBlank = Utils.GetFromDatabase(Database.allItems, 2048, 1)[0];

            List<Item> mcWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1, 36));
            List<Item> yoWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 39, 32));
            List<Item> yuWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 77, 28));
            List<Item> chWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 112, 31));
            List<Item> kaWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 150, 26));
            List<Item> naWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 183, 15));
            List<Item> teWeps = new List<Item>(Utils.GetFromDatabase(Database.allItems, 217, 22));

            List<Item> bugs = new List<Item>(Utils.GetFromDatabase(Database.allItems, 909, 7));

            List<Item> fish = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1004, 3));

            armor = new List<Item>(Utils.GetFromDatabase(Database.allItems, 257, 8));
            accessories = new List<Item>(Utils.GetFromDatabase(Database.allItems, 513, 96));
            consumables = new List<Item>(Utils.GetFromDatabase(Database.allItems, 769, 51));
            books = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1136, 5));
            materials = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1281, 128));
            cards = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1537, 252));
            veggies = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2089, 16));
            minerals = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2128, 29));

            socialLink = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1184, 20));
            shelf = new List<Item>(Utils.GetFromDatabase(Database.allItems, 2056, 5));
            costumes = new List<Item>(Utils.GetFromDatabase(Database.allItems, 1792, 193));
            questitems = new List<Item>(Utils.GetFromDatabase(Database.allItems, 978, 29));
            other = new List<Item>();

            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 615, 69));
            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 685, 1));
            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 687, 8));

            accessories.AddRange(Utils.GetFromDatabase(Database.allItems, 754, 13));

            veggies.AddRange(Utils.GetFromDatabase(Database.allItems, 2107, 4));

            costumes.AddRange(Utils.GetFromDatabase(Database.allItems, 2040, 6));

            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 266, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 287, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 293, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 307, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 315, 3));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 328, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 334, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 347, 2));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 350, 9));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 367, 4));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 372, 7));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 387, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 394, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 407, 6));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 414, 5));
            armor.AddRange(Utils.GetFromDatabase(Database.allItems, 420, 13));

            shelf.AddRange(Utils.GetFromDatabase(Database.allItems, 1234, 13));

            books.AddRange(Utils.GetFromDatabase(Database.allItems, 1145, 7));
            books.AddRange(Utils.GetFromDatabase(Database.allItems, 1259, 20));

            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1207, 3));
            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1224, 3));
            socialLink.AddRange(Utils.GetFromDatabase(Database.allItems, 1228, 1));

            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1410, 21));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1432, 21));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1454, 20));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1475, 19));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1495, 19));
            materials.AddRange(Utils.GetFromDatabase(Database.allItems, 1513, 21));

            mcWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2305, 11));
            yoWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2326, 9));
            yuWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2345, 11));
            chWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2367, 9));
            kaWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2385, 3));
            kaWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2389, 8));
            naWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2407, 8));
            teWeps.AddRange(Utils.GetFromDatabase(Database.allItems, 2425, 8));

            fish.AddRange(Utils.GetFromDatabase(Database.allItems, 1008, 7));

            mcWeps.Add(new Item(Database.allItems[2434], 2434));
            yoWeps.Add(new Item(Database.allItems[2435], 2435));
            yuWeps.Add(new Item(Database.allItems[2437], 2437));
            chWeps.Add(new Item(Database.allItems[2436], 2436));
            kaWeps.Add(new Item(Database.allItems[2438], 2438));
            naWeps.Add(new Item(Database.allItems[2439], 2439));
            teWeps.Add(new Item(Database.allItems[2440], 2440));

            Database.weapons = new List<Item>();
            Database.weapons.AddRange(mcWeps);
            Database.weapons.AddRange(yoWeps);
            Database.weapons.AddRange(chWeps);
            Database.weapons.AddRange(yuWeps);
            Database.weapons.AddRange(kaWeps);
            Database.weapons.AddRange(naWeps);
            Database.weapons.AddRange(teWeps);

            mcWeps.Insert(0, wepBlank);
            yoWeps.Insert(0, wepBlank);
            yuWeps.Insert(0, wepBlank);
            chWeps.Insert(0, wepBlank);
            kaWeps.Insert(0, wepBlank);
            naWeps.Insert(0, wepBlank);
            teWeps.Insert(0, wepBlank);

            consumables.Insert(0, consumableBlank);
            armor.Insert(0, armorBlank);
            accessories.Insert(0, accessoryBlank);
            cards.Insert(0, cardBlank);

            minerals.Insert(0, mineralBlank);
            materials.Insert(0, materialBlank);
            veggies.Insert(0, consumableBlank);
            shelf.Insert(0, otherBlank);
            socialLink.Insert(0, otherBlank);
            books.Insert(0, otherBlank);
            questitems.Insert(0, questitemsBlank);
            other.Insert(0, otherBlank);
            bugs.Insert(0, otherBlank);
            fish.Insert(0, otherBlank);

            Database.weapons.Insert(0, wepBlank);

            weps = new List<Item>[] { mcWeps, yoWeps, chWeps, yuWeps, null, kaWeps, naWeps, teWeps };
            items = new List<Item>[] { Database.weapons, armor, accessories, consumables, materials, cards, books, veggies, minerals, socialLink, shelf, costumes, bugs, fish, questitems, other };

            itemSectBox.ItemsSource = itmSections;
            itemSectBox.SelectedIndex = 0;
            member.ItemsSource = Database.party;
            member.SelectedIndex = 0;

            skillBox1.ItemsSource = Database.skills; compSkillBox1.ItemsSource = Database.skills;
            skillBox2.ItemsSource = Database.skills; compSkillBox2.ItemsSource = Database.skills;
            skillBox3.ItemsSource = Database.skills; compSkillBox3.ItemsSource = Database.skills;
            skillBox4.ItemsSource = Database.skills; compSkillBox4.ItemsSource = Database.skills;
            skillBox5.ItemsSource = Database.skills; compSkillBox5.ItemsSource = Database.skills;
            skillBox6.ItemsSource = Database.skills; compSkillBox6.ItemsSource = Database.skills;
            skillBox7.ItemsSource = Database.skills; compSkillBox7.ItemsSource = Database.skills;
            skillBox8.ItemsSource = Database.skills; compSkillBox8.ItemsSource = Database.skills;

            itemBox.ItemsSource = items[0];
            personaIDs.ItemsSource = personae;
            phaseBox.ItemsSource = Database.phases;
            nextPhaseBox.ItemsSource = Database.phases;
            skillBox1.SelectedIndex = 0; compSkillBox1.SelectedIndex = 0;
            skillBox2.SelectedIndex = 0; compSkillBox2.SelectedIndex = 0;
            skillBox3.SelectedIndex = 0; compSkillBox3.SelectedIndex = 0;
            skillBox4.SelectedIndex = 0; compSkillBox4.SelectedIndex = 0;
            skillBox5.SelectedIndex = 0; compSkillBox5.SelectedIndex = 0;
            skillBox6.SelectedIndex = 0; compSkillBox6.SelectedIndex = 0;
            skillBox7.SelectedIndex = 0; compSkillBox7.SelectedIndex = 0;
            skillBox8.SelectedIndex = 0; compSkillBox8.SelectedIndex = 0;
            personaIDs.SelectedIndex = 0;
            phaseBox.SelectedIndex = 0;
            nextPhaseBox.SelectedIndex = 0;
            itemBox.SelectedIndex = 0;
            slots = new Persona[][]
            {
               new Persona[12] { new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona(), new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() },
               new Persona[1] { new Persona() }
            };

            Day = 0;
            NextDay = 0;

            courageBox.ItemsSource = courageLevels;
            knowledgeBox.ItemsSource = knowledgeLevels;
            expressionBox.ItemsSource = expressionLevels;
            understandingBox.ItemsSource = understandingLevels;
            diligenceBox.ItemsSource = diligenceLevels;

            courageBox.SelectedIndex = 0;
            knowledgeBox.SelectedIndex = 0;
            expressionBox.SelectedIndex = 0;
            understandingBox.SelectedIndex = 0;
            diligenceBox.SelectedIndex = 0;

            wepBox.ItemsSource = weps[(charBox.SelectedItem as Item).ID];
            wepBox.InvalidateVisual();
            armBox.ItemsSource = armor;
            accBox.ItemsSource = accessories;
            cosBox.ItemsSource = costumes;
            wepBox.SelectedIndex = 0;
            armBox.SelectedIndex = 0;
            accBox.SelectedIndex = 0;
            cosBox.SelectedIndex = 0;

            dayBox.Text = "0";
            nextDayBox.Text = "0";

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string file = args[1];
                if (File.Exists(file))
                {
                    int last = file.LastIndexOf('\\');
                    filename = file.Substring(last + 1, file.Length - (last + 1));

                    OpenFile(File.OpenRead(file));
                    Title = originalTitle + " - " + filename;
                }
            }
        }

        private int PointsToStatusLevel(byte status, ushort points)
        {
            if (status == 3)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }

            if (status == 1)
            {
                if (points <= 29)
                    return 1;
                else if (points >= 30 && points <= 79)
                    return 2;
                else if (points >= 80 && points <= 149)
                    return 3;
                else if (points >= 150 && points <= 239)
                    return 4;
                else if (points >= 240)
                    return 5;
            }
            else if (status == 0)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }
            else if (status == 4)
            {
                if (points <= 12)
                    return 1;
                else if (points >= 13 && points <= 32)
                    return 2;
                else if (points >= 33 && points <= 52)
                    return 3;
                else if (points >= 53 && points <= 84)
                    return 4;
                else if (points >= 85)
                    return 5;
            }
            else if (status == 2)
            {
                if (points <= 15)
                    return 1;
                else if (points >= 16 && points <= 39)
                    return 2;
                else if (points >= 40 && points <= 79)
                    return 3;
                else if (points >= 80 && points <= 139)
                    return 4;
                else if (points >= 140)
                    return 5;
            }

            return 0;
        }

        private ushort StatusLevelToPoints(byte status, byte level)
        {
            if (status == 3)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;
            }

            if (status == 1)
            {
                if (level == 1)
                    return 29;
                else if (level == 2)
                    return 30;
                else if (level == 3)
                    return 80;
                else if (level == 4)
                    return 150;
                else if (level == 5)
                    return 240;
            }
            else if (status == 0)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;

            }
            else if (status == 4)
            {
                if (level == 1)
                    return 12;
                else if (level == 2)
                    return 13;
                else if (level == 3)
                    return 33;
                else if (level == 4)
                    return 53;
                else if (level == 5)
                    return 85;
            }
            else if (status == 2)
            {
                if (level == 1)
                    return 15;
                else if (level == 2)
                    return 16;
                else if (level == 3)
                    return 40;
                else if (level == 4)
                    return 80;
                else if (level == 5)
                    return 140;
            }

            return 0;
        }

        private void personaID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private byte[] inventory;

        private void personaIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                Persona persona = slots[member.SelectedIndex][personaSlot.SelectedIndex];
                persona.id = (personaIDs.SelectedItem as Item).ID;
                if (persona.level == 0 && persona.id != 0 && persona.exists == false)
                    persona.level = 1;
                persona.exists = persona.id != 0;
            }
        }

        private void skillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill1 = (skillBox1.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill2 = (skillBox2.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill3 = (skillBox3.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill4 = (skillBox4.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill5 = (skillBox5.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill6 = (skillBox6.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill7 = (skillBox7.SelectedItem as Item);
                slots[member.SelectedIndex][personaSlot.SelectedIndex].skill8 = (skillBox8.SelectedItem as Item);
            }
        }

        private void compSkillBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                (compendiumBox.SelectedItem as Persona).skill1 = (compSkillBox1.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill2 = (compSkillBox2.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill3 = (compSkillBox3.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill4 = (compSkillBox4.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill5 = (compSkillBox5.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill6 = (compSkillBox6.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill7 = (compSkillBox7.SelectedItem as Item);
                (compendiumBox.SelectedItem as Persona).skill8 = (compSkillBox8.SelectedItem as Item);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            readyEvents = true;
        }

        private void personaSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                readyEvents = false;
                for (int i = 0; i < personae.Count; i++)
                    if ((personaIDs.Items[i] as Item).ID == slots[member.SelectedIndex][personaSlot.SelectedIndex].id)
                        personaIDs.SelectedIndex = i;
                skillBox1.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill1;
                skillBox2.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill2;
                skillBox3.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill3;
                skillBox4.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill4;
                skillBox5.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill5;
                skillBox6.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill6;
                skillBox7.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill7;
                skillBox8.SelectedItem = slots[member.SelectedIndex][personaSlot.SelectedIndex].skill8;

                LVSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].level;
                STSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].st;
                MASlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].ma;
                DESlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].de;
                AGSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].ag;
                LUSlider.Value = slots[member.SelectedIndex][personaSlot.SelectedIndex].lu;

                xpBox.Text = slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp.ToString();

                readyEvents = true;
            }
        }

        private void LVSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
            {
                slots[member.SelectedIndex][personaSlot.SelectedIndex].level = (byte)e.NewValue;
                if (slots[member.SelectedIndex][personaSlot.SelectedIndex].level > 99)
                    LVVal.Foreground = new SolidColorBrush(Colors.Red);
                else
                    LVVal.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void STSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].st = (byte)e.NewValue;
        }

        private void MASlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].ma = (byte)e.NewValue;
        }

        private void DESlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].de = (byte)e.NewValue;
        }

        private void AGSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].ag = (byte)e.NewValue;
        }

        private void LUSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
                slots[member.SelectedIndex][personaSlot.SelectedIndex].lu = (byte)e.NewValue;
        }

        private void member_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            personaSlot.SelectedIndex = 0;
            if (member.SelectedIndex != 0)
                personaSlot.IsEnabled = false;
            else personaSlot.IsEnabled = true;
            personaSlot_SelectionChanged(null, null);
            MCLVSlider.IsEnabled = member.SelectedIndex == 0;
            mcXpBox.IsEnabled = member.SelectedIndex == 0;
            calcXp.IsEnabled = member.SelectedIndex == 0;
        }

        private void yenBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(yenBox.Text, out result))
                    yen = result;
                else
                    yenBox.Text = yen.ToString();
            }
        }

        private void phaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                DayPhase = (byte)phaseBox.SelectedIndex;
            }
        }

        private void fnBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                firstname = fnBox.Text;
                Database.party[0] = firstname + " " + surname;
                charEList[0] = new Item(firstname + " " + surname, charEList[0].ID);
                member.ItemsSource = Database.party;
                charBox.ItemsSource = charEList;
                member.SelectedIndex = 1;
                member.SelectedIndex = 0;
                charBox.SelectedIndex = 1;
                charBox.SelectedIndex = 0;
            }
        }

        private void snBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                surname = snBox.Text;
                Database.party[0] = firstname + " " + surname;
                charEList[0] = new Item(firstname + " " + surname, charEList[0].ID);
                member.ItemsSource = Database.party;
                charBox.ItemsSource = charEList;
                member.SelectedIndex = 1;
                member.SelectedIndex = 0;
                charBox.SelectedIndex = 1;
                charBox.SelectedIndex = 0;
            }
        }

        private void SaveFile(Stream stream)
        {
            using (BinaryWriter w = new BinaryWriter(stream))
            {
                w.Write(currentFileCopy);
                w.BaseStream.Position = 16;
                w.WriteJString(surname);
                w.BaseStream.Position = 34;
                w.WriteJString(firstname);
                w.BaseStream.Position = 88;
                w.Write(yen);
                for (byte i = 0; i < 3; i++)
                {
                    w.Write(((partymembers.Children[i] as ComboBox).SelectedItem as ItemByte).ID);
                    w.BaseStream.Position++;
                }

                w.BaseStream.Position = 100;
                w.WritePString(surname);
                w.WritePString(firstname);
                w.BaseStream.Position = 136;
                for (int i = 0; i < invBox.Items.Count; i++)
                {
                    inventory[(invBox.Items[i] as Item).ID] = byte.Parse((stackBox.Items[i] as TextBox).Text);
                }

                w.Write(inventory);
                w.BaseStream.Position = 2700;
                for (int i = 0; i < 12; i++)
                {
                    w.WritePersona(slots[0][i]);
                    w.BaseStream.Position += 15;
                }

                w.BaseStream.Position = 3360;
                w.Write(equippedWeapons[0]);
                w.Write(equippedArmors[0]);
                w.Write(equippedAccessories[0]);
                w.Write(equippedCostumes[0]);
                w.BaseStream.Position = 3492;
                //              w.BaseStream.Position = 3500;
                for (int i = 1; i < 8; i++)
                {
                    w.Write(equippedWeapons[i]);
                    w.Write(equippedArmors[i]);
                    w.Write(equippedAccessories[i]);
                    w.Write(equippedCostumes[i]);
                    w.WritePersona(slots[i][0]);
                    w.BaseStream.Position += 91;
                }

                w.BaseStream.Position = 3290;
                w.Write(mcLevel);
                w.BaseStream.Position = 3292;
                w.Write(mcTotalXp);

                w.BaseStream.Position = 3336;
                for (byte i = 0; i < 5; i++)
                    w.Write(socialStats[i]);

                w.BaseStream.Position = 6484;
                w.Write(Day);
                w.BaseStream.Position++;
                w.Write(DayPhase);
                w.BaseStream.Position = 6492;
                w.Write(NextDay);
                w.BaseStream.Position++;
                w.Write(NextDayPhase);
                w.BaseStream.Position = 6512;
                byte[] chunk = new byte[368];
                Array.Clear(chunk, 0, chunk.Length);
                w.Write(chunk);
                w.BaseStream.Position = 6512;
                byte lcount = (byte)(23 <= sLinkBox.Items.Count ? 23 : sLinkBox.Items.Count);
                for (byte i = 0; i < lcount; i++)
                {
                    SocialLink s = sLinkBox.Items[i] as SocialLink;
                    w.Write(s.ID);
                    w.BaseStream.Position++;
                    w.Write(s.Level);
                    w.BaseStream.Position++;
                    w.Write(s.Progress);
                    w.BaseStream.Position += 7;
                    w.Write(s.Flag);
                    if (i < lcount - 1) w.BaseStream.Position += 3;
                }

                w.BaseStream.Position = 9688;
                byte lcount2 = (byte)(compendiumBox.Items.Count);
                byte[] zeromemory = new byte[11937];
                Array.Clear(zeromemory, 0, zeromemory.Length);
                w.Write(zeromemory);
                for (int i = 0; i < lcount2; i++)
                {
                    Persona persona = compendiumBox.Items[i] as Persona;
                    w.BaseStream.Position = 9688 + (48 * (persona.id - 1));
                    w.WritePersona(persona);
                }
            }

            if (File.Exists(filename + "slot"))
            {
                byte[] md5 = MD5.Create().ComputeHash(File.OpenRead(filename));
                Stream s = File.OpenWrite(filename + "slot");

                using (BinaryWriter w = new BinaryWriter(s))
                {
                    w.Seek(0x18, 0);
                    w.Write(md5);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(File.OpenWrite(filename));
        }

        private void itemSectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                itemBox.ItemsSource = items[itemSectBox.SelectedIndex];
                itemBox.SelectedIndex = itemSel[itemSectBox.SelectedIndex];
            }
        }

        private void AddInventoryItem(Item item, byte stack)
        {
            if (item.Name == "Blank" || item.ID == 1792) return;
            invBox.Items.Add(item);
            invBox.SelectedItem = item;
            var box = new TextBox() { Text = stack.ToString(), Width = 30, Height = 16, TextWrapping = TextWrapping.NoWrap, TextAlignment = TextAlignment.Right };
            box.PreviewTextInput += personaID_PreviewTextInput;
            box.SelectionChanged += (object o, RoutedEventArgs e) =>
            {
                stackBox.SelectedItem = e.Source;
            };
            box.TextChanged += itemStack_TextChanged;
            stackBox.Items.Add(box);
            stackBox.SelectedIndex = invBox.SelectedIndex;
        }

        private void AddCompendiumItem(Persona persona)
        {
            if (persona.Name == "Blank" || persona.id == 0) return;
            compendiumBox.Items.Add(persona);
        }

        private void RemoveInventoryItem(Item item)
        {
            int i = invBox.Items.IndexOf(item);
            invBox.Items.RemoveAt(i);
            stackBox.Items.RemoveAt(i);
        }

        private void itemBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                if (itemBox.SelectedIndex != -1)
                {
                    itemSel[itemSectBox.SelectedIndex] = itemBox.SelectedIndex;
                    if (invBox.Items.Count == 0)
                    {
                        AddInventoryItem(items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]], 1);
                    }
                    else
                        for (int i = 0; i < invBox.Items.Count; i++)
                            if ((invBox.Items[i] as Item).ID != items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]].ID)
                            {
                                if (i == invBox.Items.Count - 1)
                                {
                                    AddInventoryItem(items[itemSectBox.SelectedIndex][itemSel[itemSectBox.SelectedIndex]], 1);
                                    break;
                                }
                            }
                            else
                            {
                                invBox.SelectedIndex = i;
                                break;
                            }
                }
                else
                {
                    itemSel[itemSectBox.SelectedIndex] = itemBox.SelectedIndex = 0;
                }
            }
        }

        private void invBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackBox.SelectedIndex = invBox.SelectedIndex;
        }

        private void stackBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            invBox.SelectedIndex = stackBox.SelectedIndex;
        }

        private void itemStack_TextChanged(object sender, TextChangedEventArgs e)
        {
            stackBox.SelectedIndex = stackBox.Items.IndexOf(e.Source);
            byte result = 0;
            if (!byte.TryParse((stackBox.SelectedItem as TextBox).Text, out result))
                (stackBox.SelectedItem as TextBox).Text = inventory[(invBox.Items[stackBox.SelectedIndex] as Item).ID].ToString();
            else
            {
                inventory[(invBox.Items[stackBox.SelectedIndex] as Item).ID] = result;
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void skillBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (Item i in (e.Source as ComboBox).Items)
            {
                if (i.Name.ToString().ToUpper().Contains(e.Text.ToUpper()))
                {
                    (e.Source as ComboBox).SelectedItem = i;
                    break;
                }
            }

            e.Handled = true;
        }

        private void MCLVSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (readyEvents)
            {
                mcLevel = (byte)(e.Source as Slider).Value;
                if (mcLevel > 99)
                    MCLVVal.Foreground = new SolidColorBrush(Colors.Red);
                else
                    MCLVVal.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            deleteMenuItem.IsEnabled = invBox.SelectedIndex != -1;
        }

        private void ContextMenu_Opened2(object sender, RoutedEventArgs e)
        {
            deleteMenuItem2.IsEnabled = sLinkBox.SelectedIndex != -1;
        }

        private void ContextMenu_Opened3(object sender, RoutedEventArgs e)
        {
            deleteMenuItem3.IsEnabled = compendiumBox.SelectedIndex != -1;
            deleteAllMenuItem.IsEnabled = compendiumBox.Items.Count > 0;
        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                RemoveInventoryItem(invBox.SelectedItem as Item);
            }
        }

        private void deleteMenuItem_Click2(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                sLinkBox.Items.Remove(sLinkBox.SelectedItem);
            }
        }

        private void deleteMenuItem_Click3(object sender, RoutedEventArgs e)
        {
            if (readyEvents)
            {
                compendiumBox.Items.Remove(compendiumBox.SelectedItem);
            }
        }

        private void charBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                int sect = (charBox.SelectedItem as Item).ID;
                ushort[] eq = new ushort[8];
                equippedWeapons.CopyTo(eq, 0);
                wepBox.ItemsSource = weps[sect];
                for (int i = 0; i < weps[sect].Count; i++)
                {
                    if (weps[sect][i].ID == eq[sect])
                        wepBox.SelectedItem = weps[sect][i];
                    if (weps[sect][i].ID == 0)
                        wepBox.SelectedIndex = 0;
                }

                for (int i = 0; i < armor.Count; i++)
                {
                    if (armor[i].ID == equippedArmors[(charBox.SelectedItem as Item).ID])
                    {
                        armBox.SelectedIndex = 0;
                        armBox.SelectedItem = armor[i];
                    }
                    if (equippedArmors[(charBox.SelectedItem as Item).ID] == 0)
                        armBox.SelectedIndex = 0;
                }

                for (int i = 0; i < accessories.Count; i++)
                {
                    if (accessories[i].ID == equippedAccessories[(charBox.SelectedItem as Item).ID])
                    {
                        accBox.SelectedIndex = 0;
                        accBox.SelectedItem = accessories[i];
                    }
                    if (equippedAccessories[(charBox.SelectedItem as Item).ID] == 0)
                        accBox.SelectedIndex = 0;
                }

                for (int i = 0; i < costumes.Count; i++)
                {
                    if (costumes[i].ID == equippedCostumes[(charBox.SelectedItem as Item).ID])
                    {
                        cosBox.SelectedIndex = 0;
                        cosBox.SelectedItem = costumes[i];
                    }
                    if (equippedCostumes[(charBox.SelectedItem as Item).ID] == 0)
                        cosBox.SelectedIndex = 0;
                }
            }
        }

        private void wepBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedWeapons[(charBox.SelectedItem as Item).ID] = wepBox.SelectedItem != null ? (wepBox.SelectedItem as Item).ID : (ushort)0;
        }

        private void armBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedArmors[charBox.SelectedIndex] = (armBox.SelectedItem as Item).ID;

        }

        private void accBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedAccessories[charBox.SelectedIndex] = (accBox.SelectedItem as Item).ID;

        }

        private void cosBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
                equippedCostumes[charBox.SelectedIndex] = (cosBox.SelectedItem as Item).ID;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.Owner = this;
            a.ShowDialog();
        }

        private void sLinkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                if (sLinkComboBox.SelectedIndex != 0)
                {
                    for (byte i = 0; i < sLinkBox.Items.Count; i++)
                        if ((sLinkBox.Items[i] as SocialLink).ID == (sLinkComboBox.SelectedItem as SocialLink).ID)
                            return;
                    sLinkBox.Items.Add(socialLinkIDs[sLinkComboBox.SelectedIndex].Copy());
                }
            }
        }

        private void OpenFile(Stream file)
        {
            using (BinaryReader r = new BinaryReader(file))
            {
                r.BaseStream.Position = 16;
                surname = r.ReadJString();
                r.BaseStream.Position = 34;
                firstname = r.ReadJString();
                r.BaseStream.Position = 88;
                yen = r.ReadUInt32();
                for (byte i = 0; i < 3; i++)
                {
                    byte c = r.ReadByte();
                    for (byte p = 0; p < 7; p++)
                        if (((partymembers.Children[i] as ComboBox).Items[p] as ItemByte).ID == c)
                            (partymembers.Children[i] as ComboBox).SelectedIndex = p;
                    r.BaseStream.Position++;
                }
                r.BaseStream.Position = 100;
                surname = r.ReadPString();
                firstname = r.ReadPString();
                r.BaseStream.Position = 136;
                inventory = r.ReadBytes(2559);
                r.BaseStream.Position = 2700;
                for (int i = 0; i < 12; i++)
                {
                    slots[0][i] = r.ReadPersona();
                    r.BaseStream.Position += 15;
                }
                r.BaseStream.Position = 3336;
                for (byte i = 0; i < 5; i++)
                    socialStats[i] = r.ReadUInt16();

                r.BaseStream.Position = 3360;
                equippedWeapons[0] = r.ReadUInt16();
                equippedArmors[0] = r.ReadUInt16();
                equippedAccessories[0] = r.ReadUInt16();
                equippedCostumes[0] = r.ReadUInt16();
                r.BaseStream.Position = 3492;
                //                  memoryReader.BaseStream.Position = 3500;
                for (int i = 1; i < 8; i++)
                {
                    equippedWeapons[i] = r.ReadUInt16();
                    equippedArmors[i] = r.ReadUInt16();
                    equippedAccessories[i] = r.ReadUInt16();
                    equippedCostumes[i] = r.ReadUInt16();
                    slots[i][0] = r.ReadPersona();
                    r.BaseStream.Position += 91;
                }

                r.BaseStream.Position = 3290;
                mcLevel = r.ReadByte();
                r.BaseStream.Position = 3292;
                mcTotalXp = r.ReadUInt32();

                r.BaseStream.Position = 6484;
                Day = r.ReadByte();
                r.BaseStream.Position++;
                DayPhase = r.ReadByte();
                r.BaseStream.Position = 6492;
                NextDay = r.ReadByte();
                r.BaseStream.Position++;
                NextDayPhase = r.ReadByte();
                r.BaseStream.Position = 6512;
                sLinkBox.Items.Clear();
                for (byte i = 0; i < 23; i++)
                {
                    byte id = r.ReadByte();
                    r.BaseStream.Position++;
                    byte level = r.ReadByte();
                    r.BaseStream.Position++;
                    byte progress = r.ReadByte();
                    r.BaseStream.Position += 7;
                    byte flag = r.ReadByte();
                    if (id != 0)
                    {
                        bool found = false;
                        for (byte q = 0; q < socialLinkIDs.Length; q++)
                        {
                            if (socialLinkIDs[q].ID == id)
                            {
                                sLinkBox.Items.Add(socialLinkIDs[q].Copy(level, progress, flag));
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            sLinkBox.Items.Add(new SocialLink("Unknown (" + id.ToString() + ')', id, 0, level, progress, flag));
                        }
                    }

                    if (i < 22) r.BaseStream.Position += 3;
                }
                compendiumBox.Items.Clear();
                //r.BaseStream.Position = 109620;
                r.BaseStream.Position = 9688;
                for (int i = 0; i < 249; i++)
                {
                    r.BaseStream.Position = 9688 + (48 * i);
                    Persona persona = r.ReadPersona();
                    AddCompendiumItem(persona);
                }

                r.BaseStream.Position = 0;
                currentFileCopy = r.ReadBytes((int)r.BaseStream.Length);

                if (compendiumBox.Items.Count > 0)
                    compendiumBox.SelectedIndex = 0;
                Database.party[0] = firstname + surname;
                member.ItemsSource = Database.party;
                member.InvalidateVisual();
                yenBox.Text = yen.ToString();
                phaseBox.SelectedIndex = DayPhase;
                nextPhaseBox.SelectedIndex = NextDayPhase;
                dayBox.Text = Day.ToString();
                nextDayBox.Text = NextDay.ToString();
                snBox.Text = surname;
                fnBox.Text = firstname;

                mcXpBox.Text = mcTotalXp.ToString();

                MCLVSlider.Value = mcLevel;

                readyEvents = false;
                courageBox.SelectedIndex = PointsToStatusLevel(0, socialStats[0]) - 1;
                knowledgeBox.SelectedIndex = PointsToStatusLevel(1, socialStats[1]) - 1;
                diligenceBox.SelectedIndex = PointsToStatusLevel(2, socialStats[2]) - 1;
                understandingBox.SelectedIndex = PointsToStatusLevel(3, socialStats[3]) - 1;
                expressionBox.SelectedIndex = PointsToStatusLevel(4, socialStats[4]) - 1;
                readyEvents = true;

                invBox.Items.Clear();
                stackBox.Items.Clear();

                for (int i = 0; i < inventory.Length; i++)
                    if (inventory[i] != 0)
                        AddInventoryItem(new Item(Database.allItems[i], (ushort)i), inventory[i]);

                Array.Clear(inventory, 0, inventory.Length);

                member.SelectedIndex = 1;
                member.SelectedIndex = 0;

                member_SelectionChanged(null, null);
                charBox_SelectionChanged(null, null);

                saveMenu.IsEnabled = true;
                saveAsMenu.IsEnabled = true;
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Binary files (*.bin)|*.bin";
            if (d.ShowDialog() == true)
            {
                SaveFile(File.OpenWrite(filename = d.FileName));
                Title = originalTitle + " - " + d.SafeFileName;
            }
        }

        private void compendiumBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (compendiumBox.SelectedItem == null)
            {
                compSkillBox1.IsEnabled = false;
                compSkillBox2.IsEnabled = false;
                compSkillBox3.IsEnabled = false;
                compSkillBox4.IsEnabled = false;
                compSkillBox5.IsEnabled = false;
                compSkillBox6.IsEnabled = false;
                compSkillBox7.IsEnabled = false;
                compSkillBox8.IsEnabled = false;
                readyEvents = false;
                compSkillBox1.SelectedItem = Database.skills[0];
                compSkillBox2.SelectedItem = Database.skills[0];
                compSkillBox3.SelectedItem = Database.skills[0];
                compSkillBox4.SelectedItem = Database.skills[0];
                compSkillBox5.SelectedItem = Database.skills[0];
                compSkillBox6.SelectedItem = Database.skills[0];
                compSkillBox7.SelectedItem = Database.skills[0];
                compSkillBox8.SelectedItem = Database.skills[0];
                readyEvents = true;
                compendiumComboBox.SelectedIndex = 0;
            }
            else
            {
                compSkillBox1.IsEnabled = true;
                compSkillBox2.IsEnabled = true;
                compSkillBox3.IsEnabled = true;
                compSkillBox4.IsEnabled = true;
                compSkillBox5.IsEnabled = true;
                compSkillBox6.IsEnabled = true;
                compSkillBox7.IsEnabled = true;
                compSkillBox8.IsEnabled = true;
                for (int i = 0; i < compendiumComboBox.Items.Count; i++)
                {
                    if ((compendiumComboBox.Items[i] as Item).ID == (compendiumBox.SelectedItem as Persona).id)
                        compendiumComboBox.SelectedIndex = i;
                }

                if (readyEvents)
                {
                    readyEvents = false;
                    compSkillBox1.SelectedItem = (compendiumBox.SelectedItem as Persona).skill1;
                    compSkillBox2.SelectedItem = (compendiumBox.SelectedItem as Persona).skill2;
                    compSkillBox3.SelectedItem = (compendiumBox.SelectedItem as Persona).skill3;
                    compSkillBox4.SelectedItem = (compendiumBox.SelectedItem as Persona).skill4;
                    compSkillBox5.SelectedItem = (compendiumBox.SelectedItem as Persona).skill5;
                    compSkillBox6.SelectedItem = (compendiumBox.SelectedItem as Persona).skill6;
                    compSkillBox7.SelectedItem = (compendiumBox.SelectedItem as Persona).skill7;
                    compSkillBox8.SelectedItem = (compendiumBox.SelectedItem as Persona).skill8;
                    readyEvents = true;
                }
            }
        }

        private void nextPhaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                NextDayPhase = (byte)nextPhaseBox.SelectedIndex;
            }
        }

        private void compendiumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readyEvents)
            {
                bool exists = false;
                if (compendiumComboBox.SelectedIndex == 0)
                    compendiumBox.SelectedIndex = -1;
                else
                {
                    for (int i = 0; i < compendiumBox.Items.Count; i++)
                    {
                        if ((compendiumBox.Items[i] as Persona).id == (compendiumComboBox.SelectedItem as Item).ID)
                        {
                            exists = true;
                            compendiumBox.SelectedIndex = i;
                            compendiumBox.ScrollIntoView(compendiumBox.SelectedItem);
                        }
                    }
                    if (!exists)
                    {
                        AddCompendiumItem(new Persona(true, 0, (compendiumComboBox.SelectedItem as Item).ID, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1));
                        compendiumBox.SelectedIndex = compendiumBox.Items.Count - 1;
                        compendiumBox.ScrollIntoView(compendiumBox.SelectedItem);
                    }
                }
            }
        }

        private void deleteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            compendiumBox.Items.Clear();
            compendiumBox.SelectedIndex = -1;
        }

        private void xpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(xpBox.Text, out result))
                    slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp = result;
                else
                    xpBox.Text = slots[member.SelectedIndex][personaSlot.SelectedIndex].totalxp.ToString();
            }
        }

        private void nextDayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte result = 0;

            if (!byte.TryParse(nextDayBox.Text, out result))
            {
                int result2 = 0;
                if (int.TryParse(dayBox.Text, out result2))
                {
                    if (result2 > 255)
                        MessageBox.Show(this, "Max is 255.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (result2 < 0)
                        MessageBox.Show(this, "Day number can't be negative.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NextDay = result;
                    return;
                }
                MessageBox.Show(this, "Somehow you've managed to input something that's not a number, good job buddy.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else NextDay = result;
        }

        private void dayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            byte result = 0;

            if (!byte.TryParse(dayBox.Text, out result))
            {
                int result2 = 0;
                if (int.TryParse(dayBox.Text, out result2))
                {
                    if (result2 > 255)
                        MessageBox.Show(this, "Max is 255.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (result2 < 0)
                        MessageBox.Show(this, "Day number can't be negative.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Day = result;
                    return;
                }
                MessageBox.Show(this, "Somehow you've managed to input something that's not a number, good job buddy.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else Day = result;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                OpenFile(File.OpenRead(filename = files[0]));
                Title = originalTitle + " - " + filename;
            }
        }

        private void courageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[0] = StatusLevelToPoints(0, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void knowledgeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[1] = StatusLevelToPoints(1, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void expressionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[4] = StatusLevelToPoints(4, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void understandingBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[3] = StatusLevelToPoints(3, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void diligenceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            socialStats[2] = StatusLevelToPoints(2, (byte)((e.Source as ComboBox).SelectedIndex + 1));
        }

        private void mcXpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (readyEvents)
            {
                uint result = 0;
                if (uint.TryParse(mcXpBox.Text, out result))
                    mcTotalXp = result;
                else
                    mcXpBox.Text = mcTotalXp.ToString();
            }
        }

        private void calcXp_Click(object sender, RoutedEventArgs e)
        {
            byte level = mcLevel;
            uint xp = (uint)(((uint)Math.Pow(level, 4) + 4 * (uint)Math.Pow(level, 3) + 53 * (uint)Math.Pow(level, 2) - 58 * level) / 10);
            mcXpBox.Text = xp.ToString();
        }

        private void calcXp_Copy_Click(object sender, RoutedEventArgs e)
        {
            byte level = slots[member.SelectedIndex][personaSlot.SelectedIndex].level;
            uint xp = (uint)(((uint)Math.Pow(level, 4) + 4 * (uint)Math.Pow(level, 3) + 53 * (uint)Math.Pow(level, 2) - 58 * level) / 10);
            xpBox.Text = xp.ToString();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Binary files (*.bin)|*.bin";
            if (d.ShowDialog() == true)
            {
                OpenFile(File.OpenRead(filename = d.FileName));
                Title = originalTitle + " - " + d.SafeFileName;
            }
        }
    }
}
