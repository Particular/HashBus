angular.module('hashBusUI')
	.factory('poller', ['$timeout', '$q', function poller($timeout, $q)  {
		var tasks = [];

		function Task (opts) {
			this.opts = opts;
		}

		Task.prototype = {
			start: function () {
				var deferred = $q.defer(),
					self = this;

				(function tick () {

					var p = self.opts.action.apply(null, self.opts.params);

					p.then(function (data) {
						deferred.notify({ error: null, data: data.data });
					}, function (error) {
						deferred.notify({ error: error, data: null });
					});

					self.timeoutId = $timeout(tick, self.opts.interval);

				})();

				this.promise = deferred.promise;

				return this;
			},
			stop: function () {
				$timeout.cancel(this.timeoutId);
				this.timeoutId = null;
			},
			update: function (opts) {
				opts.interval = opts.interval || this.opts.interval;
				opts.params = opts.params || this.opts.params;
				opts.interval = opts.interval || this.opts.interval;
				opts.action = this.opts.action;
				this.opts = opts;
			}
		};

		function create (opts) {
			var task = new Task(opts);
			tasks.push(task);

			return task;
		}

		function stopAll () {
			var i = 0, len = tasks.length;
			for(; i < len; i++) {
				tasks[i].stop();
			}
		}

		function clear () {
			stopAll();
			tasks = [];
		}

		return {
			create: create,
			stopAll: stopAll,
			clear: clear
		};
	}]);