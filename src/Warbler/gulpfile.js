/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    lib = "./wwwroot/lib";

/**
 * Removes the ./wwwroot/lib folder and all its contents.
 */
gulp.task("clean:lib", function (cb) {
    rimraf(lib, cb);
});

/**
 * Moves node_modules dependencies to ./wwwroot/libs
 */
gulp.task("move:lib", function() {
    return gulp.src([
            "node_modules/jquery/**/*.*",
            "node_modules/signalr/**/*.*",
            "node_modules/tether/**/*.*",
            "node_modules/bootstrap/**/*.*",
            "node_modules/knockout/**/*.*"
            /* Add npm dependencies here as needed. */
        ], { base: "./node_modules" })
        .pipe(gulp.dest(lib));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("default", gulp.series("clean:lib", "move:lib"));
