using Amazon.SimpleNotificationService.Util;
using Xunit;

namespace AWSSDKNetCoreSupportTest {
	public class MessageExtensionTest {
		public const string DUMMY = "{\"Type\" : \"SubscriptionConfirmation\",  \"MessageId\" : \"932822b7-8e4d-405a-8281-cdd155027d17\",  \"Token\" : \"2336412f37fb687f5d51e6e241d59b68c9f4190787f25c55e7b1f9392d64d8138bea8d83ef0340c733ff78df6923954cbd920ed03edfbac21c36aba276ace625e83881dd351090d27d0534f97a581c1d7aa331a8f93162a7b7a7626dadb1a07ed9b309d175c4b50deeb753348f4ed70243280da988d9f9350b2919e272b67ba47196ddcf8323a8394c9ba3e478d4c1e9\",  \"TopicArn\" : \"arn:aws:sns:ap-southeast-2:335346329915:topic-aws-sdk-net-core-support-test\",  \"Message\" : \"You have chosen to subscribe to the topic arn:aws:sns:ap-southeast-2:335346329915:topic-aws-sdk-net-core-support-test.\\nTo confirm the subscription, visit the SubscribeURL included in this message.\",  \"SubscribeURL\" : \"https://sns.ap-southeast-2.amazonaws.com/?Action=ConfirmSubscription&TopicArn=arn:aws:sns:ap-southeast-2:335346329915:topic-aws-sdk-net-core-support-test&Token=2336412f37fb687f5d51e6e241d59b68c9f4190787f25c55e7b1f9392d64d8138bea8d83ef0340c733ff78df6923954cbd920ed03edfbac21c36aba276ace625e83881dd351090d27d0534f97a581c1d7aa331a8f93162a7b7a7626dadb1a07ed9b309d175c4b50deeb753348f4ed70243280da988d9f9350b2919e272b67ba47196ddcf8323a8394c9ba3e478d4c1e9\",  \"Timestamp\" : \"2017-04-12T07:40:51.227Z\",  \"SignatureVersion\" : \"1\",  \"Signature\" : \"ji9XK3hdEW9HmEcThEC2DFIjcOO7pomGIJ+3FELre6pFjKGXqDO4k8dqpD5RcQHzIHpOswpnsdUqhnnlMS0O8Sd75EzHahQmtV55+98qrF+1nDjZtHithsGyzzBCkHEqxo7FEDoJW1i3X2MbEmri2FRbrX2B0v5Fe1fbm87PDtFAlegOHy63uwjqA3jG+jEqwF/4Hi0tywI6n/Hv+RyqWyGT6RNcEV6nun4OjduJv5QHaTpdESLiO3lvNZq7wbcz1MPMQ2F+tK7spEdB570isAn8Yy3nMtF3jo3tNfJWDgnszAxCOFw4PFlOZn6pRHeTf/5Vrd1L9n9xM3jNquHz7w==\",  \"SigningCertURL\" : \"https://sns.ap-southeast-2.amazonaws.com/SimpleNotificationService-b95095beb82e8f6a046b3aafc7f4149a.pem\"}";

		[Fact]
		public void MessageSignatureValidTest() {
			var message = Message.ParseMessage(DUMMY);
			Assert.True(message.IsMessageSignatureValid());
		}
	}
}