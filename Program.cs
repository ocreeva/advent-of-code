using System.Reflection;
using Moyba.AdventOfCode;

var assembly = Assembly.GetExecutingAssembly();
var solutionTypesByYear = assembly.GetTypes()
    .Where(t => !t.IsAbstract && !t.IsInterface)
    .Where(t => t.IsAssignableTo(typeof(ISolution)))
    .Select(t => t.GetConstructor(Type.EmptyTypes)?.Invoke(null))
    .Cast<ISolution>()
    .GroupBy(s => s.Year)
    .OrderBy(g => g.Key);
foreach (var group in solutionTypesByYear)
{
    foreach (var solution in group.OrderBy(s => s.Day))
    {
        await solution.SolveAsync();
    }
}

//var puzzles = new Puzzles2022();
//await puzzles.SolveAsync();
