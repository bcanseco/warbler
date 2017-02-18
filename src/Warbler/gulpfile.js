﻿/// <binding BeforeBuild='default' Clean='clean' />
"use strict";

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify"),
  header = require("gulp-header"),
    less = require("gulp-less");

var paths = {
    webroot: "./wwwroot/",
    js: "./wwwroot/js/**/*.js",
    minJs: "./wwwroot/js/**/*.min.js",
    css: "./wwwroot/css/**/*.css",
    minCss: "./wwwroot/css/**/*.min.css",
    concatJsDest: "./wwwroot/js/site.min.js",
    concatCssDest: "./wwwroot/css/site.min.css"
};

// **********************************************************
// ****************** JAVASCRIPT SECTION ********************
// **********************************************************

/**
 * Removes the ./wwwroot/js folder and all its contents.
 */
gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

/**
 * Moves the .js files inside ./Scripts to ./wwwroot/js.
 */
gulp.task("move:js", function () {
    return gulp.src([
        "Scripts/**/*.*"
    ])
    .pipe(header("/**\n    DO NOT EDIT THIS FILE! IT IS DELETED ON EACH BUILD.\n    Edit files in ~/Scripts instead.\n*/\n"))
    .pipe(gulp.dest(paths.webroot + "js"));
});

/**
 * Minifies and concatenates the .js files inside ./wwwroot/js.
 */
gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
      .pipe(concat(paths.concatJsDest))
      .pipe(uglify())
      .pipe(gulp.dest("."));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("js", gulp.series("clean:js", "move:js", "min:js"));

// **********************************************************
// ****************** STYLESHEET SECTION ********************
// **********************************************************

/**
 * Removes the ./wwwroot/css folder and all its contents.
 */
gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

/**
 * Converts .less files in ./Styles to CSS in ./wwwroot/css.
 */
gulp.task("less:css", function () {
    return gulp.src("./Styles/**/*.less")
      .pipe(less({
          paths: [paths.webroot + "lib"]
      }))
      .pipe(header("/**\n    DO NOT EDIT THIS FILE! IT IS DELETED ON EACH BUILD.\n    Edit files in ~/Styles instead.\n*/\n"))
      .pipe(gulp.dest(paths.webroot + "css"));
});

/**
 * Minifies and concatenates the .CSS files inside ./wwwroot/css.
 */
gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
      .pipe(concat(paths.concatCssDest))
      .pipe(cssmin())
      .pipe(gulp.dest("."));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("css", gulp.series("clean:css", "less:css", "min:css"));

// **********************************************************
// ******************** MEDIA SECTION ***********************
// **********************************************************

/**
 * Removes the ./wwwroot/images folder and all its contents.
 */
gulp.task("clean:images", function (cb) {
    rimraf(paths.webroot + "images", cb);
});

/**
 * Moves the .png files in ./Graphics to ./wwwroot/images
 */
gulp.task("move:images", function () {
    return gulp
        .src(["Graphics/**/*.png"])
        .pipe(gulp.dest(paths.webroot + "images"));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("media", gulp.series("clean:images", "move:images"));

// **********************************************************
// ****************** DEPENDENCY SECTION ********************
// **********************************************************

/**
 * Removes the ./wwwroot/lib folder and all its contents.
 */
gulp.task("clean:lib", function (cb) {
    rimraf(paths.webroot + "lib", cb);
});

/**
 * Moves node_modules dependencies to ./wwwroot/libs
 */
gulp.task("move:lib", function() {
    return gulp.src([
            "node_modules/jquery/**/*.*",
            "node_modules/tether/**/*.*",
            "node_modules/bootstrap/**/*.*"
        ], { base: "./node_modules" })
        .pipe(gulp.dest(paths.webroot + "lib"));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("lib", gulp.series("clean:lib", "move:lib"));

// **********************************************************
// ******************* DEFAULT SECTION **********************
// **********************************************************

/**
 * This task will be run every time the project is built.
 */
gulp.task("default", gulp.parallel("js", "css", "media", "lib"));
