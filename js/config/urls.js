angular.module('hashBusUI')
	.factory('urls', function urls(){	
		return{
				topTweetersRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters-retweeters/해시BuildStuffLT',
				topTweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters/해시BuildStuffLT',
				topRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-retweeters/해시BuildStuffLT',
				mostMentioned:'http://hashbus-demo.cloudapp.net:8080/most-mentioned/해시BuildStuffLT',
				mostRetweeted:'http://hashbus-demo.cloudapp.net:8080/most-retweeted/해시BuildStuffLT',
				mostHashTagged:'http://hashbus-demo.cloudapp.net:8080/most-hashtagged/해시BuildStuffLT',
			}
	});
