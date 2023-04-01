using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;

namespace NethereumSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string head;
            string tail;
            decimal num;
            bool isCaseSensitive;

            // 引数の確認
            if (!CheckArgs(args, out head, out tail, out num, out isCaseSensitive))
            {
                Console.WriteLine("PrivateKeyGacha の使いかた");
                Console.WriteLine(" /H 先頭の文字を指定します(デフォルトは空文字)");
                Console.WriteLine(" /T 末尾の文字を指定します(デフォルトは空文字)");
                Console.WriteLine(" /N ガチャを引く回数を指定します(デフォルトは UInt32.MaxValue");
                Console.WriteLine(" /CS 大文字と小文字を区別します(デフォルトは false)");
                return;
            }

            // ここまで来たらガチャ処理の呼び出し
            Gacha(head, tail, num, isCaseSensitive);
        }

        static bool CheckArgs(string[] args, out string head, out string tail, out decimal num, out bool isCaseSensitive)
        {
            // デフォルト値
            head = "";
            tail = "";
            num = UInt32.MaxValue;
            isCaseSensitive = false;

            // オプション確認
            var i = 0;
            while (i < args.Length)
            {
                var option = args[i++];

                // ヘッド
                if (option == "/H")
                {
                    if (i < args.Length)
                    {
                        head = args[i++];

                        // 16進数扱いできなければエラー
                        int temp;
                        if (!int.TryParse(head, System.Globalization.NumberStyles.HexNumber, null, out temp))
                        {
                            return (false);
                        }
                    }
                    else
                    {
                        return (false);
                    }
                }
                // テイル
                else if (option == "/T")
                {
                    if (i < args.Length)
                    {
                        tail = args[i++];

                        // 16進数扱いできなければエラー
                        int temp;
                        if (!int.TryParse(tail, System.Globalization.NumberStyles.HexNumber, null, out temp))
                        {
                            return (false);
                        }
                    }
                    else
                    {
                        return (false);
                    }
                }
                // 試行回数
                else if (option == "/N")
                {
                    if (i < args.Length)
                    {
                        if (!decimal.TryParse(args[i++], out num))
                        {
                            return (false);
                        }
                    }
                    else
                    {
                        return (false);
                    }
                }
                // 大文字小文字判定
                else if (option == "/CS")
                {
                    isCaseSensitive = true;
                }
                // ここにきたら駄目
                else
                {
                    return (false);
                }
            }

            // 頭と尻尾がなければエラー
            if (head.Length <= 0 && tail.Length <= 0)
            {
                return (false);
            }

            // ここまできたらOK
            return (true);
        }

        static void Gacha(string targetHead, string targetTail, decimal num, bool isCaseSensiive)
        {
            // 大文字小文字区別しないなら小文字に統一
            if (!isCaseSensiive)
            {
                targetHead = targetHead.ToLower();
                targetTail = targetTail.ToLower();
            }

            // 指定回数回す
            for (decimal i = 0; i < num; i++)
            {
                // ランダムで秘密鍵を作る
                var ecKey = EthECKey.GenerateKey();
                var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                var account = new Account(privateKey);

                Console.WriteLine(account.Address + "(" + (i + 1).ToString() + "連目)");

                // 頭の指定が有効で一致しなければハズレ
                if (targetHead.Length > 0)
                {
                    var temp = account.Address.Substring(2, targetHead.Length);
                    if (!isCaseSensiive)
                    {
                        temp = temp.ToLower();
                    }

                    if (targetHead != temp)
                    {
                        continue;
                    }
                }

                // 尻尾の指定が有効で一致しなければハズレ
                if (targetTail.Length > 0)
                {
                    var temp = account.Address.Substring(42 - targetTail.Length, targetTail.Length);
                    if (!isCaseSensiive)
                    {
                        temp = temp.ToLower();
                    }

                    if (targetTail != temp)
                    {
                        continue;
                    }
                }

                // ここまできたらアタリ
                Console.WriteLine("★★★★★大勝利★★★★★");
                Console.WriteLine("アドレス : " + account.Address);
                Console.WriteLine("秘密鍵 : " + account.PrivateKey);
                return;
            }

            // ここまで来たらハズレ
            Console.WriteLine("爆死した...");
        }
    }
}
