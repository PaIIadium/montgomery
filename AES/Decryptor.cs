namespace AES
{
    using System.Collections.Generic;

    public static class Decryptor
    {
        private const int RoundsCount = 10;

        public static List<byte> Decrypt(List<byte> key, List<byte> input, EncryptionMode mode, List<byte> iv = null)
        {
            switch (mode)
            {
                case EncryptionMode.ECB:
                    return DecryptECB(key, input);
                default:
                case EncryptionMode.CBC:
                    return DecryptCBC(key, iv, input);
            }
        }

        private static List<byte> DecryptECB(List<byte> key, List<byte> input)
        {
            var blocks = Tools.SplitIntoBlocks(input);
            var keyBlock = new Block(key);
            var roundKeys = Tools.KeyExpansion(keyBlock);
            
            foreach (var block in blocks)
            {
                DecryptBlock(block, roundKeys, keyBlock);
            }

            return Tools.UniteBlocks(blocks);
        }

        private static void DecryptBlock(Block block, List<Block> roundKeys, Block keyBlock)
        {
            block.AddRoundKey(roundKeys[^1]);
            for (var j = RoundsCount - 1; j >= 0; j--)
            {
                block.InvShiftRows();
                block.InvSubBytes();

                if (j != 0)
                {
                    block.AddRoundKey(roundKeys[j - 1]);
                    block.InvMixColumns();
                }
                else
                {
                    block.AddRoundKey(keyBlock);
                }
            }
        }
        
        private static List<byte> DecryptCBC(List<byte> key, List<byte> initializationVector, List<byte> input)
        {
            var blocks = Tools.SplitIntoBlocks(input);
            var keyBlock = new Block(key);
            var ivBlock = new Block(initializationVector);
            var roundKeys = Tools.KeyExpansion(keyBlock);

            for (var i = blocks.Count - 1; i >= 0; i--)
            {
                var block = blocks[i];
                DecryptBlock(block, roundKeys, keyBlock);
                block.XorWith(i != 0 ? blocks[i - 1] : ivBlock);
            }

            return Tools.UniteBlocks(blocks);
        }
    }
}