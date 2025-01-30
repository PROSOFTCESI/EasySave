using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    internal class DifferentialSave : SaveJob
    {
        //ATTRIBUTES



        //CONTRUCTOR
        public DifferentialSave(string name, string sourcePath, string targetPath) : base(name, sourcePath, targetPath)
        {
        }

        //METHODS
        public override bool CreateSave(string name, string sourcePath, string targetPath)
        {
            return true;
        }

        public override bool UpdateSave(string name)
        {
            return true;
        }

        public override bool RestoreSave(string name)
        {
            return true;
        }

        public override bool DeleteSave(string name)
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
