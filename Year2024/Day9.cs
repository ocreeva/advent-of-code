namespace Moyba.AdventOfCode.Year2024
{
    public class Day9(string[] _data) : IPuzzle
    {
        private readonly int[] _diskMap = _data
            .Single()
            .Select(_ => _ - '0')
            .ToArray();

        [PartOne("6283170117911")]
        [PartTwo("6307653242596")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var disk = _diskMap
                .SelectMany((_, i) => Enumerable.Repeat(_GetFileId(i), _))
                .ToArray();

            for (int i = 0, j = disk.Length - 1; i < j; i++)
            {
                if (disk[i] != -1) continue;

                while (disk[j] == -1) j--;
                (disk[i], disk[j]) = (disk[j], disk[i]);
            }

            var checksum = 0L;
            for (var i = 0; disk[i] != -1; i++)
            {
                checksum += i * disk[i];
            }

            yield return $"{checksum}";

            var impossibleLength = 10;
            var defrag = new LinkedList<(int id, int length)>(_diskMap.Select((_, i) => (_GetFileId(i), _)));
            for (var block = defrag.Last; block != null; block = block.Previous)
            {
                if (block.Value.id == -1) continue;
                if (block.Value.length >= impossibleLength) continue;

                var foundFree = false;
                for (var free = defrag.First; free != null && free != block; free = free.Next)
                {
                    if (free.Value.id != -1) continue;
                    if (free.Value.length < block.Value.length) continue;

                    defrag.AddBefore(free, new LinkedListNode<(int id, int length)>(block.Value));
                    block.Value = (-1, block.Value.length);

                    if (free.Value.length == block.Value.length) defrag.Remove(free);
                    else free.Value = (free.Value.id, free.Value.length - block.Value.length);

                    foundFree = true;
                    break;
                }

                if (!foundFree) impossibleLength = block.Value.length;
            }

            checksum = 0L;
            var index = 0;
            for (var node = defrag.First; node != null; node = node.Next)
            {
                if (node.Value.id == -1)
                {
                    index += node.Value.length;
                    continue;
                }

                for (var i = 0; i < node.Value.length; i++)
                {
                    checksum += index++ * node.Value.id;
                }
            }

            yield return $"{checksum}";

            await Task.CompletedTask;
        }

        private static int _GetFileId(int index)
            => index % 2 == 0 ? index >> 1 : -1;
    }
}
