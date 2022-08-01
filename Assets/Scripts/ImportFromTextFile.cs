using System.IO;
using TMPro;
using UnityEngine;

namespace RedundantTalismans
{
	public class ImportFromTextFile : MonoBehaviour
	{
		[SerializeField] private TMP_InputField filePathInputField;
		[SerializeField] private TMP_InputField talismanInputField;

		private const string FILE_KEY = "FilePathInput";

		private void Start()
		{
			filePathInputField.text = SaveLoader.Load(FILE_KEY);
		}

		public void Import()
		{
			string filePath = filePathInputField.text;
			if (!File.Exists(filePath)) return;
			
			SaveLoader.Save(FILE_KEY, filePath);
			string text = File.ReadAllText(filePath);
			talismanInputField.text = text;
		}
	}
}