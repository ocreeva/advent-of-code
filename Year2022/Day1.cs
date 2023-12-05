using System.Collection.Generic;

namespace Moyba.AdventOfCode.Year2022
{
    public class Day1(string[] data) : IPuzzle
    {
        private readonly long[][] _elves = data.Cluster().Select(_ => _.Select(Int64.Parse).ToArray()).ToArray();

        private long _maxCalories;
        private long _maxCaloriesForThree;

        public Task ComputeAsync()
        {
            var calories = _elves.Select(_ => _.Sum()).OrderDescending().Take(3).ToArray();

            _maxCalories = calories[0];
            _maxCaloriesForThree = calories.Sum();

            return Task.CompletedTask;
        }

        [Solution("71924")]
        public string PartOne => $"{_maxCalories}";

        [Solution("210406")]
        public string PartTwo => $"{_maxCaloriesForThree}";
    }
}
