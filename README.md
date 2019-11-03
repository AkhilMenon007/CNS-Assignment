## Cryptography and Network Security Assignment

### **Question :**

The question is to to create a system which provides a message sending service which guarentees the source to be a valid one with digital signature. 
**[SHA1](https://en.wikipedia.org/wiki/SHA-1)** is used to hash the message which is used as a part of signature.

The hashed message is signed using **[RSA](https://en.wikipedia.org/wiki/RSA_%28cryptosystem%29)** with a private key on sender and is verified with the help of the public key on the receiver.

The encrypted hash is concatenated with the message which is encrypted using symmetric key encryption.
A symmetric Key encryption technique of **[DES](https://en.wikipedia.org/wiki/Data_Encryption_Standard)** is used to encrypt the digitally signed message. The same system may be used for authentication of an entity.

The receiver uses the secret DES key to decrypt the incoming message. This is then split into the *encrypted* hash and message.
The *encrypted* hash is then *decrypted* using the public key of RSA to get the hash of the message.
The message is confirmed to be authentic if the decrypted value matches the actual hash of the message.

![Question](https://github.com/AkhilMenon007/CNS-Assignment/blob/master/CNS_ProgrammingAssignment/Question.PNG)

### **Solution :**

In the repository you may find a file named [CNS.cs](https://github.com/AkhilMenon007/CNS-Assignment/blob/master/CNS.cs) which is a static class which can be used for achieving the solution to the problem. Within the class are the following static functions which solves the problem with the help of in built C# functions  : 

 - **GenerateMessage** : This function generates the message which can be sent with all the required operations performed. It takes in the **message** to be sent as a string, an RSAParameters object containing the **private key** for RSA encryption of the hash , a string representing the **DES Key** and a string representing **DES Initial Vector** both encoded as [base64](https://en.wikipedia.org/wiki/Base64) strings.
 -  **ReproduceMessage** : This function returns true if the message recieved can be verified successfully by using the available keys. It takes in the **message** recieved from the sender as a string encoded in base64 , an **out message** which gives the message after its been deciphered, an RSAParameters object containing the **public key** for RSA decryption of hash, a string representing the **DES Key** and a string representing **DES Initial Vector** both encoded as base64 strings.
 -  **GenerateRSAParameters** : This function generates the [RSAParameters](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsaparameters?view=netframework-4.8) which can be used for Generating the **public** and **private keys**.
 
 -  **RSAExponentiate** : This function act as a multi-tool for both encryption and decryption  with RSA. It takes the **input number** , followed by **power** and **modulus**.
 
 - **ComputeSHAHash** : Computes the hash value of the input string
 
 - **EncryptWithDES** : Encrypts the **message** with given **Key** and **Initial Vector**.
 
 - **DecryptWithDES** : Decrypts the **message** with given **Key** and **Initial Vector**.

### **Additional Information :**

The assignment is solved using C# System.Security.Cryptography implementation of RSA,DES and SHA1. The following points are specific for using this :

 - **RSAParameters** : This is returned from **ExportParameters** method and contains both the private and public part of the key or just the public part depending on if the parameter to ExportParameters. It will have both the private and public part if true is passed to ExportParameters and just the public part otherwise. Only the **public part** of the key is to be sent to the receiver. The **private part** is used for encrypting the hash.
 
 - **BigInteger** : The RSA exponentiation for encryption and decryption are done using the **ModPow** method of the **BigInteger** struct of C# which stores the values in *Big-Endian* representation whilst the values obtained from RSA Parameters are all in *Little-Endian* representation as an array of bytes. The conversion between the 2 representations is handled by **GetBigInt** helper method which converts the Little-Endian byte array to Big-Endian array with a 0 byte at the MSB representing unsigned BigInteger.
 
 - **Strings** : The byte array is converted to a [string of base64](https://en.wikipedia.org/wiki/Base64) for easy information exchange.
