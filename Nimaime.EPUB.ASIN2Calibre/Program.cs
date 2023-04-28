using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimaime.EPUB.ASIN2Calibre
{
	internal class Program
	{
		static void Main(string[] args)
		{
			foreach (string epubFileName in args)
			{
				string extractedFolder = Path.Combine(Path.GetDirectoryName(epubFileName), Path.GetFileNameWithoutExtension(epubFileName));
				ZipFile.ExtractToDirectory(epubFileName, extractedFolder);
				string opfFileName = Path.Combine(extractedFolder, "OEBPS", "content.opf");
				string opfText = File.ReadAllText(opfFileName);
				string asin = opfText.Substring(opfText.IndexOf("<dc:identifier id=\"ASIN\">") + 25, 10);
				Console.WriteLine(asin);
				try
				{
					DirectoryInfo di = new DirectoryInfo(extractedFolder);
					di.Delete(true);
				}
				catch (Exception)
				{
				}
				File.Move(epubFileName, epubFileName.Replace(" ", ""));
				string epubFileNameWithoutExtension = epubFileName.Replace(" ", "");
				Console.WriteLine(CMD(string.Format("ebook-meta --identifier=AMAZON_JP:{0} {1}", asin, epubFileNameWithoutExtension)));
			}
		}

		/// <summary>
		/// 执行命令行
		/// </summary>
		/// <param name="command">命令</param>
		/// <returns></returns>
		static string CMD(string command)
		{
			Process p = new Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();
			p.StandardInput.WriteLine(command);
			p.StandardInput.WriteLine("exit");
			p.StandardInput.AutoFlush = true;
			string strOuput = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			p.Close();
			return strOuput;
		}
	}
}
