$selfSignedRootCA = New-SelfSignedCertificate -type CodeSigningCert -DnsName "Ayalon.MFA.CredProvider" -notafter (Get-Date).AddMonths(12) -CertStoreLocation Cert:\LocalMachine\My\ -KeyExportPolicy Exportable -KeyUsage CertSign,CRLSign,DigitalSignature -KeySpec KeyExchange -KeyLength 2048 -KeyUsageProperty All -KeyAlgorithm 'RSA' -HashAlgorithm 'SHA256' -Provider 'Microsoft Enhanced RSA and AES Cryptographic Provider'
$CertPassword = ConvertTo-SecureString -String "MyPassword" -Force -AsPlainText
$selfSignedRootCA | Export-PfxCertificate -FilePath C:\Ayalon.MFA.CredProvider.pfx -Password $CertPassword

Import-PfxCertificate -FilePath "C:\Ayalon.MFA.CredProvider.pfx" -CertStoreLocation "Cert:\LocalMachine\My" -Password $CertPassword


#signtool sign /fd SHA256 /f "C:\Ayalon.MFA-CredProvider.pfx" /p "MyPassword" /td SHA256 /v "C:\Windows\System32\AyalonMFA\Ayalon.MFA.CredProvider.dll"
