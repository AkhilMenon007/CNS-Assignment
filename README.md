## Cryptography and Network Security Assignment

**Question :**

The question is to to create a system which provides a message sending service which guarentees the source to be a valid one with digital signature. A symmetric Key encryption technique of **DES** is used to encrypt the digitally signed message. The same system may be used for authentication of an entity.
**SHA1** is used to hash the message which is used as a part of signature.
The hashed message is signed using **RSA** with a private key on sender and is verified with the help of the public key.
The encrypted hash is concatenated with the message which is encrypted using symmetric key encryption.

The receiver uses the secret DES key to decrypt the incoming message. This is then split into the *encrypted* hash and message.
The *encrypted* hash is then *decrypted* using the public key of RSA to get the hash of the message.
The message is confirmed to be authentic if the decrypted value matches the actual hash of the message.

![Question](https://github.com/AkhilMenon007/CNS-Assignment/blob/master/CNS_ProgrammingAssignment/Question.PNG)

**Solution**

In the repository you may find a file named [CNS.cs](https://github.com/AkhilMenon007/CNS-Assignment/blob/master/CNS.cs) which is a static class which can be used for achieving the solution to the problem. Within the class are the following static functions which solves the problem with the help of in built C# functions  : 

1. **GenerateMessage** : This function generates the message which can be sent with all the required operations performed. It takes in the **message** to be sent as a string, an RSAParameters object containing the **private key** for RSA encryption of the hash , a string representing the **DES Key** and a string representing **DES Initial Vector** both encoded as base64 strings.
2.  **ReproduceMessage** : This function returns true if the message recieved can be verified successfully by using the available keys. It takes in the **message** recieved from the sender as a string encoded in base64 , an **out message** which gives the message after its been deciphered, an RSAParameters object containing the **public key** for RSA decryption of hash, a string representing the **DES Key** and a string representing **DES Initial Vector** both encoded as base64 strings.
3.  **GenerateRSAParameters** : This function generates the RSAParameters which can be used for Generating the **public** and **private keys**.
4.  **RSAExponentiate** : This function act as a multi-tool for both encryption and decryption  with RSA. It takes the **input number** , followed by **power** and **modulus**.
5. **ComputeSHAHash** : Computes the hash value of the input string
6. **EncryptWithDES** : Encrypts the **message** with given **Key** and **Initial Vector**.
7. **DecryptWithDES** : Decrypts the **message** with given **Key** and **Initial Vector**.
