using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    internal class FullSave : SaveJob
    {
        //ATTRIBUTES

        

        //CONTRUCTOR
        public FullSave(string name, string sourcePath, string targetPath) : base(name, sourcePath, targetPath)
        {
        }

        //METHODS

        public override bool Save()
        {
            FullSave(SourcePath, TargetPath);
            return true;
        }

        public override bool RestoreSave()
        {
            return true;
        }

        public override string ToString()
        {
            string str = "";

            return str;
        }
    }
}
