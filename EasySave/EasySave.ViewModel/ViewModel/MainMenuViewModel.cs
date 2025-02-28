using EasySave.Utils.JobStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Graphic3._0.ViewModel;

public class MainMenuViewModel
{
    private static List<SaveJob> saveJobs = null;
    public List<SaveJob> GetJobs()
    {
        saveJobs = SaveJob.Instances;
        return saveJobs;
    }
}
