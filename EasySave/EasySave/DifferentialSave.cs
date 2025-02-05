﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    public class DifferentialSave : SaveJob
    {
        //ATTRIBUTES



        //CONTRUCTOR
        public DifferentialSave(string name, string sourcePath, string targetPath) : base(name, sourcePath, targetPath)
        {
        }

        //METHODS

        public override bool Save()
        {
            return true;
        }

        public override bool RestoreSave()
        {
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + ", Differential Save";
        }
    }
}
