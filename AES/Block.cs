namespace AES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GM = GaloisMultiplication;

    public class Block
    {
        private List<byte> bytes;
        private List<byte> temp = new(new byte [16]);
        public Block(List<byte> bytes)
        {
            this.bytes = Transpose(bytes);
        }

        private List<byte> Transpose(List<byte> byteList)
        {
            return new List<byte>
            {
                byteList[0], byteList[4], byteList[8], byteList[12],
                byteList[1], byteList[5], byteList[9], byteList[13],
                byteList[2], byteList[6], byteList[10], byteList[14],
                byteList[3], byteList[7], byteList[11], byteList[15]
            };
        }

        public List<byte> GetWord(int number)
        {
            return number switch
            {
                0 => new List<byte> {bytes[0], bytes[4], bytes[8], bytes[12]},
                1 => new List<byte> {bytes[1], bytes[5], bytes[9], bytes[13]},
                2 => new List<byte> {bytes[2], bytes[6], bytes[10], bytes[14]},
                3 => new List<byte> {bytes[3], bytes[7], bytes[11], bytes[15]},
                _ => throw new Exception("Number should be between 0 and 3")
            };
        }

        public List<byte> GetAllBytes()
        {
            return Transpose(bytes);
        }

        public void AddRoundKey(Block block)
        {
            var otherBlockBytes = block.bytes.ToList();
            for (var i = 0; i < 16; i++)
            {
                var byte1 = bytes[i];
                var byte2 = otherBlockBytes[i];
                bytes[i] = (byte)(byte1 ^ byte2);
            }
        }

        public void SubBytes()
        {
            Boxes.SubBytes(bytes);
        }

        public void ShiftRows()
        { 
            (bytes[4], bytes[5]) = (bytes[5], bytes[4]);
            (bytes[5], bytes[6]) = (bytes[6], bytes[5]);
            (bytes[6], bytes[7]) = (bytes[7], bytes[6]);

            (bytes[8], bytes[10]) = (bytes[10], bytes[8]);
            (bytes[9], bytes[11]) = (bytes[11], bytes[9]);

            (bytes[15], bytes[14]) = (bytes[14], bytes[15]);
            (bytes[14], bytes[13]) = (bytes[13], bytes[14]);
            (bytes[13], bytes[12]) = (bytes[12], bytes[13]);
        }
        
        public void MixColumns() {
            for (var c = 0; c < 4; c++) {
                temp[c] = (byte)(GM.Mul(0x02, Get(0, c)) ^ GM.Mul(0x03, Get(1, c)) ^ Get(2, c) ^ Get(3, c));
                temp[4 + c] = (byte)(Get(0, c) ^ GM.Mul(0x02, Get(1, c)) ^ GM.Mul(0x03, Get(2, c)) ^ Get(3, c));
                temp[8 + c] = (byte)(Get(0, c) ^ Get(1, c) ^ GM.Mul(0x02, Get(2, c)) ^ GM.Mul(0x03, Get(3, c)));
                temp[12 + c] = (byte)(GM.Mul(0x03, Get(0,c)) ^ Get(1, c) ^ Get(2, c) ^ GM.Mul(0x02, Get(3, c)));
            }

            (bytes, temp) = (temp, bytes);
        }
        
        private byte Get(int row, int column)
        {
            return bytes[4 * row + column];
        }

        public void InvSubBytes()
        {
            Boxes.InvSubBytes(bytes);
        }

        public void InvShiftRows()
        {
            (bytes[6], bytes[7]) = (bytes[7], bytes[6]);
            (bytes[5], bytes[6]) = (bytes[6], bytes[5]);
            (bytes[4], bytes[5]) = (bytes[5], bytes[4]);

            (bytes[8], bytes[10]) = (bytes[10], bytes[8]);
            (bytes[9], bytes[11]) = (bytes[11], bytes[9]);

            (bytes[13], bytes[12]) = (bytes[12], bytes[13]);
            (bytes[14], bytes[13]) = (bytes[13], bytes[14]);
            (bytes[15], bytes[14]) = (bytes[14], bytes[15]);
        }
        
        public void InvMixColumns() {
            for (var c = 0; c < 4; c++) {
                temp[c] = (byte)(GM.Mul(0x0E, Get(0, c)) ^
                                 GM.Mul(0x0B, Get(1, c)) ^
                                 GM.Mul(0x0D, Get(2, c)) ^
                                 GM.Mul(0x09, Get(3, c)));
                temp[4 + c] = (byte)(GM.Mul(0x09, Get(0, c)) ^
                                     GM.Mul(0x0E, Get(1, c)) ^
                                     GM.Mul(0x0B, Get(2, c)) ^
                                     GM.Mul(0x0D, Get(3, c)));
                temp[8 + c] = (byte)(GM.Mul(0x0D, Get(0, c)) ^
                                     GM.Mul(0x09, Get(1, c)) ^
                                     GM.Mul(0x0E, Get(2, c)) ^
                                     GM.Mul(0x0B, Get(3, c)));
                temp[12 + c] = (byte)(GM.Mul(0x0B, Get(0, c)) ^
                                      GM.Mul(0x0D, Get(1, c)) ^
                                      GM.Mul(0x09, Get(2, c)) ^
                                      GM.Mul(0x0E, Get(3, c)));
            }

            (bytes, temp) = (temp, bytes);
        }

        public void XorWith(Block otherBlock)
        {
            for (var i = 0; i < 16; i++)
            {
                bytes[i] ^= otherBlock.bytes[i];
            }
        }
    }
}