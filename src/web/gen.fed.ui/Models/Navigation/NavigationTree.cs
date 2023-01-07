using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen.fed.application.Models.Navigation
{
    public class NavigationTree
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<NavigationBranch>? Branches { get; set; }
    }
}
