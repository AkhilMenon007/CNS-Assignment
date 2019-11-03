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
