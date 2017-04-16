/// <binding BeforeBuild='default' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    fs = require("fs"),
    pack = JSON.parse(fs.readFileSync("./package.json")),
    lib = "./wwwroot/lib";

/**
 * Removes the ./wwwroot/lib folder and all its contents.
 */
gulp.task("clean:lib", function (cb) {
    rimraf(lib, cb);
});

/**
 * Moves npm dependencies to ./wwwroot/libs
 */
gulp.task("move:lib", function () {
    var libs = Object.keys(pack.dependencies).map(function(dep) {
        return "node_modules/" + dep + "/**/*.*";
    });

    return gulp
        .src(libs, { base: "./node_modules" })
        .pipe(gulp.dest(lib));
});

/**
 * This gulp task will run each of these in order.
 */
gulp.task("default", gulp.series("clean:lib", "move:lib"));
