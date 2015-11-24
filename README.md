### Prerequisites

* [MongoDB](https://www.mongodb.org/downloads)
* SQL Server
  * You must create the database before running the apps (see the connection string in the config files)

### HashBus.Twitter.Monitor configuration [Optional]

The `HashBus.Twitter.Monitor.Simulator` allows to test all the HashBus functionalities without using the public Twitter API/Feed. In order to run against the public Twitter API/Feed the following steps must be accomplished:

* Go to https://apps.twitter.com/ and create a new application;
* On the machine where the `HashBus.Twitter.Monitor` needs to run create the following `environment variables`:
   * `HASHBUS_TWITTER_CONSUMER_KEY`: the created Twitter app consumer key;
   * `HASHBUS_TWITTER_CONSUMER_SECRET`: the created Twitter app consumer secret;
   * `HASHBUS_TWITTER_ACCESS_TOKEN`: the created Twitter app access token;
   * `HASHBUS_TWITTER_ACCESS_TOKEN_SECRET`: the created Twitter app token secret;

### Web API

The web API is hosted at http://hashbus-demo.cloudapp.net:8080/

Available resources are:

* `http://hashbus-demo.cloudapp.net:8080/top-tweeters-retweeters/{track}`
* `http://hashbus-demo.cloudapp.net:8080/top-tweeters/{track}`
* `http://hashbus-demo.cloudapp.net:8080/top-retweeters/{track}`
* `http://hashbus-demo.cloudapp.net:8080/most-mentioned/{track}`
* `http://hashbus-demo.cloudapp.net:8080/most-retweeted/{track}`
* `http://hashbus-demo.cloudapp.net:8080/most-hashtagged/{track}`

A 'track' is a Twitter search term. At the time of writing, the HashBus Twitter monitor is running for the [#BuildStuffLT](https://twitter.com/search?q=%23BuildStuffLT) hashtag.

Here comes the funky thing. There is a [bug in Nancy](https://github.com/NancyFx/Nancy/issues/1154) which prevents a `#` (hash/pound) sign from being used in a URL, even if URL encoded. For this reason we use a special character sequence `해시` to represent `#`. (해시 means "hash" in Korean!)

Thus, example URL's for #BuildStuffLT are:

* http://hashbus-demo.cloudapp.net:8080/top-tweeters-retweeters/해시BuildStuffLT
* http://hashbus-demo.cloudapp.net:8080/top-tweeters/해시BuildStuffLT
* http://hashbus-demo.cloudapp.net:8080/top-retweeters/해시BuildStuffLT
* http://hashbus-demo.cloudapp.net:8080/most-mentioned/해시BuildStuffLT
* http://hashbus-demo.cloudapp.net:8080/most-retweeted/해시BuildStuffLT
* http://hashbus-demo.cloudapp.net:8080/most-hashtagged/해시BuildStuffLT

These URL's will give you a leaderboard object which looks like this:

```JSON
{
	"entries": [{
		"position": 1,
		"id": 1351703234,
		"idStr": "1351703234",
		"name": "Build Stuff 2015 LT",
		"screenName": "BuildStuffLT",
		"count": 28
	},
	{
		"position": 2,
		"id": 15528065,
		"idStr": "15528065",
		"name": "Malk’Zameth",
		"screenName": "malk_zameth",
		"count": 16
	},
	{
		"position": 3,
		"id": 183551266,
		"idStr": "183551266",
		"name": "Daniel Lee",
		"screenName": "danlimerick",
		"count": 15
	},
	{
		"position": 4,
		"id": 235599885,
		"idStr": "235599885",
		"name": "Peter Even",
		"screenName": "petervaneven",
		"count": 14
	},
	{
		"position": 5,
		"id": 22696598,
		"idStr": "22696598",
		"name": "Mauro Servienti",
		"screenName": "mauroservienti",
		"count": 10
	},
	{
		"position": 6,
		"id": 2511419816,
		"idStr": "2511419816",
		"name": "Jean-François Saguin",
		"screenName": "jfsaguin",
		"count": 9
	},
	{
		"position": 7,
		"id": 161837846,
		"idStr": "161837846",
		"name": "Bouillier Clément",
		"screenName": "clem_bouillier",
		"count": 7
	},
	{
		"position": 8,
		"id": 14128651,
		"idStr": "14128651",
		"name": "Grégory Weinbach",
		"screenName": "gweinbach",
		"count": 7
	},
	{
		"position": 9,
		"id": 8885582,
		"idStr": "8885582",
		"name": "Rui Carvalho",
		"screenName": "rhwy",
		"count": 6
	},
	{
		"position": 10,
		"id": 2375271441,
		"idStr": "2375271441",
		"name": "Ernestas Kardzys",
		"screenName": "ErnestasKardzys",
		"count": 6
	}],
	"count": 310,
	"since": "2015-11-17T16:17:41.0000000Z"
}
```
