using System;
using System.IO;
using System.Text;
using UnityEngine;
using Application = UnityEngine.Application;

namespace RedundantTalismans
{
	public static class SaveLoader
	{
		private const string FILE_EXTENSION = ".txt",
			SUB_DIRECTORY_NAME = "SaveData",
			DIRECTORY_FORMAT = "{0}\\" + SUB_DIRECTORY_NAME,
			FILE_PATH_FORMAT = DIRECTORY_FORMAT + "\\{1}" + FILE_EXTENSION;
		
		public static void Save(string key, string value)
		{
			string filePath = GetFilePath(key);
			string directoryPath = GetDirectory();
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			
			try
			{
				using FileStream fs = File.Create(filePath);
				byte[] info = new UTF8Encoding(true).GetBytes(value);
				fs.Write(info, 0, info.Length);
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}

		public static string Load(string key)
		{
			string filePath = GetFilePath(key);
			return ReadFile(filePath);
		}

		private static string ReadFile(string filePath)
		{
			try
			{
				using StreamReader sr = File.OpenText(filePath);
				string text = sr.ReadToEnd();
				return text;
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}

			return null;
		}

		private static string GetDirectory() =>
			string.Format(DIRECTORY_FORMAT, Application.persistentDataPath);

		private static string GetFilePath(string key, bool local = false) =>
			string.Format(FILE_PATH_FORMAT, Application.persistentDataPath, key);
	}
}