using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace EasyEncrypt
{
	class Encrypt
	{
		static public void EncryptFile(string out_path, byte[] data)
		{
			using (var fs = new FileStream(out_path, FileMode.Create, FileAccess.Write))
			{
				EncryptData(fs, data);
			}
		}

		static private void EncryptData(Stream out_stream, byte[] data)
		{
			using (var aes = Aes.Create())
			{
				aes.BlockSize = 128;              // BlockSize = 16bytes
				aes.KeySize = 128;                // KeySize = 16bytes
				aes.Mode = CipherMode.CBC;        // CBC mode
				aes.Padding = PaddingMode.PKCS7;    // Padding mode is "PKCS7".

				//キーを自動生成
				aes.GenerateKey();
				// IV ( Initilization Vector ) は、自動生成する
				aes.GenerateIV();

				//Encryption interface.
				var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
				using (CryptoStream cse = new CryptoStream(out_stream, encryptor, CryptoStreamMode.Write))
				{
					var xor_key = aes.Key.Select((k, i) => (byte)(k ^ aes.IV[i])).ToArray();
					out_stream.Write(xor_key, 0, 16);
					out_stream.Write(aes.IV, 0, 16); // 次にIVも埋め込む
					cse.Write(data, 0, data.Length);
					cse.FlushFinalBlock();
				}
			}
		}

		static public byte[] DecryptFile(string path)
		{
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return DecryptData(fs);
			}
		}

		static private byte[] DecryptData(Stream encrypt_stream)
		{
			using (var aes = Aes.Create())
			{
				aes.BlockSize = 128;              // BlockSize = 16bytes
				aes.KeySize = 128;                // KeySize = 16bytes
				aes.Mode = CipherMode.CBC;        // CBC mode
				aes.Padding = PaddingMode.PKCS7;    // Padding mode is "PKCS7".

				// XOR KEY
				byte[] xor_key = new byte[16];
				encrypt_stream.Read(xor_key, 0, 16);

				// Initilization Vector
				byte[] iv = new byte[16];
				encrypt_stream.Read(iv, 0, 16);
				aes.IV = iv;

				aes.Key = xor_key.Select((k, i) => (byte)(k ^ aes.IV[i])).ToArray();

				//Decryption interface.
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (CryptoStream cse = new CryptoStream(encrypt_stream, decryptor, CryptoStreamMode.Read))
				{
					//復号化されたデータを書き出す
					using (var ms = new MemoryStream())
					{
						byte[] buf = new byte[1024];
						int readLen;
						//復号化に失敗すると例外CryptographicExceptionが発生
						while ((readLen = cse.Read(buf, 0, buf.Length)) > 0)
						{
							ms.Write(buf, 0, readLen);
						}
						return ms.ToArray();
					}
				}
			}
		}
	}
}