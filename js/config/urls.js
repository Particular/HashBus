angular.module('hashBusUI')
	.factory('urls', function urls(){
		return{
				topTweetersRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters-retweeters/해시BuildStuffUA',
				topTweeters:'http://hashbus-demo.cloudapp.net:8080/top-tweeters/해시BuildStuffUA',
				topRetweeters:'http://hashbus-demo.cloudapp.net:8080/top-retweeters/해시BuildStuffUA',
				mostMentioned:'http://hashbus-demo.cloudapp.net:8080/most-mentioned/해시BuildStuffUA',
				mostRetweeted:'http://hashbus-demo.cloudapp.net:8080/most-retweeted/해시BuildStuffUA',
				mostHashTagged:'http://hashbus-demo.cloudapp.net:8080/most-hashtagged/해시BuildStuffUA',
			}
	});
