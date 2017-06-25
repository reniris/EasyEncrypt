using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace EasyEncrypt
{
	/// <summary>
	/// 
	/// </summary>
	class Encrypt
	{
		/// <summary>
		/// ファイルを暗号化する
		/// </summary>
		/// <param name="out_path">暗号化するファイルのパス</param>
		/// <param name="data">暗号化するデータ</param>
		/// <param name="key">暗号化キー nullの場合は自動生成</param>
		/// <returns></returns>
		static public byte[] EncryptFile(string out_path, byte[] data, byte[] key = null)
		{
			using (var fs = new FileStream(out_path, FileMode.Create, FileAccess.Write))
			{
				return EncryptData(fs, data, key);
			}
		}

		/// <summary>
		/// データを暗号化してストリームに出力する
		/// </summary>
		/// <param name="out_stream">出力ストリーム</param>
		/// <param name="data">暗号化するデータ</param>
		/// <param name="key">暗号化キー nullの場合は自動生成</param>
		/// <returns></returns>
		static private byte[] EncryptData(Stream out_stream, byte[] data, byte[] key)
		{
			using (var aes = Aes.Create())
			{
				aes.BlockSize = 128;              // BlockSize = 16bytes
				aes.KeySize = 128;                // KeySize = 16bytes
				aes.Mode = CipherMode.CBC;        // CBC mode
				aes.Padding = PaddingMode.PKCS7;    // Padding mode is "PKCS7".

				// IV ( Initilization Vector ) は、自動生成する
				aes.GenerateIV();

				if (key == null)
					aes.GenerateKey();  //暗号化キーを自動生成
				else
					aes.Key = key;

				//Encryption interface.
				var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
				using (CryptoStream cse = new CryptoStream(out_stream, encryptor, CryptoStreamMode.Write))
				{
					out_stream.Write(aes.IV, 0, 16); // 先頭にIVを埋め込む
					cse.Write(data, 0, data.Length);
					cse.FlushFinalBlock();

					return aes.Key;
				}
			}
		}

		/// <summary>
		/// ファイルを複合化する
		/// </summary>
		/// <param name="path">複合化するファイルのパス</param>
		/// <param name="key">暗号化キー</param>
		/// <returns></returns>
		static public byte[] DecryptFile(string path, byte[] key)
		{
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return DecryptData(fs, key);
			}
		}

		/// <summary>
		/// データを複合化する
		/// </summary>
		/// <param name="encrypt_stream">暗号化された入力ストリーム</param>
		/// <param name="key">暗号化キー</param>
		/// <returns></returns>
		static private byte[] DecryptData(Stream encrypt_stream, byte[] key)
		{
			using (var aes = Aes.Create())
			{
				aes.BlockSize = 128;              // BlockSize = 16bytes
				aes.KeySize = 128;                // KeySize = 16bytes
				aes.Mode = CipherMode.CBC;        // CBC mode
				aes.Padding = PaddingMode.PKCS7;    // Padding mode is "PKCS7".

				// Initilization Vector
				byte[] iv = new byte[16];
				encrypt_stream.Read(iv, 0, 16);
				aes.IV = iv;

				aes.Key = key;

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