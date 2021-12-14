using UnityEngine;
using System.Collections;
using System;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using Ionic.Zip;
using System.Text;
using System.IO;

public class ZipUtil
{

	public static void Unzip (string zipFilePath, string location)
	{
		Directory.CreateDirectory (location);
		
		using (ZipFile zip = ZipFile.Read (zipFilePath)) {
			zip.ExtractAll (location, ExtractExistingFileAction.OverwriteSilently);
		}

	}

	public static void Zip (string zipFileName, params string[] files)
	{
		string path = Path.GetDirectoryName (zipFileName);
		Directory.CreateDirectory (path);
		
		using (ZipFile zip = new ZipFile()) {
			foreach (string file in files) {
				Debug.Log("fn = " + Path.GetFileName(file) + "   dir = " + Path.GetDirectoryName(file));
				zip.AddFile (Path.GetFileName(file), Path.GetDirectoryName(file));
			}
			zip.Save (zipFileName);
		}
	}
    public static string[] ZipDirectoryContents(string zipFileName, string directoryPath)
    {
        string[] files = System.IO.Directory.GetFiles(directoryPath, "*.*", System.IO.SearchOption.AllDirectories);
        string path = Path.GetDirectoryName(zipFileName);
        Directory.CreateDirectory(path);

        using (ZipFile zip = new ZipFile())
        {
            foreach (string file in files)
            {
                string relativeDirectory = Path.GetDirectoryName(file).Substring(directoryPath.Length);
                relativeDirectory = Path.GetFileName(relativeDirectory);
                zip.AddFile(file, relativeDirectory);
            }
            zip.Save(zipFileName);
        }
        return files;
    }

}
