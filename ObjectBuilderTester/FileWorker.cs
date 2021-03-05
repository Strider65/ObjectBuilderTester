using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ObjectBuilderTester
{
    class FileWorker
    {
        public string FileToUse { get; set; }
        private int CurrentLine;
        private Boolean EndOfFile;
        private string[] lines;
        private int TotalLines;


        public FileWorker()
        {
            Init();
        }

        private void Init()
        {
            CurrentLine = 0;
            FileToUse = "";
            EndOfFile = false;
            TotalLines = 0;
        }

        public Boolean EOF()
        {
            return EndOfFile;
        }
        public void ReadFile()
        {
            lines = System.IO.File.ReadAllLines(FileToUse);
            TotalLines = lines.Length;
            if (TotalLines == 0) { EndOfFile = true; }
            //MessageBox.Show(TotalLines.ToString(),"");
        }

        public string ReadLine()
        {
            string Tempstring = "";
            if(!EndOfFile)
            {
                Tempstring = lines[CurrentLine];
                CurrentLine++;
                if (CurrentLine == TotalLines) { EndOfFile = true; }                
            }
            return (Tempstring);
        }

        public void Reset() { Init(); }
    }
}
