using MessagePack;
using System;
using System.IO;

namespace EasyEncrypt
{
    class Program
    {
        static void Main(string[] args)
		{
			var dir = AppContext.BaseDirectory;
			var fullpath = Path.Combine(dir, "save.data");
			Console.WriteLine(fullpath);

			var data = new GameData();
			data.Money = 6500;

			byte[] bytes = Encode(fullpath,data);
			Decode(fullpath);

			Console.ReadLine();
		}

		private static void Decode(string fullpath)
		{
			var bytes = Encrypt.DecryptFile(fullpath);

			var data2 = MessagePackSerializer.Deserialize<GameData>(bytes);

			var json = MessagePackSerializer.ToJson(bytes);
			Console.WriteLine(json);
		}

		private static byte[] Encode(string fullpath, GameData data)
		{
			var bytes = MessagePackSerializer.Serialize(data);
			Console.WriteLine(BitConverter.ToString(bytes));
			Encrypt.EncryptFile(fullpath, bytes);
			return bytes;
		}		
    }
}