angular.module('hashBusUI', ['ui.router']);
angular.module('hashBusUI').config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/leaderboards");
    $stateProvider
      .state('leaderboards', {
          url: "/leaderboards?hashtag",
          controller: 'leaderboardsCtrl',
          templateUrl: "js/templates/leaderboards.tpl.html"
      })
});
