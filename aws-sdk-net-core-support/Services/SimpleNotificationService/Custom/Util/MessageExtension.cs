using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Amazon.Util;
using ThirdParty.BouncyCastle.OpenSsl;

// Add back verify message function for AWS .Net Core SDK 
// Fork from: https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/SimpleNotificationService/Custom/Util/Message.cs
namespace Amazon.SimpleNotificationService.Util {
	public static class MessageExtension {
		/// <summary>
		/// Verifies the authenticity of a message sent by Amazon SNS. This is done by computing a signature from the fields in the message and then comparing 
		/// the signature to the signature provided as part of the message.
		/// </summary>
		/// <returns>Returns true if the message is authentic.</returns>
		public static bool IsMessageSignatureValid(this Message message) {
			MessageVerifier messageVerifier = new MessageVerifier(message);
			return messageVerifier.IsValid();
		}


		private class MessageVerifier {
			private const int MAX_RETRIES = 3;

			private Message message;


			internal MessageVerifier(Message message) {
				this.message = message;
			}


			public bool IsValid() {
				var bytesToSign = GetMessageBytesToSign();
				var certificate = GetX509Certificate();

				// Old AWS SDK code snippet
				//	var rsa = certificate.PublicKey.Key as RSACryptoServiceProvider;
				//	return rsa.VerifyData(bytesToSign, CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(this.Signature));

				// The AWS SDK using old-way to doing data verify prior to .Net 4.6 and .Net core not support that.
				// So just move to the new way.
				var rsa = certificate.GetRSAPublicKey();
				return rsa.VerifyData(bytesToSign, Convert.FromBase64String(message.Signature), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
			}

			private byte[] GetMessageBytesToSign() {
				string stringToSign = null;

				if (message.IsNotificationType)
					stringToSign = BuildNotificationStringToSign();
				else if (message.IsSubscriptionType || message.IsUnsubscriptionType)
					stringToSign = BuildSubscriptionStringToSign();
				else
					throw new Exception("Unknown message type: " + message.Type);

				byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
				return bytesToSign;
			}

			/// <summary>
			/// Build the string to sign for Notification messages.
			/// </summary>
			/// <returns>The string to sign</returns>
			private string BuildSubscriptionStringToSign() {
				StringBuilder stringToSign = new StringBuilder();

				stringToSign.Append("Message\n");
				stringToSign.Append(message.MessageText);
				stringToSign.Append("\n");

				stringToSign.Append("MessageId\n");
				stringToSign.Append(message.MessageId);
				stringToSign.Append("\n");

				stringToSign.Append("SubscribeURL\n");
				stringToSign.Append(message.SubscribeURL);
				stringToSign.Append("\n");

				stringToSign.Append("Timestamp\n");
				stringToSign.Append(message.Timestamp.ToString(AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture));
				stringToSign.Append("\n");

				stringToSign.Append("Token\n");
				stringToSign.Append(message.Token);
				stringToSign.Append("\n");

				stringToSign.Append("TopicArn\n");
				stringToSign.Append(message.TopicArn);
				stringToSign.Append("\n");

				stringToSign.Append("Type\n");
				stringToSign.Append(message.Type);
				stringToSign.Append("\n");

				return stringToSign.ToString();
			}

			/// <summary>
			/// Build the string to sign for SubscriptionConfirmation and UnsubscribeConfirmation messages.
			/// </summary>
			/// <returns>The string to sign</returns>
			private string BuildNotificationStringToSign() {
				StringBuilder stringToSign = new StringBuilder();

				stringToSign.Append("Message\n");
				stringToSign.Append(message.MessageText);
				stringToSign.Append("\n");

				stringToSign.Append("MessageId\n");
				stringToSign.Append(message.MessageId);
				stringToSign.Append("\n");

				if (message.Subject != null) {
					stringToSign.Append("Subject\n");
					stringToSign.Append(message.Subject);
					stringToSign.Append("\n");
				}

				stringToSign.Append("Timestamp\n");
				stringToSign.Append(message.Timestamp.ToString(AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture));
				stringToSign.Append("\n");


				stringToSign.Append("TopicArn\n");
				stringToSign.Append(message.TopicArn);
				stringToSign.Append("\n");

				stringToSign.Append("Type\n");
				stringToSign.Append(message.Type);
				stringToSign.Append("\n");

				return stringToSign.ToString();
			}

			static Dictionary<string, X509Certificate2> certificateCache = new Dictionary<string, X509Certificate2>();
			private X509Certificate2 GetX509Certificate() {
				lock (certificateCache) {
					if (certificateCache.ContainsKey(message.SigningCertURL)) {
						return certificateCache[message.SigningCertURL];
					} else {
						for (int retries = 1; retries <= MAX_RETRIES; retries++) {
							try {
								WebRequest request = WebRequest.Create(message.SigningCertURL) as HttpWebRequest;
								WebResponse response = request.GetResponseAsync().Result;

								using (var reader = new StreamReader(response.GetResponseStream())) {
									var content = reader.ReadToEnd().Trim();
									var pemObject = new PemReader(new StringReader(content)).ReadPemObject();

									X509Certificate2 certificate = new X509Certificate2(pemObject.Content);
									certificateCache[message.SigningCertURL] = certificate;
									return certificate;
								}
							} catch (Exception e) {
								if (retries == MAX_RETRIES) {
									throw new Exception(string.Format("Unable to download signing cert after {0} retries", MAX_RETRIES), e);
								} else {
									AWSSDKUtils.Sleep((int)(Math.Pow(4, retries) * 100));
								}
							}
						}
					}

					throw new Exception(string.Format("Unable to download signing cert after {0} retries", MAX_RETRIES));
				}
			}
		}
	}
}