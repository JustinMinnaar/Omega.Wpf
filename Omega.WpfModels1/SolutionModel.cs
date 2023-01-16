using Jem.DocDatabaseLibrary1;

using System;

namespace Omega.WpfModels1;

public class SolutionModel : IdNamedModel
{
    public static SolutionModel From(DocSolution dbSolution)
    {
        return new SolutionModel { Id = dbSolution.Id, Name = dbSolution.Name };
    }
}