using lLCroweTool.LogSystem;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace lLCroweTool.QC.Security
{
    //암호화//복호화//양방향이 필요
    //암복호화 시 사용하는 키(KEY)를 프로젝트 소스에 담지 말기

    public static class Security
    {

        public enum ConvertEncryptType
        {
            Encrypt,//암호화
            Decrypt,//복호화
        }

        /// <summary>
        /// DES 암호화
        /// </summary>
        public class DESEncryption
        {
            //테스트완료(잘됨)


            //Key 값은 무조건 8자리여야한다.
            //Key값이 8자리가 아니면, Specified key is not a valid size for this algorithm 와 같은 에러가 발생하니 명심하자.

            /// <summary>
            /// 암복호화
            /// </summary>
            /// <param name="origin">원본값</param>
            /// <param name="password">키값</param>
            /// <param name="convertEncryptType">변환타입</param>
            /// <returns>변환값</returns>
            public static string EncryptString(string origin, string password, ConvertEncryptType convertEncryptType)
            {
                try
                {
                    byte[] key = Encoding.ASCII.GetBytes(password);
                    var des = new DESCryptoServiceProvider()
                    {
                        Key = key,
                        IV = key
                    };

                    var memoryStream = new MemoryStream();
                    ICryptoTransform cryptoTransform = null;
                    byte[] dataArray = null;
                    switch (convertEncryptType)
                    {
                        case ConvertEncryptType.Encrypt:
                            cryptoTransform = des.CreateEncryptor();
                            dataArray = Encoding.UTF8.GetBytes(origin.ToCharArray());
                            break;
                        case ConvertEncryptType.Decrypt:
                            cryptoTransform = des.CreateDecryptor();
                            dataArray = Convert.FromBase64String(origin);
                            break;
                    }

                    //큽토 스트림
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                    cryptoStream.Write(dataArray, 0, dataArray.Length);
                    cryptoStream.FlushFinalBlock();

                    origin = convertEncryptType == ConvertEncryptType.Encrypt ? Convert.ToBase64String(memoryStream.ToArray()) : Encoding.UTF8.GetString(memoryStream.GetBuffer());
                }
                catch (Exception e)
                {
                    LogManager.Register("DESSecurity", "DESSecurity", false, true);
                    LogManager.Log("DESSecurity", $"{convertEncryptType}:{origin} {e.Message}");
                }
                return origin;

            }
        }

        /// <summary>
        /// AES256암호화
        /// </summary>
        public class AES256Encryption
        {
            //고장 안됨//복호화문제거나 아니면 암호할떄문제거나 체크하기

            /// <summary>
            /// 암복호화
            /// </summary>
            /// <param name="origin">원복값</param>
            /// <param name="password">키</param>
            /// <param name="convertEncryptType">변환타입</param>
            /// <returns>변환값</returns>
            public static string EncryptString(string origin, string password, ConvertEncryptType convertEncryptType)
            {
                try
                {
                    // Rihndael class를 선언하고, 초기화
                    RijndaelManaged managed = new RijndaelManaged();
                    // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서 
                    // Salt를 사용한다.
                    byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());

                    // PasswordDeriveBytes 클래스를 사용해서 SecretKey를 얻는다.
                    PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

                    // Create a encryptor from the existing SecretKey bytes.
                    // encryptor 객체를 SecretKey로부터 만든다.
                    // Secret Key에는 32바이트
                    // (Rijndael의 디폴트인 256bit가 바로 32바이트입니다)를 사용하고, 
                    // Initialization Vector로 16바이트
                    // (역시 디폴트인 128비트가 바로 16바이트입니다)를 사용한다.
                    ICryptoTransform cryptoTransform = managed.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
                    MemoryStream memoryStream = null;
                    CryptoStream cryptoStream = null;                   
                    

                    byte[] plainText = null;
                    switch (convertEncryptType)
                    {
                        case ConvertEncryptType.Encrypt:
                            // 입력받은 문자열을 바이트 배열로 변환
                            plainText = Encoding.Unicode.GetBytes(origin);
                            memoryStream = new MemoryStream();// 메모리스트림 객체를 선언,초기화 

                            // CryptoStream객체를 암호화된 데이터를 쓰기 위한 용도로 선언
                            cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);

                            // 암호화 프로세스가 진행된다.
                            cryptoStream.Write(plainText, 0, plainText.Length);

                            // 암호화 종료
                            cryptoStream.FlushFinalBlock();

                            // 암호화된 데이터를 바이트 배열로 담는다.
                            byte[] cipherBytes = memoryStream.ToArray();

                            // 스트림 해제
                            memoryStream.Close();
                            cryptoStream.Close();

                            // 암호화된 데이터를 Base64 인코딩된 문자열로 변환한다.
                            origin = Convert.ToBase64String(cipherBytes);

                            break;
                        case ConvertEncryptType.Decrypt:
                            byte[] encryptedData = Convert.FromBase64String(origin);
                            memoryStream = new MemoryStream(encryptedData);

                            // 데이터 읽기(복호화이므로) 용도로 cryptoStream객체를 선언, 초기화
                            cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);


                            // 복호화된 데이터를 담을 바이트 배열을 선언한다.
                            // 길이는 알 수 없지만, 일단 복호화되기 전의 데이터의 길이보다는
                            // 길지 않을 것이기 때문에 그 길이로 선언한다.
                            plainText = new byte[encryptedData.Length];

                            // 복호화 시작
                            int DecryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

                            // 스트림 해제
                            memoryStream.Close();
                            cryptoStream.Close();

                            // 복호화된 데이터를 문자열로 바꾼다.
                            origin = Encoding.Unicode.GetString(plainText, 0, DecryptedCount);
                            break;
                    }

                 
                }
                catch (Exception e)
                {
                    LogManager.Register("AES256Security", "AES256Security", false, true);
                    LogManager.Log("AES256Security", $"{convertEncryptType}:{origin} {e.Message}");
                }
                // 최종 결과를 리턴
                return origin;
            }
        }


        //따로 키값을 안가지고 암복호화//마소에서 제공
        //DPAPI(Data Protection API)// 운영 체제에서 제공 되는 서비스
        //사용자 또는 컴퓨터 자격 증명을 사용하여 데이터를 암호화하거나 암호 해독하는 보호를 제공
        //근데 느낌상이거 다른 윈도우컴가면 작동안될 느낌인데//그래서 정품이용자만 할수 있는 세이브일수도?

        //https://learn.microsoft.com/ko-kr/dotnet/api/system.security.cryptography.protecteddata?view=dotnet-plat-ext-5.0

        /// <summary>
        /// MS에서 제공하는 암복화
        /// </summary>
        //public class MSEncryption
        //{
        //    //테스트(참조해줘야됨)
        //    //정품사용자인지 체크할때//정품아니면 문제가 발생할수 있음.
        //    //DataProtectionScope.LocalMachine//=>문제가 발생시 이걸로 처리
        //    //DataProtectionScope.CurrentUser

        //    /// <summary>
        //    /// 암복호화
        //    /// </summary>
        //    /// <param name="origin">원복값</param>
        //    /// <param name="convertEncryptType">변환타입</param>
        //    /// <returns>변환값</returns>
        //    public static string EncryptString(string origin, ConvertEncryptType convertEncryptType)
        //    {
        //        try
        //        {
        //            switch (convertEncryptType)
        //            {
        //                case ConvertEncryptType.Encrypt:
        //                    origin = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(origin), null, DataProtectionScope.LocalMachine));
        //                    break;
        //                case ConvertEncryptType.Decrypt:
        //                    origin = Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(origin), null, DataProtectionScope.LocalMachine));
        //                    break;
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            LogManager.Register("MSSecurity", "MSSecurity", false, true);
        //            LogManager.Log("MSSecurity", $"{convertEncryptType}:{origin} {e.Message}");
        //        }
        //        return origin;
        //    }
        //}
    }
}


