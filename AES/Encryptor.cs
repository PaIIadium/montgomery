namespace AES
{
    using System.Collections.Generic;

    public static class Encryptor
    {
        private const int RoundsCount = 10;

        public static List<byte> Encrypt(List<byte> key, List<byte> input, EncryptionMode mode, List<byte> iv = null)
        {
            switch (mode)
            {
                case EncryptionMode.ECB:
                    return EncryptECB(key, input);
                default:
                case EncryptionMode.CBC:
                    return EncryptCBC(key, iv, input);
            }
        }

        private static List<byte> EncryptECB(List<byte> key, List<byte> input)
        {
            var keyBlock = new Block(key);
            var blocks = Tools.SplitIntoBlocks(input);
            var roundKeys = Tools.KeyExpansion(keyBlock);
            
            foreach (var block in blocks)
            {
                EncryptBlock(block, roundKeys, keyBlock);
            }

            return Tools.UniteBlocks(blocks);
        }
        
        private static void EncryptBlock(Block block, List<Block> roundKeys, Block keyBlock)
        {
            block.AddRoundKey(keyBlock);
            for (var j = 0; j < RoundsCount; j++)
            {
                block.SubBytes();
                block.ShiftRows();
                if (j != RoundsCount - 1) block.MixColumns();
                block.AddRoundKey(roundKeys[j]);
            }
        }

        private static List<byte> EncryptCBC(List<byte> key, List<byte> initializationVector, List<byte> input)
        {
            var keyBlock = new Block(key);
            var blocks = Tools.SplitIntoBlocks(input);
            var ivBlock = new Block(initializationVector);
            var roundKeys = Tools.KeyExpansion(keyBlock);

            var lastEncryptedBlock = ivBlock;
            foreach (var block in blocks)
            {
                block.XorWith(lastEncryptedBlock);
                EncryptBlock(block, roundKeys, keyBlock);
                lastEncryptedBlock = block;
            }
            
            return Tools.UniteBlocks(blocks);
        }
    }
}