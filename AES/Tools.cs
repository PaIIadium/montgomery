namespace AES
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Tools
    {
        private const int RowsCount = 4;
        private const int ColumnsCount = 4;
        private const int RoundsCount = 10;
        private const int BlockSize = RowsCount * ColumnsCount;

        public static List<Block> SplitIntoBlocks(List<byte> bytes)
        {
            
            Align(bytes, BlockSize);

            var blocks = new List<Block>();
            for (var i = 0; i < bytes.Count; i += BlockSize)
            {
                var flatBlock = bytes.GetRange(i, BlockSize);
                var block = new Block(flatBlock);
                blocks.Add(block);
            }

            return blocks;
        }
        
        private static void Align(List<byte> bytes, int length)
        {
            var residue = bytes.Count % length;
            if (residue != 0)
            {
                var remainingBitsCount = length - residue;
                var additionalBytes = new byte[remainingBitsCount];
                bytes.AddRange(additionalBytes);
            }
        }

        public static List<byte> UniteBlocks(List<Block> blocks)
        {
            var bytes = new List<byte>();
            foreach (var block in blocks)
            {
                bytes.AddRange(block.GetAllBytes());
            }

            return bytes;
        }
        
        public static List<Block> KeyExpansion(Block initialKey)
        {
            var roundKeys = new List<Block>(RoundsCount);

            var prevWord0 = initialKey.GetWord(0);
            var prevWord1 = initialKey.GetWord(1);
            var prevWord2 = initialKey.GetWord(2);
            var prevWord3 = initialKey.GetWord(3);
            
            for (var i = 1; i < RoundsCount + 1; i++)
            {
                var word0 = prevWord3.ToList();
                RotWord(word0);
                Boxes.SubBytes(word0);
                Boxes.XorWithRCon(word0, i);
                XorWords(word0, prevWord0);
                prevWord0 = word0;
                var word1 = word0.ToList();
                XorWords(word1, prevWord1);
                prevWord1 = word1;
                var word2 = word1.ToList();
                XorWords(word2, prevWord2);
                prevWord2 = word2;
                var word3 = word2.ToList();
                XorWords(word3, prevWord3);
                prevWord3 = word3;
                
                roundKeys.Add(MakeKeyFromWords(word0, word1, word2, word3));
            }

            return roundKeys;
        }

        private static void RotWord(List<byte> word)
        {
            (word[0], word[1]) = (word[1], word[0]);
            (word[1], word[2]) = (word[2], word[1]);
            (word[2], word[3]) = (word[3], word[2]);
        }

        private static void XorWords(List<byte> bytes1, List<byte> bytes2)
        {
            bytes1[0] ^= bytes2[0];
            bytes1[1] ^= bytes2[1];
            bytes1[2] ^= bytes2[2];
            bytes1[3] ^= bytes2[3];
        }

        private static Block MakeKeyFromWords(List<byte> word0, List<byte> word1, List<byte> word2, List<byte> word3)
        {
            return new Block(new List<byte>
            {
                word0[0], word0[1], word0[2], word0[3],
                word1[0], word1[1], word1[2], word1[3],
                word2[0], word2[1], word2[2], word2[3],
                word3[0], word3[1], word3[2], word3[3]
            });
        }
    }
}