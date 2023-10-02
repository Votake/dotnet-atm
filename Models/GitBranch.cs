using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Models;

public class GitBranch
{
    public GitBranch(string brachRegex = "") { regex = brachRegex; }

    public string regex { get; set; } = string.Empty;
    


}
