using System;
using System.Linq;
using System.Text;

namespace DTT.Hangman
{
    /// <summary>
    /// Represents a phrase usable for a hangman game.
    /// </summary>
    [Serializable]
    public struct Toned
    {
        public static char Check(char letter, int i){
            if(char.IsUpper(letter)) return char.ToUpper(AddTone(letter, i));
            else return char.ToLower(AddTone(letter, i));
        }
        public static char AddTone(char letter, int tone)
        {   
            char c = char.ToLower(letter);
            switch (c)
            {
                case 'a':
                    return tone switch
                    {
                        0 => 'a',
                        1 => 'à',
                        2 => 'á',
                        3 => 'ả',
                        4 => 'ã',
                        5 => 'ạ',
                        _ => 'a',
                    };
                case 'â':
                    return tone switch
                    {
                        0 => 'â',
                        1 => 'ầ',
                        2 => 'ấ',
                        3 => 'ẩ',
                        4 => 'ẫ',
                        5 => 'ậ',
                        _ => 'â',
                    };
                case 'ă':
                    return tone switch
                    {
                        0 => 'ă',
                        1 => 'ằ',
                        2 => 'ắ',
                        3 => 'ẳ',
                        4 => 'ẵ',
                        5 => 'ặ',
                        _ => 'ă',
                    };
                case 'e':
                    return tone switch
                    {
                        0 => 'e',
                        1 => 'è',
                        2 => 'é',
                        3 => 'ẻ',
                        4 => 'ẽ',
                        5 => 'ẹ',
                        _ => 'e',
                    };
                case 'ê':
                    return tone switch
                    {
                        0 => 'ê',
                        1 => 'ề',
                        2 => 'ế',
                        3 => 'ể',
                        4 => 'ễ',
                        5 => 'ệ',
                        _ => 'ê',
                    };
                case 'i':
                    return tone switch
                    {
                        0 => 'i',
                        1 => 'ì',
                        2 => 'í',
                        3 => 'ỉ',
                        4 => 'ĩ',
                        5 => 'ị',
                        _ => 'i',
                    };
                case 'o':
                    return tone switch
                    {
                        0 => 'o',
                        1 => 'ò',
                        2 => 'ó',
                        3 => 'ỏ',
                        4 => 'õ',
                        5 => 'ọ',
                        _ => 'o',
                    };
                case 'ô':
                    return tone switch
                    {
                        0 => 'ô',
                        1 => 'ồ',
                        2 => 'ố',
                        3 => 'ổ',
                        4 => 'ỗ',
                        5 => 'ộ',
                        _ => 'ô',
                    };
                case 'ơ':
                    return tone switch
                    {
                        0 => 'ơ',
                        1 => 'ờ',
                        2 => 'ớ',
                        3 => 'ở',
                        4 => 'ỡ',
                        5 => 'ợ',
                        _ => 'ơ',
                    };
                case 'u':
                    return tone switch
                    {
                        0 => 'u',
                        1 => 'ù',
                        2 => 'ú',
                        3 => 'ủ',
                        4 => 'ũ',
                        5 => 'ụ',
                        _ => 'u',
                    };
                case 'ư':
                    return tone switch
                    {
                        0 => 'ư',
                        1 => 'ừ',
                        2 => 'ứ',
                        3 => 'ử',
                        4 => 'ữ',
                        5 => 'ự',
                        _ => 'ư',
                    };
                case 'y':
                    return tone switch
                    {
                        0 => 'y',
                        1 => 'ỳ',
                        2 => 'ý',
                        3 => 'ỷ',
                        4 => 'ỹ',
                        5 => 'ỵ',
                        _ => 'y',
                    };
                default:
                    return c;
            }
        }
        public static char Removed(char c)
        {
            c = char.ToLower(c);

            switch (c)
            {
                // 'a' variants
                case 'à': case 'á': case 'ả': case 'ã': case 'ạ':
                    return 'a';
                // 'â' variants
                case 'ầ': case 'ấ': case 'ẩ': case 'ẫ': case 'ậ':
                    return 'â';
                // 'ă' variants
                case 'ằ': case 'ắ': case 'ẳ': case 'ẵ': case 'ặ':
                    return 'ă';
                case 'a': case 'â': case 'ă':
                    return c;

                // 'e' variants
                case 'è': case 'é': case 'ẻ': case 'ẽ': case 'ẹ':
                    return 'e';
                // 'ê' variants
                case 'ề': case 'ế': case 'ể': case 'ễ': case 'ệ':
                    return 'ê';
                case 'e': case 'ê':
                    return c;

                // 'i' variants
                case 'ì': case 'í': case 'ỉ': case 'ĩ': case 'ị':
                    return 'i';
                case 'i':
                    return c;

                // 'o' variants
                case 'ò': case 'ó': case 'ỏ': case 'õ': case 'ọ':
                    return 'o';
                // 'ô' variants
                case 'ồ': case 'ố': case 'ổ': case 'ỗ': case 'ộ':
                    return 'ô';
                // 'ơ' variants
                case 'ờ': case 'ớ': case 'ở': case 'ỡ': case 'ợ':
                    return 'ơ';
                case 'o': case 'ô': case 'ơ':
                    return c;

                // 'u' variants
                case 'ù': case 'ú': case 'ủ': case 'ũ': case 'ụ':
                    return 'u';
                // 'ư' variants
                case 'ừ': case 'ứ': case 'ử': case 'ữ': case 'ự':
                    return 'ư';
                case 'u': case 'ư':
                    return c;

                // 'y' variants
                case 'ỳ': case 'ý': case 'ỷ': case 'ỹ': case 'ỵ':
                    return 'y';
                case 'y':
                    return c;

                // 'đ' remains the same
                case 'đ':
                    return 'đ';
                case 'd':
                    return c;

                // Consonants remain the same
                case 'b': case 'c': case 'g': case 'h': case 'k':
                case 'l': case 'm': case 'n': case 'p': case 'q':
                case 'r': case 's': case 't': case 'v': case 'x':
                    return c;
                default:
                    return c;
            }
        }
    }
}
