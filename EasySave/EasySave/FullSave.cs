using EasySave.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    public class FullSave : SaveJob
    {
        //ATTRIBUTES

        //CONTRUCTOR
        public FullSave(string name, string sourcePath, string targetPath, bool checkBusinessSoftwares = false) : base(name, sourcePath, targetPath, checkBusinessSoftwares)
        {
        }

        //METHODS

        public override bool Save()
        {
            string saveTargetPath = Path.Combine(TargetPath, ("FullSave_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss")));
            CreateFullSave(SourcePath, saveTargetPath);
            return true;
        }

        public override bool RestoreSave()
        {
            //TODO faire le chemin inverse, supprimer tout dans sourcePath et copier tout de la derniere save
            //TODO, creer une fonction de recuperation de la dernkiere full save
            return true;
        }

        public override string ToString()
        {
            return base.ToString() + ", Total Save";
        }
    }
}
