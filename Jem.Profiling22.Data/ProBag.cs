namespace Jem.Profiling22.Data;

using PropertyChanged;

using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

public class ProBag : IdNamed
{
    /// <summary>These are groups of profiles for a common type of document, such as bank statements.</summary>
    public ProGroups Groups { get; set; } = new();

    /// <summary>These are keywords to use to skip specific files.</summary>
    public ProSkipables Skipables { get; set; } = new();

    public ProStamps Stamps { get; set; } = new();
}
