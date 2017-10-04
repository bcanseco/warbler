<p align="center">
 <a href="https://borja.io/warbler">
  <img height="200" alt="Warbler" src="https://github.com/bcanseco/warbler/blob/master/src/Warbler/Graphics/logo.png?raw=true"/>
 </a>
 <br/>
 <span>Auto-generated, geo-location based chatrooms for universities.</span>
</p>

## Table of Contents
- [Dependencies](#dependencies)
- [Installing](#installing)
- [Running](#running)
- [Webdev](#webdev)
- [Testing](#testing)

## Dependencies
* **Git:** Available [here](https://git-scm.com/downloads) if you don't already have it.
* **Visual Studio 2017**: Click [here](https://www.microsoft.com/net/core) and follow the instructions.
* **npm**: Click [here](https://nodejs.org/) to download node.js, which comes with npm.  

npm is our client-side package manager.
1. It stores any third-party packages in a `node_modules` folder inside [src/Warbler](https://github.com/bcanseco/warbler/tree/master/src/Warbler).
   * Due to how big these packages can get, we have Git ignore that folder to keep our repository size down.  
   * Each time you pull commits from Github, it's a good habit to run `npm install` in the same directory as [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json).
      * This command will pull all the dependencies listed inside [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json).
1. This way, we can easily keep track of what Warbler depends on without having to push and pull a bunch of big files from GitHub.
1. If you need to add a dependency:
   1. Search for the package on the [npm website](https://www.npmjs.com).
   1. Run `npm install --save <package name>`.
      * The `--save` flag will update [package.json](https://github.com/bcanseco/warbler/blob/master/src/Warbler/package.json)'s `dependencies` list.
   1. Add the name of the package to the [webpack vendor config](https://github.com/bcanseco/warbler/blob/master/src/Warbler/webpack.config.vendor.js).
   1. Import it in your component using the [ES6 `import` keyword](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/import) - more details in the **Front-End Development** section.

## Installing
1. Open a terminal window and navigate to a folder that you want the Warbler code to be in.
1. `git clone https://github.com/bcanseco/warbler.git Warbler`
1. `cd Warbler/src/Warbler`
1. `npm install`
1. Double-click [Warbler.sln](https://github.com/bcanseco/warbler/blob/master/Warbler.sln) to open the project in Visual Studio.

## Running
To build, hit <kbd>F5</kbd> in Visual Studio.

1. This will trigger our webpack scripts.
   * The [vendor script](https://github.com/bcanseco/warbler/blob/master/src/Warbler/webpack.config.vendor.js) takes third party dependencies, bunches them together, and spits them out into the `wwwroot/dist` folder. You'll see a `vendor.js` and `vendor.css`, which contain Bootstrap, SignalR, and other codebases. These are referenced on all pages.
   * The [normal script](https://github.com/bcanseco/warbler/blob/master/src/Warbler/webpack.config.js) does the same thing but with our own code. You'll see our React components and other static files being spit out in the `wwwroot/dist` folder.
1. Visual Studio will then build the project.
1. The webserver then (usually) runs on [http://localhost:1337/](http://localhost:1337/) with Chrome.
1. .NET Core is aware of the webpack script, and will hot reload any changes you make.
   * For React scripts inside [Component folders](https://github.com/bcanseco/warbler/blob/master/src/Warbler/Components), no page refresh is necessary to see changes.
   * For Less styles inside [Component folders](https://github.com/bcanseco/warbler/blob/master/src/Warbler/Components), no page refresh is necessary to see changes.

You can set breakpoints in the backend code to debug as needed. Use `Debug` mode to use a local DB, or `Release` to use the Azure DB.

## Webdev
All new pages should be made using [React](https://reactjs.org/) components. For examples, see the [Components](https://github.com/bcanseco/warbler/blob/master/src/Warbler/Components) folder.

### Making a new component
1. Make a subfolder inside the Components folder, call it something appropriate.
1. Make an `index.jsx` file in there.
   * It's recommended that you copy and adapt the [Dev](https://github.com/bcanseco/warbler/blob/master/src/Warbler/Components/Dev/index.jsx) one - be sure to keep the hot reloading stuff at the bottom.
   * This file will contain your root component.
1. Make a `styles.less` file in there too - this is where your CSS will live. It gets imported at the top of the JSX index.
1. If you want to render subcomponents (it's a good idea to make your code as modular as possible), feel free to make other JSX files in this folder or in further subfolders.
1. To actually use your component, a page needs to serve it. Here's an example of a controller returning a React component:

   ```csharp
   public IActionResult Index()
   {
       ViewData["Title"] = "Developer Panel";
       ViewData["Heading"] = true;
       ViewData["Component"] = "Dev"; // Components/Dev

       return View("ReactComponent", ViewData);
   }
   ```

### ViewData options:
* **Title** - Title of the page. *(REQUIRED)*
* **Component** - The name of your component's subfolder. *(REQUIRED)* 
* **Heading** - Render the [heading partial](https://github.com/bcanseco/warbler/blob/master/src/Warbler/Views/Shared/_HeadingPartial.cshtml) on top of your component *(default: false)*
* **OtherScripts** - A list of scripts to inject before loading the component's script. You shouldn't need to use this; most libraries nowadays support ES6 importing *(default: empty)*

We use [Less](http://lesscss.org/) for CSS styling. Since Less is a superset of CSS, you can just write normal CSS if you don't want to take advantage of Less's features. All you have to do is make sure you're editing a `.less` file. Please use [kebab-case](http://wiki.c2.com/?KebabCase) for identifiers.

If you want to import an image into the project, just drag it into the [Graphics folder]() and rebuild. You can reference it as such: `~/dist/<file>.<ext>`
* Note that React handles `src` attributes on images differently: https://stackoverflow.com/a/26065890/6609109

## Testing
All tests should be in the [Warbler.Tests](src/Warbler.Tests) project.
* Try to match the folder structure from Warbler
   * e.g. If you're testing classes in [Warbler/Misc](src/Warbler/Misc), your tests should be in Warbler.Tests/Misc.
   * This applies for subfolders too.
* Match the name of the class you're testing, but prepend "Test".
   * e.g. If you're testing [University.cs](src/Warbler/Models/University.cs), your test class would be called "TestUniversity.cs".
* Use the naming convention `MethodName_ExpectedBehavior`.
   * When testing that the `CreateAsync()` method works in its expected (successful) scenario, you might use `CreateAsync_Should_Create_A_New_University()`.
   * When testing that the `CreateAsync()` method fails in an expected yet unsuccessful scenario, you might use `CreateAsync_Fails_When_University_Name_Is_Null()`.
* We are using MSTest as our unit testing framework. You can read docs for it [here](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest#adding-more-features). Key points:
   * Test classes should have a `[TestClass]` [attribute](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/attributes).
   * Test methods should have a `[TestMethod]` attribute.
   * You set up your own variables and test using the `Assert` class.
     * Type `Assert.` in Visual Studio to see available methods, e.g. `AreEqual()` and `IsFalse()`.
* We have also imported a third party library called [Moq](https://github.com/Moq/moq4/wiki/Quickstart). This library is useful for mocking a complicated part of the program when you only really want to test a small part of it.
   1. As a practical example, we'll use Moq with this method from SqlUniversityRepository:
      ```csharp
      public async Task SaveAsync()
      {
          // we want to test if this method gets called.
          await Context.SaveChangesAsync();
      }
      ```
   1. `SaveChangesAsync()` is a method of the `Context` property of the repository. Since we just want a test to ensure it gets called from `SaveAsync()`, here's how we can mock it:
      ```csharp
      [TestMethod]
      public async Task SaveASync_Should_Save_Changes_On_Context()
      {
          var saveChangesCalled = false;

          // Create a fake WarblerDbContext
          var mockContext = new Mock<WarblerDbContext>(Options);

          // Watch its SaveChangesAsync() method to see if it gets called by the repo
          mockContext.Setup(x => x.SaveChangesAsync(default(CancellationToken)))
              .Callback(() => saveChangesCalled = true)
              .ReturnsAsync(0);

          // Attach it to the repo and call SaveAsync()
          var repo = new SqlUniversityRepository(mockContext.Object);
          await repo.SaveAsync();

          Assert.IsTrue(saveChangesCalled);
      }
      ```
   1. You can view the full test class for this example [here](src/Warbler.Tests/Repositories/TestSqlUniversityRepository.cs).
* You can run tests in Visual Studio by clicking on **Test > Windows > Test Explorer**.
   * You can also run tests from a terminal using `dotnet test`.