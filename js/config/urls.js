angular.module('hashBusUI')
	.factory('urls', function urls(){	
		return{
				topTweetersRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters-retweeters/해시',
				topTweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters/해시',
				topRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-retweeters/해시',
				mostMentioned:'http://hashbus-demo.cloudapp.net:8080/most-mentioned/해시',
				mostRetweeted:'http://hashbus-demo.cloudapp.net:8080/most-retweeted/해시',
				mostHashTagged:'http://hashbus-demo.cloudapp.net:8080/most-hashtagged/해시',
			}
	});
