using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Graphic3._0.ViewModel;

internal class MainMenuViewModel
{
    public static List<SaveJob> GetJobs()
    {
        return StateJsonReader.GetInstance().GetJobs(true);
    }
}
