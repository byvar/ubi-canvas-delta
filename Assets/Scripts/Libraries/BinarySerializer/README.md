# BinarySerializer
BinarySerializer is a library for reading/writing from/to binary data.

* Allows binary files to be read/written using C# classes, thus making it easy to work with the data
* Adds an abstraction layer to the serialization process, allowing the same code to be reused for both reading and writing as well as other usages such as for logging and editors
* Support for XOR encryption, checksum checks, encoding (such as compression), pointers and more
* Serialization logging which logs all serialized values to a generic source where the address of each value can be seen

# Usage

## Context
In order to serialize data from/to a file or other source a `Context` needs to be created.
```cs
using Context context = new Context(basePath);
```
The base path passed in will be used as the base path for all files added to the context. This is useful when serializing multiple files which depend on each other using relative paths. Alternatively an empty string can be passed in to the context when all paths should be absolute.

## Binary File
A `BinaryFile` needs to be added for each file source to use in the context.
```cs
context.AddFile(new LinearSerializedFile(context, relativeFilePath));
```
There are different `BinaryFile` types to use, such as `LinearSerializedFile`, `MemoryMappedFile` and `StreamFile`. Additional ones can be created by inheriting from `BinaryFile`.

## Serializable objects
For each data object to serialize from/to you will need a matching class inheriting from `BinarySerializable`. This ensures the class can handle the data serialization.
```cs
public class FileData : BinarySerializable
{
    public int FirstValue { get; set; }
    public byte SecondValue { get; set; }

    public override void SerializeImpl(SerializerObject s)
    {
        FirstValue = s.Serialize<int>(FirstValue, name: nameof(FirstValue));
        SecondValue = s.Serialize<byte>(SecondValue, name: nameof(SecondValue));
    }
}
```

The `SerializerObject` has many helpful methods, such as `DoAt()` for following a pointer and serializing the data it points to.
```cs
DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));
Data = s.DoAt(DataPointer, () => s.Serialize<byte>(Data, name: nameof(Data)));
```

## Read/write
To read/write from/to files the `FileFactory` class can be used.
```cs
FileData data = FileFactory.Read<FileData>(relativeFilePath, context);
```

# Documentation

## Context
The `Context` is the most important object, required for the majority of features in the library. If you're serializing multiple files which all get memory mapped to different locations with pointers pointing between them then the context will manage that. Each file which is to be serialized has to first be registered in the context. A context is disposable and should be disposed when not used anymore to make sure all open files and other streams are correctly closed.

### Logging
By including a `ISerializerLog` in the context the serialization can be logged to a source such as a file. Here's an example of how the log will be structured:
```
(R) ROM.gba|0x08000000[0x00000000]:  (Object: BinarySerializer.Ray1.GBA_ROM) ROM.gba
(R) ROM.gba|0x081539A4[0x001539A4]:    (Byte[12]) WorldLevelOffsetTable: 00 00 16 28 35 42 4E 00 52 00 00 00
(R) ROM.gba|0x0835F8E0[0x0035F8E0]:    (Object: BinarySerializer.Ray1.GBA_EventGraphicsData) DES_Ray
(R) ROM.gba|0x0835F8E0[0x0035F8E0]:      (Pointer32) ImageBufferPointer: ROM.gba|0x0825CCAC[0x0025CCAC]
(R) ROM.gba|0x0835F8E4[0x0035F8E4]:      (UInt32) ImageBufferSize: 27456
(R) ROM.gba|0x0835F8E8[0x0035F8E8]:      (Pointer32) SpritesPointer: ROM.gba|0x0829085C[0x0029085C]
(R) ROM.gba|0x0835F8EC[0x0035F8EC]:      (UInt32) SpritesLength: 1908
(R) ROM.gba|0x0835F8F0[0x0035F8F0]:      (Pointer32) ETAsPointer: ROM.gba|0x0832D180[0x0032D180]
(R) ROM.gba|0x0835F8F4[0x0035F8F4]:      (UInt32) ETAsCount: 1
(R) ROM.gba|0x0835F8F8[0x0035F8F8]:      (Pointer32) AnimationsPointer: ROM.gba|0x0835F28C[0x0035F28C]
(R) ROM.gba|0x0835F8FC[0x0035F8FC]:      (UInt32) AnimationsCount: 135
(R) ROM.gba|0x0825CCAC[0x0025CCAC]:      (Byte[27456]) ImageBuffer: 00 00 00 00 00 00 00 00 00 00 00 00 44 44 04 00 
                                                                    00 00 00 00 00 00 00 40 44 44 14 00 00 00 00 00 
                                                                    00 00 00 00 FF FE FE FE 01 00 00 00 00 00 00 00 
                                                                    00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
                                                                    00 00 00 40 00 00 00 44 00 00 00 49 00 00 00 00 
                                                                    00 00 00 40 00 00 00 44 00 00 94 46 44 94 16 99 
                                                                    44 46 91 44 19 44 41 44 44 11 44 44 00 00 00 90 
                                                                    00 00 00 AB 00 00 B0 B5 00 00 50 B5 00 00 50 B5 
                                                                    00 00 50 B5 00 00 50 B7 00 00 70 B7 99 11 99 99 
                                                                    BB 93 99 99 BA 3B 99 39 AA BB 99 99 AA BB 99 99 ...
```

### Settings
A context can include settings used by serializable objects to determine how the data should be parsed. There are two types of setting. The first is the standard setting, `ISerializerSettings`, which determines general aspects of the serialization process, such as object caching. For more specific settings the additional settings can be used. This is a container where multiple specific settings can be included. Use `AddSettings<T>(T settings)` and `GetSettings<T>()` to add and retrieve the additional settings based on the type.

### Storage
In some cases data might need to be shared between multiple objects. There are several ways of doing this, such as using `OnPreSerialize`. Another way is to use the context's simple storage feature. It allows any object to be stored and identified by the key.

### Pre-Defined Pointers
For some binary data the only references to certain data is hard-coded in the code. For these situations these addresses can be pre-defined in the context and retrieved as pointers. The most common usage of this is to have an enum defining the available pointers for the data being parsed. For example, if a GBA ROM is being parsed the addresses for the data will be different depending on the regional version of the game. When the context is created the correct addresses should be specified and matched up with the correct names (or enum values) so that when the data gets parsed the correct pointer can be retrieved.
```cs
// Add the pointers to the context
context.AddPreDefinedPointers(new Dictionary<DefinedPointerEnum, long>()
{
    [DefinedPointerEnum.HardCodedData] = 0x08267940,
});

// Get a pointer
public override void SerializeImpl(SerializerObject s)
{
    // Retrieving the pointer from the serializer object will set it to use the current BinaryFile
    Pointer pointer = s.GetPreDefinedPointer(DefinedPointerEnum.HardCodedData);

    // Get the pointer directly from the context in order to manually specify the BinaryFile
    pointer = s.Context.GetPreDefinedPointer(DefinedPointer.MenuPack, Offset.File);
}
```

## SerializerObject
The `SerializerObject` is resposible for serializing the data. The most common variants are `BinaryDeserializer` for reading and `BinarySerializer` for writing. Other serializer objects can be created for other purposes.

The serialize object is most commonly used within a serializable class, inheriting from `BinarySerializable`.
```cs
public class FileData : BinarySerializable
{
    public override void SerializeImpl(SerializerObject s)
    {
        // Serialization code goes here
    }
}
```

Within a serializer object are multiple methods used when serializing the data.

### Values and objects
There are two common methods available for serializing data, `Serialize` and `SerializeObject`. The first one is used for serializing values, such as integers and booleans, while the second one is for objects. These objects must inherit from `BinarySerializable`. When serializing data you usually write it out like this, ensuring that the current value is captured and correctly updated:
```cs
MyIntValue = s.Serialize<int>(MyIntValue, name: nameof(MyIntValue));
```
When serializing an object there might be properties you want to set before serializing, such as passing in a reference to a another object or setting a flag. These properties will usually by convention have their names prefixed with `Pre_` and appear before the serializable properties. Setting them can be done using the `OnPreSerialize` action.
```cs
DDSData = s.SerializeObject<DDS>(DDSData, onPreSerialize: x => x.Pre_SkipHeader = true, name: nameof(DDSData));
```

### Structs and non-serializable classes
In some cases it is more convenient to serialize data as C# structs instead. For this `SerializeInto` can be used.
```cs
Tile = s.SerializeInto<MapTile>(Tile, MapTile.SerializeInto_Regular, name: nameof(Tile));
```
A func is then passed in which handles the serialization. In the example above it has been implemented like this:
```cs
public static SerializeInto<MapTile> SerializeInto_Regular = (s, x) =>
{
    s.DoBits<ushort>(b =>
    {
        int tileIndex = b.SerializeBits<int>(x.TileIndex, 10, name: nameof(TileIndex));
        bool flipX = b.SerializeBits<bool>(x.FlipX, 1, name: nameof(FlipX));
        bool flipY = b.SerializeBits<bool>(x.FlipY, 1, name: nameof(FlipY));
        byte paletteIndex = b.SerializeBits<byte>(x.PaletteIndex, 4, name: nameof(PaletteIndex));

        x = new MapTile(tileIndex, flipX, flipY, paletteIndex);
    });

    return x;
};
```
`SerializeInto` can also be used on normal classes which do not inherit from `BinarySerializable`, such as if they're from a referenced library.

### Strings
Strings are special cases as their lengths can vary. Usually a string either has a pre-defined length or is null terminated. If a string is serialized as a value using `Serialize<string>` then it will be treated as null-terminated. In order to parse a string with a pre-defined length, and also be able to specify an encoding, then `SerializeString` should be used.
```cs
MyStringValue = s.SerializeString(MyStringValue, length: 9, encoding: Encoding.Unicode, name: nameof(MyStringValue));
```
The default encoding to use can be specified in the serializer settings. If none are specified it will default to `UTF8`.

### Arrays
Serializing arrays can be done similarily to values and objects with the biggest difference being that a length has to be specified.
```cs
Values = s.SerializeArray<short>(Values, ValuesCount, name: nameof(Values));
```
In some cases the length of the array is not known until a terminator value is found. For these cases the `SerializeArrayUntil` method can be used.
```cs
Values = s.SerializeArrayUntil<short>(Values, x => x == -1, () => -1, name: nameof(Values));
```
If the array is nested into multiple arrays a combination of `InitializeArray` and `DoArray` can be used.
```cs
NestedArray = s.InitializeArray(NestedArray, 32);
s.DoArray(NestedArray, (x, name) => s.SerializeObjectArray<DDS>(x, 5, name: name), name: nameof(NestedArray));
```

### Pointers
A `Pointer` is a special type of object which holds an address value along with the BinaryFile it points to. They are usually created through serialization but can also be manually created.
```cs
// Serialize a 32-bit address as a pointer
DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));

// Optionally a size can be specified
DataPointer = s.SerializePointer(DataPointer, size: PointerSize.Pointer64, name: nameof(DataPointer));

// If the address is relative to another address, such as the offset of the current data, then an anchor can be used
DataPointer = s.SerializePointer(DataPointer, anchor: Offset, name: nameof(DataPointer));
```
Using a pointer you can serialize data at different locations, such as data being referenced by a some other data.
```cs
s.DoAt(DataPointer, () =>
{
    // Any code in here will be serialized starting from DataPointer
});
```

### Encoded
For encoded data, such as if it's compressed, an encoder can be specified.
```cs
s.DoEncoded(new BytePairEncoder(), () =>
{
    // Any code in here will be serialized using the decoded data
});
```

### XOR
```cs
s.DoProcessed(new Xor8Processor(xorKey), () =>
{
    // Any code here will have the data xored using the specified key
});
```

### Checksum
```cs
s.DoProcessed(new Checksum8Processor(), p =>
{
    // This defines where the checksum value is serialized (usually before or after the data)
    p.Serialize<byte>(s, "Checksum");

    // Any code here will have its data included as part of the checksum
});
```

### Bit Fields
```cs
s.DoBits<ushort>(b =>
{
    TX = b.SerializeBits<byte>(TX, 4, name: nameof(TX));
    TY = b.SerializeBits<byte>(TY, 1, name: nameof(TY));
    ABR = b.SerializeBits<byte>(ABR, 2, name: nameof(ABR));
    TP = b.SerializeBits<TexturePageTP>(TP, 2, name: nameof(TP));
});
```

## BinaryFile
Each file or other data source being used in the context needs to be added as a BinaryFile. There are two types, `PhysicalFile` for data with a physical file source and `VirtualFile` for data usually stored in memory.