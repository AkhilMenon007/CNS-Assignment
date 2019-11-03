using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Numerics;

public static class CNS
{
    public const int RSA_SIZE = 2048;
    public static void Test()
    {
        RSAParameters parameters = GenerateRSAParameters();
        RSACryptoServiceProvider encrypt = new RSACryptoServiceProvider(RSA_SIZE);
        parameters = encrypt.ExportParameters(true);

        DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
        dESCryptoServiceProvider.GenerateKey();
        dESCryptoServiceProvider.GenerateIV();

        string key = Convert.ToBase64String(dESCryptoServiceProvider.Key);
        string IV = Convert.ToBase64String(dESCryptoServiceProvider.IV);

        string msg = GenerateMessage("Hello", parameters, key, IV);

        ReproduceMessage(msg, out msg, parameters, key, IV);

    }

    #region Send Message in Format of Assignment

    public static string GenerateMessage(string message,RSAParameters privateRSAKey,string DESKey,string DESIV) 
    {
        BigInteger h = ComputeSHAHash(message);
        h = RSAExponentiate(h, privateRSAKey.D, privateRSAKey.Modulus);
        string concat = message.Length + " " + message + GetString64(h);

        return EncryptWithDES(concat, DESKey, DESIV);
    }

    #endregion

    #region Reproduce Message / Verify Message

    public static bool ReproduceMessage(string cryptText,out string message,RSAParameters publicRSAKey,string DESKey,string DESIV) 
    {
        string concat = DecryptWithDES(cryptText, DESKey, DESIV);
        int msgLengthIndex = concat.IndexOf(' ');
        string msgLength = concat.Substring(0, msgLengthIndex);
        int len;
        if(!int.TryParse(msgLength, out len)) 
        {
            Console.WriteLine("Invalid message recieved");
            message = "";
            return false;
        }
        message = concat.Substring(msgLengthIndex + 1, len);
        string cryptHash = concat.Substring(msgLengthIndex + len + 1);

        BigInteger hash=RSAExponentiate(new BigInteger(Convert.FromBase64String(cryptHash)), publicRSAKey.Exponent, publicRSAKey.Modulus);

        if (ComputeSHAHash(message) != hash) 
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Helpers
    public static byte[] StringToByte(string input)
    {
        return Encoding.Unicode.GetBytes(input);

    }
    public static string ByteToString(byte[] input)
    {
        return Encoding.Unicode.GetString(input);
    }

    public static string GetString64(BigInteger integer) 
    {
        return Convert.ToBase64String(integer.ToByteArray());
    }
    public static string GetString64(byte[] integer) 
    {
        return Convert.ToBase64String(integer);
    }


    public static BigInteger GetBigInt(byte[] input) 
    {
        byte[] byteArray = (byte[])input.Clone();

        Array.Reverse(byteArray);

        byte[] res = new byte[byteArray.Length + 1];

        Array.Copy(byteArray, 0, res, 0, byteArray.Length);

        res[byteArray.Length] = 0;

        return new BigInteger(res);
    }


    #endregion

    #region RSA Encrypt and Decrypt for Testing

    public static RSAParameters GenerateRSAParameters()
    {
        using (RSACryptoServiceProvider encrypt = new RSACryptoServiceProvider(RSA_SIZE))
        {
            return encrypt.ExportParameters(true);
        }
    }


    public static string RSAEncrypt(string input,RSAParameters key) 
    {
        using (RSACryptoServiceProvider encrypt = new RSACryptoServiceProvider(RSA_SIZE)) 
        {
            encrypt.ImportParameters(key);
            var res=encrypt.Encrypt(StringToByte(input),false);
            return Convert.ToBase64String(res);
        }
    }

    public static string RSADecrypt(string input, RSAParameters key) 
    {
        using(RSACryptoServiceProvider decrypt = new RSACryptoServiceProvider(RSA_SIZE)) 
        {
            decrypt.ImportParameters(key);
            var res = decrypt.Decrypt(Convert.FromBase64String(input), false);
            return ByteToString(res);
        }
    }

    #endregion

    #region RSAExponentiate Methods

    public static BigInteger RSAExponentiate(BigInteger input,byte[] power,byte[] modulus) 
    {
        return RSAExponentiate(input, GetBigInt(power), GetBigInt(modulus));
    }

    public static BigInteger RSAExponentiate(string input,BigInteger power, BigInteger modulus) 
    {
        return RSAExponentiate(GetBigInt(StringToByte(input)), power, modulus);
    }

    public static BigInteger RSAExponentiate(BigInteger input, BigInteger power, BigInteger modulus) 
    {
        var res = BigInteger.ModPow(input, power, modulus);
        return res * res.Sign;
    }


    #endregion

    #region SHA

    public static BigInteger ComputeSHAHash(string input) 
    {
        using(SHA1Managed sha = new SHA1Managed()) 
        {
            return GetBigInt(sha.ComputeHash(StringToByte(input)));
        }
    }


    #endregion

    #region DES

    public static string EncryptWithDES(string message,string key,string IV) 
    {
        byte[] result;

        using (DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider()) 
        {
            ICryptoTransform c = cryptoServiceProvider.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
            using (MemoryStream stream = new MemoryStream()) 
            {
                using(CryptoStream cs = new CryptoStream(stream, c, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cs))
                        writer.Write(message);
                }
                result = stream.ToArray();
            }
        }
        return Convert.ToBase64String(result);
    }

    public static string DecryptWithDES(string cryptText, string key, string IV)
    {
        string result;
        using (DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider())
        {
            ICryptoTransform c = cryptoServiceProvider.CreateDecryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(cryptText)))
            {
                using (CryptoStream cs = new CryptoStream(stream, c, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                        result = reader.ReadToEnd();
                }
            }
        }
        return result;
    }

    #endregion
}
