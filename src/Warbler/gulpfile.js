/// <binding BeforeBuild='default' />
/* global require */

const gulp   = require("gulp");
const rimraf = require("rimraf");
const fs     = require("fs");
const pack   = JSON.parse(fs.readFileSync("./package.json"));
const lib    = "./wwwroot/lib";

gulp.task("cleaning wwwroot", (cb) => rimraf(lib, cb));

gulp.task("moving dependencies to wwwroot", () => {
    const libs = Object
        .keys(pack.dependencies)
        .map((dep) => `node_modules/${dep}/**/*.*`);

    return gulp
        .src(libs, { base: "./node_modules" })
        .pipe(gulp.dest(lib));
});

gulp.task("default", gulp.series("cleaning wwwroot", "moving dependencies to wwwroot"));
