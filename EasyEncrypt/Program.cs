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

			var key = Encode(fullpath, data);   //キーを自動生成して暗号化

			var data2 = Decode(fullpath, key);  //複合化
			data2.Money = 15000;

			var key2 = Encode(fullpath, data2, key);    //さっき生成されたキーで暗号化
			Decode(fullpath, key);  //複合化

			Console.ReadLine();
		}

		private static GameData Decode(string fullpath, byte[] key)
		{
			var bytes = Encrypt.DecryptFile(fullpath, key);

			var ret = MessagePackSerializer.Deserialize<GameData>(bytes);

			var json = MessagePackSerializer.ToJson(bytes);
			Console.WriteLine(json);

			return ret;
		}

		private static byte[] Encode(string fullpath, GameData data, byte[] enc_key = null)
		{
			var bytes = MessagePackSerializer.Serialize(data);
			Console.WriteLine(BitConverter.ToString(bytes));
			var gen_key = Encrypt.EncryptFile(fullpath, bytes, enc_key);

			return gen_key;
		}
	}
}