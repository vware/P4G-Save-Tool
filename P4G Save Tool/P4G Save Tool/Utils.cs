using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace P4G_Save_Tool
{
    public static class Utils
    {
        public static void Refresh(this UIElement uiElement)
        {
            Action EmptyDelegate = delegate () { };
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static Persona ReadPersona(this BinaryReader r)
        {
            bool exists = r.ReadBoolean();
            byte unknown0 = r.ReadByte();
            ushort id = r.ReadUInt16();
            byte level = r.ReadByte();

            r.BaseStream.Position += 3;
            uint totalxp = r.ReadUInt32();
            ushort unknown1 = 0;// r.ReadUInt16();
            byte unknown2 = 0;// r.ReadByte();
            ushort s1 = r.ReadUInt16();
            ushort s2 = r.ReadUInt16();
            ushort s3 = r.ReadUInt16();
            ushort s4 = r.ReadUInt16();
            ushort s5 = r.ReadUInt16();
            ushort s6 = r.ReadUInt16();
            ushort s7 = r.ReadUInt16();
            ushort s8 = r.ReadUInt16();
            byte st = r.ReadByte();
            byte ma = r.ReadByte();
            byte de = r.ReadByte();
            byte ag = r.ReadByte();
            byte lu = r.ReadByte();
            Persona p = new Persona(exists, unknown0, id, level, totalxp, unknown1, unknown2, s1, s2, s3, s4, s5, s6, s7, s8, st, ma, de, ag, lu);
            return p;
        }

        public static void WritePersona(this BinaryWriter w, Persona persona)
        {
            w.Write(persona.exists);
            w.Write(persona.unknown0);
            w.Write(persona.id);
            w.Write(persona.level);
            w.BaseStream.Position += 3;
            w.Write(persona.totalxp);
            //w.Write(persona.unknown1);
            //w.Write(persona.unknown2);
            w.Write(persona.skill1.ID);
            w.Write(persona.skill2.ID);
            w.Write(persona.skill3.ID);
            w.Write(persona.skill4.ID);
            w.Write(persona.skill5.ID);
            w.Write(persona.skill6.ID);
            w.Write(persona.skill7.ID);
            w.Write(persona.skill8.ID);
            w.Write(persona.st);
            w.Write(persona.ma);
            w.Write(persona.de);
            w.Write(persona.ag);
            w.Write(persona.lu);
        }

        public static string ReadJString(this BinaryReader r)
        {
            string str = "";
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = r.ReadUInt16();
                byte[] utfValue = new byte[2] { (byte)(((charValue & 0xFF00) >> 8) - 0x60), 0 };
                str += Encoding.Unicode.GetString(utfValue).TrimEnd(new char[] { ' ', (char)160 });
            }
            return str;
        }

        public static string ReadPString(this BinaryReader r)
        {
            string b = "";
            List<byte> raw = new List<byte>();
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = BitConverter.ToUInt16(BitConverter.GetBytes(r.ReadUInt16()).Reverse(), 0);
                if (charValue != 0)
                {
                    if (charValue > 33358 && charValue < 33402)
                        b += (char)((charValue - 33311));
                    else if (charValue > 33408 && charValue < 33435)
                        b += (char)((charValue - 33312));
                    else if (charValue > 33079)
                    {
                        b += (char)((charValue - 33047));
                    }
                }
                //else raw[i] = (byte)' ';
            }
            return b;// Encoding.UTF8.GetString(raw.ToArray()).TrimEnd(' ');
        }

        public static void WriteJString(this BinaryWriter w, string name)
        {
            const int stride = 2;
            byte[] rawString = Encoding.Unicode.GetBytes(name);
            for (int i = 0; i < 9; i++)
            {
                ushort charValue = 0;
                if (rawString.Length > (i * stride))
                {
                    if (rawString[i * stride] != 0xA0)
                    {
                        charValue = (ushort)((rawString[i * stride] + 0x60) << 8 | 0x80);
                        charValue = (ushort)((rawString[i * stride] + 0x60) << 8 | 0x80);
                    }
                }
                w.Write(charValue);
            }
        }

        public static void WritePString(this BinaryWriter w, string name)
        {
            byte[] raw = Encoding.Unicode.GetBytes(name);
            ushort charValue = 0;
            for (int i = 0; i < 9; i++)
            {
                charValue = 0;
                if (i < name.Length)
                {
                    if (name[i] >= '0' && name[i] <= 'Z')
                        charValue = (ushort)(33311 + name[i]);
                    else if (name[i] >= 'a' && name[i] <= 'z')
                        charValue = (ushort)(33312 + name[i]);
                    else if (name[i] >= '!')
                        charValue = (ushort)(33047 + name[i]);
                }
                w.Write(BitConverter.ToUInt16(BitConverter.GetBytes(charValue).Reverse(), 0));
            }
        }

        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static void WriteUInt16BE(this BinaryWriter binRdr, ushort u16)
        {
            var v = BitConverter.GetBytes(u16);
            binRdr.Write(BitConverter.ToUInt16(v.Reverse(), 0));
        }

        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }

        public static Item[] GetFromDatabase(string[] database, ushort start, ushort length)
        {
            Item[] res = new Item[length];
            for (ushort i = 0; i < length; i++)
            {
                ushort id = (ushort)(i + start);
                res[i] = new Item(database[id], id);
            }
            return res;
        }

        public static Persona[] GetPersonaFromDatabase(ushort start, ushort length)
        {
            Persona[] res = new Persona[length];
            for (ushort i = 0; i < length; i++)
            {
                ushort id = (ushort)(i + start);
                res[i] = new Persona(true, 0, id, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1);
            }
            return res;
        }

        public static Visual GetDescendantByType(this Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

    }
}
