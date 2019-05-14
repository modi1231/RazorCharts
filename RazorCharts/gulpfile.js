var gulp = require('gulp');
gulp.task('js', function () {
    gulp.src(["node_modules/jquery/dist/jquery.js"])
        .pipe(gulp.dest("wwwroot/js"));

    gulp.src(["node_modules/bootstrap/dist/js/bootstrap.min.js"])
        .pipe(gulp.dest("wwwroot/js"));

    gulp.src(["node_modules/chart.js/dist/chart.js"])
        .pipe(gulp.dest("wwwroot/js"))
});

gulp.task('css', function () {
    gulp.src(["node_modules/bootstrap/dist/css/bootstrap.css"])
        .pipe(gulp.dest("wwwroot/css"));

    gulp.src(["node_modules/chart.js/dist/Chart.css"])
        .pipe(gulp.dest("wwwroot/css"));


});