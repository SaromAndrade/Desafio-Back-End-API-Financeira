using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16; // Tamanho do salt em bytes
        private const int HashSize = 20; // Tamanho do hash em bytes
        private const int Iterations = 10000; // Número de iterações

        // Gera um hash a partir de uma senha
        public static string HashPassword(string password)
        {
            // Gera um salt aleatório
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Gera o hash usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combina o salt e o hash em um único array
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Converte o array para uma string base64
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }
        // Verifica se a senha fornecida corresponde ao hash armazenado
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Converte a senha hash de volta para um array de bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Extrai o salt do hash
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Gera o hash da senha fornecida usando o mesmo salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Compara os hashes
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false; // Senha inválida
                }
            }

            return true; // Senha válida
        }
    }
}
