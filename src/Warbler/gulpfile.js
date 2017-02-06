/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean:lib", function(cb) {
    rimraf(paths.webroot + "lib", cb);
});

gulp.task("clean", gulp.series("clean:js", "clean:css", "clean:lib"));

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
      .pipe(concat(paths.concatJsDest))
      .pipe(uglify())
      .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
      .pipe(concat(paths.concatCssDest))
      .pipe(cssmin())
      .pipe(gulp.dest("."));
});

gulp.task("min", gulp.series("min:js", "min:css"));

// move node_modules dependencies to the lib folder inside wwwroot
// (wwwroot is the only thing ASP.NET actually serves on the frontend)
gulp.task("deps", function() {
    return gulp.src([
            "node_modules/jquery/**/*.*",
            "node_modules/tether/**/*.*",
            "node_modules/bootstrap/**/*.*"
        ], { base: "./node_modules" })
        .pipe(gulp.dest(paths.webroot + "lib"));
});

gulp.task("default", gulp.series("clean", "min", "deps"));
