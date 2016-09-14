### In Visual Studio 2015
1. Open command line tool at project folder
2. Enter "npm install" hit enter
3. In VS, locate Gruntfile.js, right click to launch Task runner explorer
4. Run 'compile' (This can be attached to pre-build event to automate the documentation, but may interfere with build on older VS-versions)

### From command line tool
(At project folder)  
1. Install grunt command line interface "npm install -g grunt-cli"
2. Install dependencies "npm install"
3. Test compilation of documentation "grunt compile" (run each time changes are made)