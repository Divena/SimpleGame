using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Reflection.Emit;

namespace WpfApplication2
{
    class Myserializer
    {
        private Stream _stream;
        private BinaryWriter _writer;
        private BinaryReader _reader;

        public void Serialize(Stream stream, object graph)
        {
            _stream = stream;
            _writer = new BinaryWriter(stream);
            Serialize(graph);
        }
        private void Serialize(object obj)
        {
            if (obj == null)
            {
                _writer.Write((byte)Codes.Null);
                return;
            }

            Type objType = obj.GetType();

            if (objType == typeof(Int32))
            {
                _writer.Write((byte)Codes.Int32);
                _writer.Write((Int32)obj);
                return;
            }
            if (objType == typeof(Int16))
            {
                _writer.Write((byte)Codes.Int16);
                _writer.Write((Int16)obj);
                return;
            }
            if (objType == typeof(Int64))
            {
                _writer.Write((byte)Codes.Int64);
                _writer.Write((Int64)obj);
                return;
            }
            if (objType == typeof(UInt32))
            {
                _writer.Write((byte)Codes.UInt32);
                _writer.Write((UInt32)obj);
                return;
            }
            if (objType == typeof(UInt16))
            {
                _writer.Write((byte)Codes.UInt16);
                _writer.Write((UInt16)obj);
                return;
            }
            if (objType == typeof(UInt64))
            {
                _writer.Write((byte)Codes.UInt64);
                _writer.Write((UInt64)obj);
                return;
            }
            if (objType == typeof(Boolean))
            {
                _writer.Write((byte)Codes.Boolean);
                _writer.Write((Boolean)obj);
                return;
            }
            if (objType == typeof(String))
            {
                _writer.Write((byte)Codes.String);
                _writer.Write((String)obj);
                return;
            }
            if (objType == typeof(Byte))
            {
                _writer.Write((byte)Codes.Byte);
                _writer.Write((Byte)obj);
                return;
            }
            if (objType == typeof(Double))
            {
                _writer.Write((byte)Codes.Double);
                _writer.Write((Double)obj);
                return;
            }
            if (objType == typeof(Single))
            {
                _writer.Write((byte)Codes.Single);
                _writer.Write((Single)obj);
                return;
            }
            if (objType == typeof(Decimal))
            {
                _writer.Write((byte)Codes.Decimal);
                _writer.Write((Decimal)obj);
                return;
            }
            if (objType == typeof(char))
            {
                _writer.Write((byte)Codes.Char);
                _writer.Write((char)obj);
                return;
            }
            if (objType == typeof(SByte))
            {
                _writer.Write((byte)Codes.SByte);
                _writer.Write((SByte)obj);
                return;
            }
            if (objType.IsPrimitive)
                throw new ArgumentException("Primitiv " + objType.ToString() + " type which is not excepted", "obj");

            if (objType == typeof(byte[]))
            {
                _writer.Write((byte)Codes.ByteArray);
                _writer.Write((byte[])obj);
                return;
            }
            if (objType.IsArray)
            {
                WriteArray(obj, objType);
                return;
            }
            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                WriteDictionary(obj, objType);
                return;
            }
            if (typeof(IList).IsAssignableFrom(objType))
            {
                WriteList(obj, objType);
                return;
            }

            WriteCustomObject(obj, objType);
        }
        public object Deserialize(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
            return Deserialize();
        }
        private object Deserialize()
        {
            byte code = _reader.ReadByte();

            if (code == (byte)Codes.Null)
                return null;
            if (code == (byte)Codes.Int32)
                return _reader.ReadInt32();
            if (code == (byte)Codes.Int16)
                return _reader.ReadInt16();
            if (code == (byte)Codes.Int64)
                return _reader.ReadInt64();
            if (code == (byte)Codes.String)
                return _reader.ReadString();
            if (code == (byte)Codes.Byte)
                return _reader.ReadByte();
            if (code == (byte)Codes.ByteArray)
                return _reader.ReadBytes(_reader.ReadInt32());
            if (code == (byte)Codes.Char)
                return _reader.ReadChar();
            if (code == (byte)Codes.SByte)
                return _reader.ReadSByte();
            if (code == (byte)Codes.Boolean)
                return _reader.ReadBoolean();
            if (code == (byte)Codes.Array)
                return ReadArray();
            if (code == (byte)Codes.List)
                return ReadList();
            if (code == (byte)Codes.Dictionaty)
                return ReadDictionary();
            if (code == (byte)Codes.CustomObject)
                return ReadCustomObject();
            if (code == (byte)Codes.UInt32)
                return _reader.ReadUInt32();
            if (code == (byte)Codes.UInt16)
                return _reader.ReadUInt16();
            if (code == (byte)Codes.UInt64)
                return _reader.ReadUInt64();
            if (code == (byte)Codes.Decimal)
                return _reader.ReadDecimal();
            if (code == (byte)Codes.Double)
                return _reader.ReadDouble();
            if (code == (byte)Codes.Single)
                return _reader.ReadSingle();
            throw new ArgumentException("Unknown  code " + code);
        }
        private void WriteCustomObject(object obj, Type objType)
        {
            _writer.Write((byte)Codes.CustomObject);
            WriteType(objType);
            long pos1 = _stream.Position;
            _writer.Write(0);
            //long pos2 = _stream.Position;
            //_writer.Write(0);
            TypedReference typedReference = __makeref(obj);
            foreach (FieldInfo field in objType.GetFields())
            {
                if (field.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length != 0)
                    continue;
                _writer.Write((string)field.Name);
                var _field = field.GetValueDirect(typedReference);
                Serialize(_field);
            }
            foreach (PropertyInfo field in objType.GetProperties())
            {
                if (field.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length != 0)
                    continue;
                _writer.Write((string)field.Name);
                var _field = field.GetValue(obj);
                Serialize(_field);
            }
            
            long end = _stream.Position;
            _stream.Position = pos1;
            _writer.Write((int)(end - pos1));
            _stream.Position = end;
           /* foreach (EventInfo field in objType.GetEvents())
            {
                if (field.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length != 0)
                    continue;
                _writer.Write(field.AddMethod.Name);
                WriteType(field.EventHandlerType);
            }
            end = _stream.Position;
            _stream.Position = pos2;
            _writer.Write((int)(end - pos2));
            _stream.Position = end;*/
        }

        private void WriteArray(object obj, Type objType)
        {
            _writer.Write((byte)Codes.Array);
            Array arr = (Array)obj;
            WriteType(arr.GetValue(0).GetType());
            _writer.Write(arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                Serialize(arr.GetValue(i));
            }
        }

        private void WriteDictionary(object obj, Type objType)
        {
            _writer.Write((byte)Codes.Dictionaty);
            WriteType(objType);
            _writer.Write(((IDictionary)obj).Count);

            foreach (DictionaryEntry entry in (IDictionary)obj)
            {
                Serialize(entry.Key);
                Serialize(entry.Value);
            }
        }

        private void WriteList(object obj, Type objType)
        {
            _writer.Write((byte)Codes.List);
            WriteType(objType);
            _writer.Write(((IList)obj).Count);
            IList list = (IList)obj;
            for (int i = 0; i < list.Count; i++)
            {
                Serialize(list[i]);
            }
        }
        
        private void WriteType(Type type)
        {
            string typeName = type.FullName + ", " + type.Assembly.FullName;
            _writer.Write(typeName);
        }

        private object ReadCustomObject()
        {
            Type type = ReadType();
            long pos = _stream.Position;
            int length1 = _reader.ReadInt32();
            long endPos = _stream.Position + length1 - sizeof(int);
           // int length2 = _reader.ReadInt32();
            //long endPos2 = _stream.Position + length1 - sizeof(int);
            var obj = Activator.CreateInstance(type);
            Convert.ChangeType(obj, type);
            while (_stream.Position != endPos)
            {
                string t = _reader.ReadString();
                foreach (FieldInfo field in type.GetFields())
                {
                    if (field.Name == t)
                    {
                        var _field = Activator.CreateInstance(field.FieldType);
                        _field = Deserialize();
                        field.SetValue(obj, _field);
                    }
                }
                foreach (PropertyInfo field in type.GetProperties())
                {
                    if (field.Name == t)
                    {
                        object _field = new object();
                        _field = Deserialize();
                        field.SetValue(obj, _field);
                    }
                }

            }
  /*           while (_stream.Position != endPos2)
            {
                foreach (EventInfo field in type.GetEvents())
                {
                    string Name_Method = _reader.ReadString();
                    Type type_del = ReadType();
                    field.AddEventHandler(obj, type.GetMethod(Name_Method).CreateDelegate(type_del));
                }

            }*/
            return obj;
        }

        private object ReadArray()
        {
            Type type = ReadType();
            int length = _reader.ReadInt32();
            Array arr = Array.CreateInstance(type, length);
            for (int i = 0; i < length; i++)
            {
                arr.SetValue(Deserialize(), i);
            }
            return arr;
        }

         private object ReadDictionary()
        {
            Type type = ReadType();
            int length = _reader.ReadInt32();

            IDictionary dic = (IDictionary)type.Assembly.CreateInstance(type.FullName);
            for (int i = 0; i < length; i++)
            {
                object key = Deserialize();
                object val = Deserialize();
                dic.Add(key, val);
            }

            return dic;
        }

        private object ReadList()
        {
            Type type = ReadType();
            int length = _reader.ReadInt32();

            IList collection = (IList)type.Assembly.CreateInstance(type.FullName);
            for (int i = 0; i < length; i++)
            {
                collection.Add(Deserialize());
            }
            return collection;
        }
        
        private Type ReadType()
        {
            string typeStr = _reader.ReadString();
            Type type;
            type = Type.GetType(typeStr);
            return type;
        }    
    }


    enum Codes : byte
    {
        Null = 0,
        Int32,
        Int16,
        Int64,
        String,
        CustomObject,
        Array,
        Dictionaty,
        List,
        UInt32,
        UInt16,
        UInt64,
        Byte,
        Boolean,
        Double,
        Single,
        Decimal,
        ByteArray,
        Char,
        SByte
    }
}
 
