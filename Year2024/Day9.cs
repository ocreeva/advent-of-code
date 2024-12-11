namespace Moyba.AdventOfCode.Year2024
{
    using Disk = LinkedList<(int id, int length)>;
    using Block = LinkedListNode<(int id, int length)>;

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

            var defrag = new Disk(_diskMap.Select((_, i) => (_GetFileId(i), _)));
            checksum = _DefragAndChecksum(defrag);

            yield return $"{checksum}";

            await Task.CompletedTask;
        }

        private class FreeBlock
        {
            private readonly LinkedListNode<FreeBlock>[] _lookups = new LinkedListNode<FreeBlock>[9];

            public FreeBlock(Block block, LinkedList<FreeBlock>[] freeBlocksByLength)
            {
                this.Block = block;

                for (var i = 0; i < block.Value.length; i++)
                {
                    _lookups[i] = freeBlocksByLength[i].AddLast(this);
                }
            }

            public Block Block { get; }

            public void Update()
            {
                for (var i = this.Block.Value.length; i < _lookups.Length; i++)
                {
                    var lookup = _lookups[i];
                    if (lookup == null) break;

                    lookup.List!.Remove(lookup);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    _lookups[i] = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
            }

            public void MakeUnavailable()
            {
                for (var i = 0; i < _lookups.Length; i++)
                {
                    var lookup = _lookups[i];
                    if (lookup == null) break;

                    lookup.List!.Remove(lookup);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    _lookups[i] = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
            }
        }

        private static long _DefragAndChecksum(Disk disk)
        {
            var freeBlocksByLength = Enumerable.Range(0, 9).Select(_ => new LinkedList<FreeBlock>()).ToArray();
            for (var block = disk.First; block != null; block = block.Next)
            {
                if (block.Value.id != -1) continue;

                new FreeBlock(block, freeBlocksByLength);
            }

            for (var block = disk.Last; block != null; block = block.Previous)
            {
                if (block.Value.id == -1)
                {
                    if (block.Value.length > 0) freeBlocksByLength[0].Last!.Value.MakeUnavailable();
                    continue;
                }

                var freeBlock = freeBlocksByLength[block.Value.length - 1].First?.Value;
                if (freeBlock == null) continue;

                disk.AddBefore(freeBlock.Block, new Block(block.Value));
                block.Value = (-1, block.Value.length);

                freeBlock.Block.Value = (freeBlock.Block.Value.id, freeBlock.Block.Value.length - block.Value.length);
                freeBlock.Update();
            }

            return _CalculateChecksum(disk);
        }

        private static long _CalculateChecksum(Disk disk)
        {
            var checksum = 0L;
            var index = 0;
            for (var node = disk.First; node != null; node = node.Next)
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

            return checksum;
        }

        private static int _GetFileId(int index)
            => index % 2 == 0 ? index >> 1 : -1;
    }
}
