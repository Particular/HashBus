angular.module('hashBusUI')
	.controller('leaderboardsCtrl', ['$scope', '$http', 'urls', 'poller', '$interval',
		function leaderboardsCtrl($scope, $http, urls, poller, $interval) {
			var interval = 10000;
            function map(idProperty, count, previousResults, newResults){
                if(!previousResults){
                    return newResults;
                }
                for(index in newResults){
                    newResults[index].share = calculateShare(count, newResults[index].count);
                    var lastIndex = getLastIndex(idProperty, previousResults, newResults[index][idProperty]);
                    if(lastIndex == -1){
                       newResults[index].positiveChange = true; 
                    }else{
                        if(newResults[index].position > previousResults[lastIndex].position){
                           newResults[index].negativeChange = true;
                        }
                        if(newResults[index].position < previousResults[lastIndex].position){
                           newResults[index].positiveChange = true;
                        }
                    }
                }
                return newResults;
            }
            function getLastIndex(idProperty, items, id){
                for(idx in items){
                   if(items[idx][idProperty] == id){
                       return idx;
                   } 
                }
                return -1;
            }
            function calculateShare(totalCount, count){
                var result = (count * 100) / totalCount;
                return Math.round(result * 100) / 100
            }
		    var topTweetersRetweeters = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.topTweetersRetweeters)
		        }
		    });
		    topTweetersRetweeters.start();
		    topTweetersRetweeters.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.topTweetersRetweeters = map("id", response.data.count, $scope.topTweetersRetweeters, response.data.entries);
		            $scope.topTweetersRetweetersSince = response.data.since;
		            $scope.topTweetersRetweetersCount = response.data.count;
		        }
		    });
		    var mostMentioned = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.mostMentioned)
		        }
		    });
		    mostMentioned.start();
		    mostMentioned.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.mostMentioned = map("id", response.data.count, $scope.mostMentioned, response.data.entries);
		            $scope.mostMentionedSince = response.data.since;
		            $scope.mostMentionedCount = response.data.count;
		        }
		    });
		    var topRetweeters = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.topRetweeters)
		        }
		    });
		    topRetweeters.start();
		    topRetweeters.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.topRetweeters = map("id", response.data.count, $scope.topRetweeters, response.data.entries);
		            $scope.topRetweetersSince = response.data.since;
		            $scope.topRetweetersCount = response.data.count;
		        }
		    });
		    var mostRetweeted = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.mostRetweeted)
		        }
		    });
		    mostRetweeted.start();
		    mostRetweeted.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.mostRetweeted = map("id", response.data.count, $scope.mostRetweeted, response.data.entries);
		            $scope.mostRetweetedSince = response.data.since;
		            $scope.mostRetweetedCount = response.data.count;
		        }
		    });
		    var topTweeters = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.topTweeters)
		        }
		    });
		    topTweeters.start();
		    topTweeters.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.topTweeters = map("id", response.data.count, $scope.topTweeters, response.data.entries);
		            $scope.topTweetersSince = response.data.since;
		            $scope.topTweetersCount = response.data.count;
		        }
		    });
		    var mostHashtagged = poller.create({
		        interval: interval,
		        action: function () {
		            return $http.get(urls.mostHashTagged)
		        }
		    });
		    mostHashtagged.start();
		    mostHashtagged.promise.then(null, null, function (response) {
		        if (response.error) {
		            $scope.errorMessage = "help help, things aren't well";
		        } else {
		            $scope.errorMessage = '';
		            $scope.mostHashtagged = map("text", response.data.count, $scope.mostHashtagged, response.data.entries);
		            $scope.mostHashtaggedSince = response.data.since;
		            $scope.mostHashtaggedCount = response.data.count;
		        }
		    });
		}]);
