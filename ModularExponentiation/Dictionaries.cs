namespace ModularExponentiation
{
    using System.Collections.Generic;

    public static class Dictionaries
    {
        public static readonly Dictionary<char, byte> CharToByte = new()
        {
            {'0', 0},
            {'1', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9}
        };

        public static readonly Dictionary<byte, char> ByteToChar = new()
        {
            {0, '0'},
            {1, '1'},
            {2, '2'},
            {3, '3'},
            {4, '4'},
            {5, '5'},
            {6, '6'},
            {7, '7'},
            {8, '8'},
            {9, '9'}
        };

        public static readonly Dictionary<bool, Dictionary<bool, Dictionary<bool, bool>>> AdderTable = new()
        {
            {
                false, new Dictionary<bool, Dictionary<bool, bool>>
                {
                    {
                        false, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, false}
                        }
                    },
                    {
                        true, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, true}
                        }
                    }
                }
            },
            {
                true, new Dictionary<bool, Dictionary<bool, bool>>
                {
                    {
                        false, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, true}
                        }
                    },
                    {
                        true, new Dictionary<bool, bool>
                        {
                            {false, true},
                            {true, true}
                        }
                    }
                }
            }
        };
        
        public static readonly Dictionary<bool, Dictionary<bool, Dictionary<bool, bool>>> SubtractorTable = new()
        {
            {
                false, new Dictionary<bool, Dictionary<bool, bool>>
                {
                    {
                        false, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, true}
                        }
                    },
                    {
                        true, new Dictionary<bool, bool>
                        {
                            {false, true},
                            {true, true}
                        }
                    }
                }
            },
            {
                true, new Dictionary<bool, Dictionary<bool, bool>>
                {
                    {
                        false, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, false}
                        }
                    },
                    {
                        true, new Dictionary<bool, bool>
                        {
                            {false, false},
                            {true, true}
                        }
                    }
                }
            }
        };
    }
}