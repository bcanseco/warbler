<p align="center">
 <a href="https://borja.io/warbler">
  <img height="200" alt="Warbler" src="https://github.com/bcanseco/warbler/blob/master/src/Warbler/Graphics/logo.png?raw=true"/>
 </a>
 <br/>
 <span>Auto-generated, geo-location based chatrooms for universities.</span>
</p>

## Dependencies
* **Git:** Available [here](https://git-scm.com/downloads) if you don't already have it.
* **Visual Studio 2017**: Click [here](https://www.microsoft.com/net/core) and follow the instructions.
* **npm**: Click [here](https://nodejs.org/) to download node.js, which comes with npm.  

npm is our client-side package manager.
* It stores any third-party packages in a `node_modules` folder inside [src/Warbler](https://github.com/bcanseco/warbler/tree/master/src/Warbler).
  * Due to how big these packages can get, we have Git [ignore that folder](https://github.com/bcanseco/warbler/blob/master/.gitignore#L6) to keep our repository size down.  
  * Each time you pull commits from Github, it's a good habit to run `npm install` in the same directory as [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json).
  * This command will pull all the dependencies listed inside [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json).
* This way, we can easily keep track of what Warbler depends on without having to push and pull a bunch of big files from GitHub.
* If you need to add a dependency:
  1. Search for the package on the [npm website](https://www.npmjs.com).
  2. Run `npm install --save <package name>`.
     * The `--save` flag will update [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json)'s `dependencies` list.
  3. Add any necessary `<script>` or `<link>` tags in the HTML as needed.

## Getting Started
1. Open a terminal window and navigate to a folder that you want the Warbler code to be in.
2. `git clone https://github.com/bcanseco/warbler.git Warbler`
3. `cd Warbler/src/Warbler`
4. `npm install`
5. Double-click [Warbler.sln](https://github.com/bcanseco/warbler/blob/master/Warbler.sln) to open the project in Visual Studio.

## Running
To build, hit <kbd>F5</kbd> in Visual Studio.

1. This will trigger an automatic task that moves folders in `node_modules` to `wwwroot/libs`.
   * ASP.NET doesn't serve files from the `node_modules` folder, so it's necessary to move them to `wwwroot`.
   * This is why you see references to `~/lib/` in the code - that simply refers to the `wwwroot/lib` folder.
    * Any `npm` dependencies should be referenced there.
      * e.g. `<script src="~/lib/example/script.js">` instead of `<script src="node_modules/example/script.js">`
   * You can do this manually at any time by hitting <kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>Backspace</kbd> and running the default gulp task.
2. Visual Studio will then build the project.
3. The webserver then (usually) runs on [http://localhost:59294/](http://localhost:59294/) with Chrome.  

You can set breakpoints in the backend code to debug as needed.
