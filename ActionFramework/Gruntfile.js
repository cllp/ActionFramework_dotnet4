module.exports = function(grunt) {

  grunt.loadNpmTasks('grunt-compile-handlebars');
  var marked = require('marked');
  var cheerio = require('cheerio');

  grunt.initConfig({
    "compile-handlebars":{
      dist: {
        files: [{
            src: 'Documentation/index.handlebars',
            dest: 'Documentation/dist/index.html'
        }
      ],
        templateData: "Documentation/tmp/data.json"
      }
    }
  });

  grunt.registerTask('compile', 'Compile all documents to single a file', function() {
    var styles = "";
    var scripts = "";
    var sections = {};
	   var assets = {};

	   grunt.file.recurse("Documentation/assets", function (path, root, sub, filename) {

		assets[path] = new Buffer(grunt.file.read(path, {encoding: null})).toString("base64");

	});

    grunt.file.recurse("Documentation/docs", function(path, root, sub, filename){
      if(sub) {
        var content = grunt.file.read(path);
        var name = path.replace(/[!\"#$%&'\(\)\*\+,\.\/:;<=>\?\@\[\\\]\^`\{\|\}~]/g, '').replace(" ","-");
        var title = filename.substring(0,filename.lastIndexOf("."));
        var fileType = filename.substring(filename.lastIndexOf("."));

        if(fileType == ".md") { //format any markdown files
          content = marked(content);
        }

        var $ = cheerio.load(content);
        $("img").each(function(i,e){ //
          var key = $(e).attr("src");
          if(assets[key]) {
            var type = key.substring(key.lastIndexOf(".") + 1);
      			if(type == "jpg") type = "jpeg";
            $(e).attr("src", "data:image/" + type + ";base64," + assets[key])
          }
        });

        $("code").each(function(i,e) {
          $(e).html($(e).html().replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;'));
        });
        content = $.html();

        var item = {
          content: content,
          name: name,
          title: title
        }
        if(!sections[sub]) {
          sections[sub] = [item];
        } else {
          sections[sub].push(item);
        }
      }
    });

    grunt.file.recurse("Documentation/styles", function (path) {
      if(path.substring(path.lastIndexOf(".")) == ".css") {
        styles += grunt.file.read(path);
      }
    });

    grunt.file.recurse("Documentation/scripts", function (path) {
      if(path.substring(path.lastIndexOf(".")) == ".js") {
        scripts += grunt.file.read(path);
      }
    });

    grunt.file.write("Documentation/dist/index.html", ""); //clear previous version

    grunt.file.write("Documentation/tmp/data.json", JSON.stringify({ sections: sections, styles: styles, scripts: scripts, assets: assets })); //store

    grunt.task.run("compile-handlebars");
  });
};
